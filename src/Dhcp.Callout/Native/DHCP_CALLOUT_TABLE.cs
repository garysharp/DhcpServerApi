using System;
using System.Runtime.InteropServices;

namespace Dhcp.Callout.Native
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint DhcpControlHookUnsafeDelegate(uint dwControlCode, IntPtr lpReserved);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint DhcpNewPktHookUnsafeDelegate(ref IntPtr Packet, ref uint PacketSize, uint IpAddress, IntPtr Reserved, IntPtr PktContext, out bool ProcessIt);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint DhcpPktDropHookUnsafeDelegate(ref IntPtr Packet, ref uint PacketSize, uint ControlCode, uint IpAddress, IntPtr Reserved, IntPtr PktContext);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint DhcpPktSendHookUnsafeDelegate(ref IntPtr Packet, ref uint PacketSize, uint ControlCode, uint IpAddress, IntPtr Reserved, IntPtr PktContext);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint DhcpAddressDelHookUnsafeDelegate(IntPtr Packet, uint PacketSize, uint ControlCode, uint IpAddress, uint AltAddress, IntPtr Reserved, IntPtr PktContext);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint DhcpAddressOfferHookUnsafeDelegate(IntPtr Packet, uint PacketSize, uint ControlCode, uint IpAddress, uint AltAddress, uint AddrType, uint LeaseTime, IntPtr Reserved, IntPtr PktContext);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint DhcpHandleOptionsHookUnsafeDelegate(IntPtr Packet, uint PacketSize, IntPtr Reserved, IntPtr PktContext, IntPtr ServerOptions);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint DhcpDeleteClientHookUnsafeDelegate(uint IpAddress, IntPtr HwAddress, uint HwAddressLength, uint Reserved, uint ClientType);

    internal struct DHCP_CALLOUT_TABLE
    {
        public DhcpControlHookUnsafeDelegate DhcpControlHook;
        public DhcpNewPktHookUnsafeDelegate DhcpNewPktHook;
        public DhcpPktDropHookUnsafeDelegate DhcpPktDropHook;
        public DhcpPktSendHookUnsafeDelegate DhcpPktSendHook;
        public DhcpAddressDelHookUnsafeDelegate DhcpAddressDelHook;
        public DhcpAddressOfferHookUnsafeDelegate DhcpAddressOfferHook;
        public DhcpHandleOptionsHookUnsafeDelegate DhcpHandleOptionsHook;
        public DhcpDeleteClientHookUnsafeDelegate DhcpDeleteClientHook;
        public IntPtr DhcpExtensionHook;
        public IntPtr DhcpReservedHook;
    }
}
