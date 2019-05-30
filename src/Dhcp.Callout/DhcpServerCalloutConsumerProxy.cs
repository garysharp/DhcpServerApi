using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dhcp.Callout
{
    internal class DhcpServerCalloutConsumerProxy : MarshalByRefObject,
        INewPacket,
        IPacketDrop,
        IPacketSend,
        IAddressDelete,
        IAddressOffer,
        IHandleOptions,
        IDeleteClient
    {
        private Assembly consumerAssembly;
        private string consumerPath;
        private ICalloutConsumer consumerInstance;

        private INewPacket newPacketHandler;
        private IPacketDrop packetDropHandler;
        private IPacketSend packetSendHandler;
        private IAddressDelete addressDeleteHandler;
        private IAddressOffer addressOfferHandler;
        private IHandleOptions handleOptionsHandler;
        private IDeleteClient deleteClientHandler;

        public string ConsumerPath => consumerPath;

        public uint TryInitialize(string ConsumerPath, uint ApiVersion)
        {
            // already initialized?
            if (consumerPath != null)
                return 0xAU; // ERROR_BAD_ENVIRONMENT

            try
            {
                consumerAssembly = Assembly.LoadFile(ConsumerPath);
                consumerPath = ConsumerPath;
                return Initialize(consumerAssembly, ApiVersion);
            }
            catch (FileNotFoundException)
            {
                return 0x2U; // ERROR_FILE_NOT_FOUND
            }
            catch (FileLoadException)
            {
                return 0xBU; // ERROR_BAD_FORMAT
            }
            catch (BadImageFormatException)
            {
                return 0xBU; // ERROR_BAD_FORMAT
            }
        }

        private uint Initialize(Assembly ConsumerAssembly, uint ApiVersion)
        {
            // locate IDhcpServerCalloutConsumer
            var consumer = ConsumerAssembly.GetTypes()
                .Where(t =>
                    typeof(ICalloutConsumer).IsAssignableFrom(t) &&
                    !t.IsValueType &&
                    !t.IsAbstract &&
                    t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) != null)
                .FirstOrDefault();

            if (consumer == null)
                return 0xBU; // assembly does not implement consumer

            consumerInstance = (ICalloutConsumer)Activator.CreateInstance(consumer);

            if (consumerInstance is INewPacket newPacketHandler)
                this.newPacketHandler = newPacketHandler;

            if (consumerInstance is IPacketDrop packetDropHandler)
                this.packetDropHandler = packetDropHandler;

            if (consumerInstance is IPacketSend packetSendHandler)
                this.packetSendHandler = packetSendHandler;

            if (consumerInstance is IAddressDelete addressDeleteHandler)
                this.addressDeleteHandler = addressDeleteHandler;

            if (consumerInstance is IAddressOffer addressOfferHandler)
                this.addressOfferHandler = addressOfferHandler;

            if (consumerInstance is IHandleOptions handleOptionsHandler)
                this.handleOptionsHandler = handleOptionsHandler;

            if (consumerInstance is IDeleteClient deleteClientHandler)
                this.deleteClientHandler = deleteClientHandler;

            // call consumer initialize
            consumerInstance.Initialize((int)ApiVersion);

            return 0x0U; // ERROR_SUCCESS
        }

        public CalloutConsumerSupportFlags GetSupportFlags()
        {
            return
                (newPacketHandler == null ? 0 : CalloutConsumerSupportFlags.NewPacket) |
                (packetDropHandler == null ? 0 : CalloutConsumerSupportFlags.PacketDrop) |
                (packetSendHandler == null ? 0 : CalloutConsumerSupportFlags.PacketSend) |
                (addressDeleteHandler == null ? 0 : CalloutConsumerSupportFlags.AddressDelete) |
                (addressOfferHandler == null ? 0 : CalloutConsumerSupportFlags.AddressOffer) |
                (handleOptionsHandler == null ? 0 : CalloutConsumerSupportFlags.HandleOptions) |
                (deleteClientHandler == null ? 0 : CalloutConsumerSupportFlags.DeleteClient);
        }

        public void Control(CalloutControlCodes ControlCode)
        {
            consumerInstance.Control(ControlCode);
        }

        public void NewPacket(IDhcpServerPacketWritable Packet, DhcpServerIpAddress ServerAddress, ref bool ProcessIt, ref bool StopPropagation)
        {
            newPacketHandler.NewPacket(Packet, ServerAddress, ref ProcessIt, ref StopPropagation);
        }

        public void PacketDrop(IDhcpServerPacket Packet, PacketDropControlCodes ControlCode, DhcpServerIpAddress ServerAddress, ref bool StopPropagation)
        {
            packetDropHandler.PacketDrop(Packet, ControlCode, ServerAddress, ref StopPropagation);
        }

        public void PacketSend(IDhcpServerPacketWritable Packet, DhcpServerIpAddress ServerAddress, ref bool StopPropagation)
        {
            packetSendHandler.PacketSend(Packet, ServerAddress, ref StopPropagation);
        }

        public void AddressDelete(IDhcpServerPacket Packet, AddressDeleteControlCodes ControlCode, DhcpServerIpAddress ServerAddress, DhcpServerIpAddress LeaseAddress, ref bool StopPropagation)
        {
            addressDeleteHandler.AddressDelete(Packet, ControlCode, ServerAddress, LeaseAddress, ref StopPropagation);
        }

        public void AddressOffer(IDhcpServerPacket Packet, OfferAddressControlCodes ControlCode, DhcpServerIpAddress ServerAddress, DhcpServerIpAddress LeaseAddress, OfferAddressTypes AddressType, TimeSpan LeaseTime, ref bool StopPropagation)
        {
            addressOfferHandler.AddressOffer(Packet, ControlCode, ServerAddress, LeaseAddress, AddressType, LeaseTime, ref StopPropagation);
        }

        public void HandleOptions(IDhcpServerPacket Packet, DhcpServerPacketOptions ServerOptions, ref bool StopPropagation)
        {
            handleOptionsHandler.HandleOptions(Packet, ServerOptions, ref StopPropagation);
        }

        public void DeleteClient(DhcpServerIpAddress LeaseAddress, DhcpServerHardwareAddress LeaseHardwareAddress, ref bool StopPropagation)
        {
            deleteClientHandler.DeleteClient(LeaseAddress, LeaseHardwareAddress, ref StopPropagation);
        }

        public void Dispose()
        {
            consumerInstance?.Dispose();
        }
    }
}
