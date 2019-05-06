using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Dhcp.Callout.Native
{
    static class CalloutNative
    {
        private const uint ERROR_SUCCESS = 0;
        private const string ConsumerRegistryKey = @"SYSTEM\CurrentControlSet\Services\DHCPServer\Parameters";
        private const string ConsumerRegistryName = @"CalloutDotNetDlls";

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate uint DhcpServerCalloutEntryDelegate(IntPtr ChainDlls, uint CalloutVersion, ref DHCP_CALLOUT_TABLE CalloutTbl);

        private static uint ApiVersion;
        private static CalloutConsumerSupportFlags ConsumerRequirements;
        private static ConsumerInstance[] Consumers;
        private static DHCP_CALLOUT_TABLE ChainTable;
        private static IntPtr ChainLibraryHandle;

        /// <summary>
        /// The DhcpServerCalloutEntry function is called by Microsoft DHCP Server to initialize a
        /// third-party DLL, and to discover for which events the third-party DLL wants notification.
        /// The DhcpServerCalloutEntry function is implemented by third-party DLLs.
        /// </summary>
        /// <param name="ChainDlls">Collection of remaining third-party DLLs that provided registry entries requesting notification of DHCP Server events, in REG_MULTI_SZ format.</param>
        /// <param name="CalloutVersion">Version of the DHCP Server API that the third-party DLL is expected to support. The current version number is zero.</param>
        /// <param name="CalloutTbl">Cumulative set of notification hooks requested by all third-party DLLs, in the form of a DHCP_CALLOUT_TABLE structure.</param>
        /// <returns>Return values are defined by the application providing the callback.</returns>
        [DllExport(CallingConvention.StdCall)]
        private static uint DhcpServerCalloutEntry(IntPtr ChainDlls, uint CalloutVersion, ref DHCP_CALLOUT_TABLE CalloutTbl)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            ApiVersion = CalloutVersion;

            // locate and initialize consumers
            var initConsumersResult = InitializeConsumers();
            if (initConsumersResult != ERROR_SUCCESS)
            {
                return initConsumersResult;
            }

            // chain subsequent dll
            var chainDllResult = HandleChainDlls(ChainDlls, CalloutVersion);
            if (chainDllResult != ERROR_SUCCESS)
            {
                return chainDllResult;
            }

            // derive hooks
            var deriveResult = DeriveHooks(ref CalloutTbl);
            if (deriveResult != ERROR_SUCCESS)
            {
                return deriveResult;
            }

            return ERROR_SUCCESS;
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == typeof(CalloutNative).Assembly.FullName)
            {
                return typeof(CalloutNative).Assembly;
            }

            return null;
        }

        private static uint HandleChainDlls(IntPtr ChainDlls, uint CalloutVersion)
        {
            // ChainDlls is in REG_MULTI_SZ format (non-empty strings separated by null, terminated with an additional null)
            // read next dll in chain
            var chainDllPath = Marshal.PtrToStringUni(ChainDlls);

            // check if no dlls (or we are the last in the chain)
            if (chainDllPath == null || chainDllPath.Length == 0)
            {
                ChainTable = new DHCP_CALLOUT_TABLE();
                return ERROR_SUCCESS;
            }

            // ensure dll exists
            if (!File.Exists(chainDllPath))
            {
                return 0x2; // FILE_NOT_FOUND
            }

            // load dll
            ChainLibraryHandle = Win32Api.LoadLibrary(chainDllPath);
            if (ChainLibraryHandle == IntPtr.Zero)
            {
                return (uint)Marshal.GetLastWin32Error();
            }

            // locate DhcpServerCalloutEntry export
            var chainCalloutEntryAddress = Win32Api.GetProcAddress(ChainLibraryHandle, "DhcpServerCalloutEntry");
            if (chainCalloutEntryAddress == IntPtr.Zero)
            {
                return (uint)Marshal.GetLastWin32Error();
            }

            // prepare arguments
            var chainCalloutEntryDelegate = (DhcpServerCalloutEntryDelegate)Marshal.GetDelegateForFunctionPointer
                (chainCalloutEntryAddress, typeof(DhcpServerCalloutEntryDelegate));

            // ChainDlls should pass a pointer to the next dll (or the final termination null)
            var chainDllsNext = ChainDlls + Encoding.Unicode.GetByteCount(chainDllPath) + 2;
            var calloutTbl = new DHCP_CALLOUT_TABLE();

            // call chain dll
            var subsequentResult = chainCalloutEntryDelegate(chainDllsNext, CalloutVersion, ref calloutTbl);
            if (subsequentResult != ERROR_SUCCESS)
            {
                return subsequentResult;
            }

            // store chain dll's callout table
            ChainTable = calloutTbl;

            return ERROR_SUCCESS;
        }

        private static uint InitializeConsumers()
        {
            // locate (only first match):
            // - registry (REG_MULTI_SZ)
            // - registry (REG_SZ)
            // - .net dll's in same path as self

            try
            {
                // try registry
                using (var regKey = Registry.LocalMachine.OpenSubKey(ConsumerRegistryKey))
                {
                    if (regKey != null && regKey.GetValueNames().Contains(ConsumerRegistryName, StringComparer.OrdinalIgnoreCase))
                    {
                        var regValueType = regKey.GetValueKind(ConsumerRegistryName);

                        switch (regValueType)
                        {
                            case RegistryValueKind.String:
                                var assemblyPath = (string)regKey.GetValue(ConsumerRegistryName);

                                if (string.IsNullOrWhiteSpace(assemblyPath) || !File.Exists(assemblyPath))
                                    return 0x2U; // ERROR_FILE_NOT_FOUND

                                if (!IsManagedAssembly(assemblyPath))
                                    return 0xBU; // ERROR_BAD_FORMAT

                                return InitializeConsumers(new List<string>() { assemblyPath }, false);
                            case RegistryValueKind.MultiString:
                                var assemblyPaths = (string[])regKey.GetValue(ConsumerRegistryName);

                                if (assemblyPaths == null || assemblyPaths.Length == 0)
                                    return 0x2U; // ERROR_FILE_NOT_FOUND

                                for (var i = 0; i < assemblyPaths.Length; i++)
                                {
                                    if (string.IsNullOrWhiteSpace(assemblyPaths[i]) || !File.Exists(assemblyPaths[i]))
                                        return 0x2U; // ERROR_FILE_NOT_FOUND

                                    if (!IsManagedAssembly(assemblyPaths[i]))
                                        return 0xBU; // ERROR_BAD_FORMAT
                                }

                                return InitializeConsumers(new List<string>(assemblyPaths), false);
                            default:
                                return 0x3F3U; // ERROR_CANTOPEN
                        }
                    }
                }

                // try host directory
                var hostLocation = typeof(CalloutNative).Assembly.Location;
                if (File.Exists(hostLocation))
                {
                    var hostFilename = Path.GetFileName(hostLocation);
                    var hostDirectory = Path.GetDirectoryName(hostLocation);
                    var assemblyPaths = new List<string>();

                    foreach (var assemblyPath in Directory.EnumerateFiles(hostDirectory, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        // ignore host
                        if (Path.GetFileName(assemblyPath).Equals(hostFilename, StringComparison.OrdinalIgnoreCase))
                            continue;

                        // add managed assemblies
                        if (IsManagedAssembly(assemblyPath))
                            assemblyPaths.Add(assemblyPath);
                    }

                    if (assemblyPaths.Count > 0)
                    {
                        return InitializeConsumers(assemblyPaths, true);
                    }
                }

                return 0x2U; // ERROR_FILE_NOT_FOUND
            }
            catch (Win32Exception ex)
            {
                return (uint)ex.NativeErrorCode;
            }
            catch (Exception)
            {
                return 0x1U; //ERROR_INVALID_FUNCTION
            }
        }

        private static uint InitializeConsumers(List<string> AssemblyPaths, bool IgnoreBadFormat)
        {
            if (AssemblyPaths == null || AssemblyPaths.Count == 0)
                return 0x2U; // ERROR_FILE_NOT_FOUND

            var hostAssemblyPath = typeof(CalloutNative).Assembly.Location;
            var proxyTypeName = typeof(DhcpServerCalloutConsumerProxy).FullName;
            var instances = new List<ConsumerInstance>(AssemblyPaths.Count);

            try
            {
                foreach (var assemblyPath in AssemblyPaths)
                {
                    var domainSetup = new AppDomainSetup()
                    {
                        ApplicationBase = Path.GetDirectoryName(assemblyPath),
                        LoaderOptimization = LoaderOptimization.MultiDomain
                    };

                    var instance = new ConsumerInstance()
                    {
                        AssemblyPath = assemblyPath
                    };
                    instances.Add(instance);

                    instance.AppDomain = AppDomain.CreateDomain($"DhcpServerCalloutConsumer: {Path.GetFileName(assemblyPath)}", null, domainSetup);
                    var proxy = (DhcpServerCalloutConsumerProxy)instance.AppDomain.CreateInstanceFromAndUnwrap(hostAssemblyPath, proxyTypeName);

                    var initResult = proxy.TryInitialize(assemblyPath, ApiVersion);

                    if (IgnoreBadFormat && initResult == 0xBU) // ERROR_BAD_FORMAT
                    {
                        // skip assembly
                        instances.Remove(instance);
                        AppDomain.Unload(instance.AppDomain);
                        continue;
                    }

                    if (initResult != ERROR_SUCCESS)
                    {
                        throw new Win32Exception((int)initResult);
                    }

                    instance.Proxy = proxy;
                    instance.SupportFlags = proxy.GetSupportFlags();
                }
            }
            catch (Exception)
            {
                // dispose and unload previously created instances
                foreach (var instance in instances)
                {
                    try
                    {
                        instance.Proxy?.Dispose();
                        instance.Proxy = null;
                    }
                    catch (Exception) { }

                    if (instance.AppDomain != null)
                    {
                        try
                        {
                            AppDomain.Unload(instance.AppDomain);
                            instance.AppDomain = null;
                        }
                        catch (Exception) { }
                    }
                }

                throw;
            }

            if (instances.Count == 0)
                return 0x2U; // ERROR_FILE_NOT_FOUND

            Consumers = instances.ToArray();
            ConsumerRequirements = instances.Aggregate(
                (CalloutConsumerSupportFlags)0, (a, i) => a | i.SupportFlags);

            return ERROR_SUCCESS;
        }

        private static bool IsManagedAssembly(string AssemblyPath)
        {
            if (!File.Exists(AssemblyPath))
                return false;

            var buffer = new byte[0x0108];
            var bufferLength = default(int);

            using (var stream = new FileStream(AssemblyPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // read DOS header
                bufferLength = stream.Read(buffer, 0, 0x40);

                // read e_lfanew (new exe header)
                if (bufferLength < 0x40)
                    return false;

                // check for DOS header magic 'MZ'
                if (buffer[0] != 0x4D || buffer[1] != 0x5A)
                    return false;

                // locate exe header
                var e_lfanew = BitConverter.ToInt32(buffer, 0x3C);
                if (stream.Length <= e_lfanew + 0x18)
                    return false;

                // read NT header
                stream.Position = e_lfanew;
                bufferLength = stream.Read(buffer, 0, buffer.Length);
            }

            // check for NT header signature 'PE__' (50 45 00 00)
            if (buffer[0] != 0x50 || buffer[1] != 0x45 || buffer[2] != 0x00 || buffer[3] != 0x00)
                return false;

            var headerOffset = 0x04;

            // get machine
            //var machine = BitConverter.ToUInt16(buffer, headerOffset);

            // optional header size
            var optionalheaderSize = BitConverter.ToUInt16(buffer, headerOffset + 0x10);
            if (bufferLength < headerOffset + 0x14 + optionalheaderSize)
                return false;

            var optionalHeaderOffset = headerOffset + 0x14;
            var optionalHeaderMagic = BitConverter.ToUInt16(buffer, optionalHeaderOffset);
            if (optionalHeaderMagic == 0x010B && optionalheaderSize == 0xE0) // PE32
            {
                var clrHeaderAddress = BitConverter.ToUInt32(buffer, optionalHeaderOffset + 0xD0);
                var clrHeaderSize = BitConverter.ToUInt32(buffer, optionalHeaderOffset + 0xD4);

                if (clrHeaderAddress != 0 && clrHeaderSize != 0)
                    return true;
            }
            else if (optionalHeaderMagic == 0x020B && optionalheaderSize == 0xF0) // PE32+
            {
                var clrHeaderAddress = BitConverter.ToUInt32(buffer, optionalHeaderOffset + 0xE0);
                var clrHeaderSize = BitConverter.ToUInt32(buffer, optionalHeaderOffset + 0xE4);

                if (clrHeaderAddress != 0 && clrHeaderSize != 0)
                    return true;
            }

            return false;
        }

        private static uint DeriveHooks(ref DHCP_CALLOUT_TABLE CalloutTbl)
        {
            // always hook control
            CalloutTbl.DhcpControlHook = new DhcpControlHookUnsafeDelegate(HandleControlHook);

            // NewPkt
            if (ConsumerRequirements.HasFlag(CalloutConsumerSupportFlags.NewPacket))
            {
                // consumer required
                CalloutTbl.DhcpNewPktHook = new DhcpNewPktHookUnsafeDelegate(HandleNewPktHook);
            }
            else if (ChainTable.DhcpNewPktHook != null)
            {
                // only chain required
                CalloutTbl.DhcpNewPktHook = ChainTable.DhcpNewPktHook;
            }
            else
            {
                CalloutTbl.DhcpNewPktHook = null;
            }

            // PktDrop
            if (ConsumerRequirements.HasFlag(CalloutConsumerSupportFlags.PacketDrop))
            {
                // consumer required
                CalloutTbl.DhcpPktDropHook = new DhcpPktDropHookUnsafeDelegate(HandleDhcpPktDropHook);
            }
            else if (ChainTable.DhcpPktDropHook != null)
            {
                // only chain required
                CalloutTbl.DhcpPktDropHook = ChainTable.DhcpPktDropHook;
            }
            else
            {
                CalloutTbl.DhcpPktDropHook = null;
            }

            // PktSend
            if (ConsumerRequirements.HasFlag(CalloutConsumerSupportFlags.PacketSend))
            {
                // consumer required
                CalloutTbl.DhcpPktSendHook = new DhcpPktSendHookUnsafeDelegate(HandlePktSendHook);
            }
            else if (ChainTable.DhcpPktSendHook != null)
            {
                // only chain required
                CalloutTbl.DhcpPktSendHook = ChainTable.DhcpPktSendHook;
            }
            else
            {
                CalloutTbl.DhcpPktSendHook = null;
            }

            // AddressDel
            if (ConsumerRequirements.HasFlag(CalloutConsumerSupportFlags.AddressDelete))
            {
                // consumer required
                CalloutTbl.DhcpAddressDelHook = new DhcpAddressDelHookUnsafeDelegate(HandleAddressDelHook);
            }
            else if (ChainTable.DhcpAddressDelHook != null)
            {
                // only chain required
                CalloutTbl.DhcpAddressDelHook = ChainTable.DhcpAddressDelHook;
            }
            else
            {
                CalloutTbl.DhcpAddressDelHook = null;
            }

            // AddressOffer
            if (ConsumerRequirements.HasFlag(CalloutConsumerSupportFlags.AddressOffer))
            {
                // consumer required
                CalloutTbl.DhcpAddressOfferHook = new DhcpAddressOfferHookUnsafeDelegate(HandleAddressOfferHook);
            }
            else if (ChainTable.DhcpAddressOfferHook != null)
            {
                // only chain required
                CalloutTbl.DhcpAddressOfferHook = ChainTable.DhcpAddressOfferHook;
            }
            else
            {
                CalloutTbl.DhcpAddressOfferHook = null;
            }

            // HandleOptions
            // Hook is only derived if the ApiVersion is 0 (as the ServerOptions could change if the version is incremented).
            if (ApiVersion == 0 && ConsumerRequirements.HasFlag(CalloutConsumerSupportFlags.HandleOptions))
            {
                // consumer required
                CalloutTbl.DhcpHandleOptionsHook = new DhcpHandleOptionsHookUnsafeDelegate(HandleHandleOptionsHook);
            }
            else if (ChainTable.DhcpHandleOptionsHook != null)
            {
                // only chain required
                CalloutTbl.DhcpHandleOptionsHook = ChainTable.DhcpHandleOptionsHook;
            }
            else
            {
                CalloutTbl.DhcpHandleOptionsHook = null;
            }

            // DeleteClient
            if (ConsumerRequirements.HasFlag(CalloutConsumerSupportFlags.DeleteClient))
            {
                // consumer required
                CalloutTbl.DhcpDeleteClientHook = new DhcpDeleteClientHookUnsafeDelegate(HandleDeleteClientHook);
            }
            else if (ChainTable.DhcpDeleteClientHook != null)
            {
                // only chain required
                CalloutTbl.DhcpDeleteClientHook = ChainTable.DhcpDeleteClientHook;
            }
            else
            {
                CalloutTbl.DhcpDeleteClientHook = null;
            }

            // Extension - not supported by DhcpServerAPI
            if (ChainTable.DhcpExtensionHook != IntPtr.Zero)
            {
                CalloutTbl.DhcpExtensionHook = ChainTable.DhcpExtensionHook;
            }
            else
            {
                CalloutTbl.DhcpExtensionHook = IntPtr.Zero;
            }

            // Reserved - not supported by DhcpServerAPI
            if (ChainTable.DhcpReservedHook != IntPtr.Zero)
            {
                CalloutTbl.DhcpReservedHook = ChainTable.DhcpReservedHook;
            }
            else
            {
                CalloutTbl.DhcpReservedHook = IntPtr.Zero;
            }

            return ERROR_SUCCESS;
        }

        private static uint HandleControlHook(uint dwControlCode, IntPtr lpReserved)
        {
            var result = ERROR_SUCCESS;

            // inform consumers
            foreach (var consumer in Consumers)
            {
                var localResult = ERROR_SUCCESS;

                try
                {
                    consumer.Proxy.Control((CalloutControlCodes)dwControlCode);
                }
                catch (Win32Exception ex)
                {
                    localResult = (uint)ex.NativeErrorCode;
                }
                catch (Exception)
                {
                    localResult = 0x1U; //ERROR_INVALID_FUNCTION
                }

                // return result from first error
                if (result == ERROR_SUCCESS)
                {
                    result = localResult;
                }
            }

            // chain
            var chainResult = ChainTable.DhcpControlHook?.Invoke(dwControlCode, lpReserved) ?? ERROR_SUCCESS;
            if (result == ERROR_SUCCESS)
            {
                result = chainResult;
            }

            // unload if dhcp stopping
            if ((CalloutControlCodes)dwControlCode == CalloutControlCodes.Stop)
            {
                try
                {
                    foreach (var consumer in Consumers)
                    {
                        try
                        {
                            consumer.Proxy?.Dispose();
                            consumer.Proxy = null;
                        }
                        catch (Exception) { }

                        if (consumer.AppDomain != null)
                        {
                            try
                            {
                                AppDomain.Unload(consumer.AppDomain);
                                consumer.AppDomain = null;
                            }
                            catch (Exception) { }
                        }
                    }
                }
                finally
                {
                    Consumers = new ConsumerInstance[0];
                }

                // unload chain dll
                if (ChainLibraryHandle != IntPtr.Zero)
                {
                    Win32Api.FreeLibrary(ChainLibraryHandle);
                }
            }

            return result;
        }

        private static uint HandleNewPktHook(ref IntPtr Packet, ref uint PacketSize, uint IpAddress, IntPtr Reserved, IntPtr PktContext, out bool ProcessIt)
        {
            ProcessIt = true;
            var stopPropagation = false;

            var packet = new DhcpServerPacketWritable(Packet, (int)PacketSize);
            var serverAddress = DhcpServerIpAddress.FromNative(IpAddress);

            foreach (var consumer in Consumers)
            {
                if (consumer.SupportFlags.HasFlag(CalloutConsumerSupportFlags.NewPacket))
                {
                    try
                    {
                        consumer.Proxy.NewPacket(packet, serverAddress, ref ProcessIt, ref stopPropagation);
                    }
                    catch (Win32Exception ex)
                    {
                        return (uint)ex.NativeErrorCode;
                    }
                    catch (Exception)
                    {
                        return 0x1U; //ERROR_INVALID_FUNCTION
                    }

                    // stop propagation if instructed or the packet is not to be processed
                    if (stopPropagation || !ProcessIt)
                        break;
                }
            }

            // write packet
            if (ProcessIt && packet.BufferModified)
            {
                packet.WriteBuffer(Packet, ref PacketSize);
            }

            // chain
            if (ProcessIt && !stopPropagation && ChainTable.DhcpNewPktHook != null)
            {
                return ChainTable.DhcpNewPktHook(ref Packet, ref PacketSize, IpAddress, Reserved, PktContext, out ProcessIt);
            }

            return ERROR_SUCCESS;
        }

        private static uint HandleDhcpPktDropHook(ref IntPtr Packet, ref uint PacketSize, uint ControlCode, uint IpAddress, IntPtr Reserved, IntPtr PktContext)
        {
            var stopPropagation = false;

            var packet = new DhcpServerPacket(Packet, (int)PacketSize);
            var controlCode = (PacketDropControlCodes)ControlCode;
            var serverAddress = DhcpServerIpAddress.FromNative(IpAddress);

            foreach (var consumer in Consumers)
            {
                if (consumer.SupportFlags.HasFlag(CalloutConsumerSupportFlags.PacketDrop))
                {
                    try
                    {
                        consumer.Proxy.PacketDrop(packet, controlCode, serverAddress, ref stopPropagation);
                    }
                    catch (Win32Exception ex)
                    {
                        return (uint)ex.NativeErrorCode;
                    }
                    catch (Exception)
                    {
                        return 0x1U; //ERROR_INVALID_FUNCTION
                    }

                    // stop propagation if instructed
                    if (stopPropagation)
                        return ERROR_SUCCESS;
                }
            }

            // chain
            if (!stopPropagation && ChainTable.DhcpPktDropHook != null)
            {
                return ChainTable.DhcpPktDropHook(ref Packet, ref PacketSize, ControlCode, IpAddress, Reserved, PktContext);
            }

            return ERROR_SUCCESS;
        }

        private static uint HandlePktSendHook(ref IntPtr Packet, ref uint PacketSize, uint ControlCode, uint IpAddress, IntPtr Reserved, IntPtr PktContext)
        {
            var stopPropagation = false;

            var packet = new DhcpServerPacketWritable(Packet, (int)PacketSize);
            var serverAddress = DhcpServerIpAddress.FromNative(IpAddress);
            // ControlCode is ignored in this wrapper as:
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363294.aspx
            // "The only acceptable value in this version of the DHCP Server API is DHCP_SEND_PACKET."

            foreach (var consumer in Consumers)
            {
                if (consumer.SupportFlags.HasFlag(CalloutConsumerSupportFlags.PacketSend))
                {
                    try
                    {
                        consumer.Proxy.PacketSend(packet, serverAddress, ref stopPropagation);
                    }
                    catch (Win32Exception ex)
                    {
                        return (uint)ex.NativeErrorCode;
                    }
                    catch (Exception)
                    {
                        return 0x1U; //ERROR_INVALID_FUNCTION
                    }

                    // stop propagation if instructed
                    if (stopPropagation)
                        break;
                }
            }

            // write packet
            if (packet.BufferModified)
            {
                packet.WriteBuffer(Packet, ref PacketSize);
            }

            // chain
            if (!stopPropagation && ChainTable.DhcpPktSendHook != null)
            {
                return ChainTable.DhcpPktSendHook(ref Packet, ref PacketSize, ControlCode, IpAddress, Reserved, PktContext);
            }

            return ERROR_SUCCESS;
        }

        private static uint HandleAddressDelHook(IntPtr Packet, uint PacketSize, uint ControlCode, uint IpAddress, uint AltAddress, IntPtr Reserved, IntPtr PktContext)
        {
            var stopPropagation = false;

            var packet = new DhcpServerPacket(Packet, (int)PacketSize);
            var controlCode = (AddressDeleteControlCodes)ControlCode;
            var serverAddress = DhcpServerIpAddress.FromNative(IpAddress);
            var leaseAddress = DhcpServerIpAddress.FromNative(AltAddress);

            foreach (var consumer in Consumers)
            {
                if (consumer.SupportFlags.HasFlag(CalloutConsumerSupportFlags.AddressDelete))
                {
                    try
                    {
                        consumer.Proxy.AddressDelete(packet, controlCode, serverAddress, leaseAddress, ref stopPropagation);
                    }
                    catch (Win32Exception ex)
                    {
                        return (uint)ex.NativeErrorCode;
                    }
                    catch (Exception)
                    {
                        return 0x1U; //ERROR_INVALID_FUNCTION
                    }

                    // stop propagation if instructed
                    if (stopPropagation)
                        return ERROR_SUCCESS;
                }
            }

            // chain
            if (!stopPropagation && ChainTable.DhcpAddressDelHook != null)
            {
                return ChainTable.DhcpAddressDelHook(Packet, PacketSize, ControlCode, IpAddress, AltAddress, Reserved, PktContext);
            }

            return ERROR_SUCCESS;
        }

        private static uint HandleAddressOfferHook(IntPtr Packet, uint PacketSize, uint ControlCode, uint IpAddress, uint AltAddress, uint AddrType, uint LeaseTime, IntPtr Reserved, IntPtr PktContext)
        {
            var stopPropagation = false;

            var packet = new DhcpServerPacket(Packet, (int)PacketSize);
            var controlCode = (OfferAddressControlCodes)ControlCode;
            var serverAddress = DhcpServerIpAddress.FromNative(IpAddress);
            var leaseAddress = DhcpServerIpAddress.FromNative(AltAddress);
            var addressType = (OfferAddressTypes)AddrType;
            var leaseTime = new TimeSpan(LeaseTime * 10_000_000L);

            foreach (var consumer in Consumers)
            {
                if (consumer.SupportFlags.HasFlag(CalloutConsumerSupportFlags.AddressOffer))
                {
                    try
                    {
                        consumer.Proxy.AddressOffer(packet, controlCode, serverAddress, leaseAddress, addressType, leaseTime, ref stopPropagation);
                    }
                    catch (Win32Exception ex)
                    {
                        return (uint)ex.NativeErrorCode;
                    }
                    catch (Exception)
                    {
                        return 0x1U; //ERROR_INVALID_FUNCTION
                    }

                    if (stopPropagation)
                        return ERROR_SUCCESS;
                }
            }

            // chain
            if (!stopPropagation && ChainTable.DhcpAddressOfferHook != null)
            {
                return ChainTable.DhcpAddressOfferHook(Packet, PacketSize, ControlCode, IpAddress, AltAddress, AddrType, LeaseTime, Reserved, PktContext);
            }

            return ERROR_SUCCESS;
        }

        private static uint HandleHandleOptionsHook(IntPtr Packet, uint PacketSize, IntPtr Reserved, IntPtr PktContext, IntPtr ServerOptions)
        {
            var stopPropagation = false;

            var packet = new DhcpServerPacket(Packet, (int)PacketSize);
            var serverOptions = new DhcpServerPacketOptions(ServerOptions);

            foreach (var consumer in Consumers)
            {
                if (consumer.SupportFlags.HasFlag(CalloutConsumerSupportFlags.HandleOptions))
                {
                    try
                    {
                        consumer.Proxy.HandleOptions(packet, serverOptions, ref stopPropagation);
                    }
                    catch (Win32Exception ex)
                    {
                        return (uint)ex.NativeErrorCode;
                    }
                    catch (Exception)
                    {
                        return 0x1U; //ERROR_INVALID_FUNCTION
                    }

                    // stop propagation if instructed
                    if (stopPropagation)
                        return ERROR_SUCCESS;
                }
            }

            // chain
            if (!stopPropagation && ChainTable.DhcpHandleOptionsHook != null)
            {
                return ChainTable.DhcpHandleOptionsHook(Packet, PacketSize, Reserved, PktContext, ServerOptions);
            }

            return ERROR_SUCCESS;
        }

        private static uint HandleDeleteClientHook(uint IpAddress, IntPtr HwAddress, uint HwAddressLength, uint Reserved, uint ClientType)
        {
            var stopPropagation = false;

            var leaseAddress = DhcpServerIpAddress.FromNative(IpAddress);
            if (HwAddressLength <= DhcpServerHardwareAddress.MaximumLength)
            {
                var leaseHardwareAddress = DhcpServerHardwareAddress.FromNative(DhcpServerHardwareType.Ethernet, HwAddress, (int)HwAddressLength);

                foreach (var consumer in Consumers)
                {
                    if (consumer.SupportFlags.HasFlag(CalloutConsumerSupportFlags.DeleteClient))
                    {
                        try
                        {
                            consumer.Proxy.DeleteClient(leaseAddress, leaseHardwareAddress, ref stopPropagation);
                        }
                        catch (Win32Exception ex)
                        {
                            return (uint)ex.NativeErrorCode;
                        }
                        catch (Exception)
                        {
                            return 0x1U; //ERROR_INVALID_FUNCTION
                        }

                        // stop propagation if instructed
                        if (stopPropagation)
                            return ERROR_SUCCESS;
                    }
                }
            }

            // chain
            if (!stopPropagation && ChainTable.DhcpDeleteClientHook != null)
            {
                return ChainTable.DhcpDeleteClientHook(IpAddress, HwAddress, HwAddressLength, Reserved, ClientType);
            }

            return ERROR_SUCCESS;
        }

        private class ConsumerInstance
        {
            public string AssemblyPath;
            public AppDomain AppDomain;
            public DhcpServerCalloutConsumerProxy Proxy;
            public CalloutConsumerSupportFlags SupportFlags;
        }
    }
}
