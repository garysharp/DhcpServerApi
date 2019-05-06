using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Dhcp
{
    internal class DhcpServerPacket : MarshalByRefObject, IDhcpServerPacket, IDhcpServerPacketRaw
    {
        internal const int maxBufferLength = 4000;
        protected readonly internal IntPtr pointer;
        protected readonly internal int size;
        protected internal int length;
        protected internal byte[] buffer;

        internal DhcpServerPacket(IntPtr pointer, int size)
        {
            // lazy-marshal the buffer as in most cases this will never be used
            this.pointer = pointer;
            this.size = size;
        }

        /// <inheritdoc />
        public byte[] Buffer
        {
            get
            {
                if (buffer == null)
                {
                    buffer = new byte[size];
                    Marshal.Copy(pointer, buffer, 0, size);
                }
                return buffer;
            }
        }

        /// <inheritdoc />
        public int GetLength()
        {
            if (size == 0)
                return 0;

            if (length == 0)
            {
                if (TryGetOptionIndex(OptionTags.End, out length))
                    length++;
                else
                    length = size;
            }

            return length;
        }

        #region Packet Fields

        protected const int OpOffset = 0;
        /// <inheritdoc />
        public MessageTypes MessageType => (MessageTypes)Buffer[OpOffset];

        protected const int HtypeOffset = OpOffset + 1; // 1
        /// <inheritdoc />
        public DhcpServerHardwareType HardwareAddressType => (DhcpServerHardwareType)Buffer[HtypeOffset];

        protected const int HlenOffset = HtypeOffset + 1; // 2
        /// <inheritdoc />
        public byte HardwareAddressLength => Buffer[HlenOffset];

        protected const int HopsOffset = HlenOffset + 1; // 3
        /// <inheritdoc />
        public byte GatewayHops => Buffer[HopsOffset];

        protected const int XidOffset = HopsOffset + 1; // 4
        /// <inheritdoc />
        public int TransactionId => BitHelper.ReadInt32(Buffer, XidOffset);

        protected const int SecsOffset = XidOffset + 4; // 8
        /// <inheritdoc />
        public ushort SecondsElapsed => BitHelper.ReadUInt16(Buffer, SecsOffset);

        protected const int FlagsOffset = SecsOffset + 2; // 10
        /// <inheritdoc />
        public PacketFlags Flags => (PacketFlags)BitHelper.ReadInt16(Buffer, FlagsOffset);

        protected const int CiaddrOffset = FlagsOffset + 2; // 12
        /// <inheritdoc />
        public DhcpServerIpAddress ClientIpAddress => BitHelper.ReadIpAddress(Buffer, CiaddrOffset);

        protected const int YiaddrOffset = CiaddrOffset + 4; // 16
        /// <inheritdoc />
        public DhcpServerIpAddress YourIpAddress => BitHelper.ReadIpAddress(Buffer, YiaddrOffset);

        protected const int SiaddrOffset = YiaddrOffset + 4; // 20
        /// <inheritdoc />
        public DhcpServerIpAddress NextServerIpAddress => BitHelper.ReadIpAddress(Buffer, SiaddrOffset);

        protected const int GiaddrOffset = SiaddrOffset + 4; // 24
        /// <inheritdoc />
        public DhcpServerIpAddress RelayAgentIpAddress => BitHelper.ReadIpAddress(Buffer, SiaddrOffset);

        protected const int ChaddrOffset = GiaddrOffset + 4; // 28
        /// <inheritdoc />
        public DhcpServerHardwareAddress ClientHardwareAddress 
            => DhcpServerHardwareAddress.FromNative(HardwareAddressType, Buffer, ChaddrOffset, HardwareAddressLength);

        protected const int SnameOffset = ChaddrOffset + 16; // 44
        /// <inheritdoc />
        public string ServerHostName => BitHelper.ReadAsciiString(Buffer, SnameOffset, 64);

        protected const int FileOffset = SnameOffset + 64; // 108
        /// <inheritdoc />
        public string FileName => BitHelper.ReadAsciiString(Buffer, FileOffset, 128);

        protected const int MagicCookieOffset = FileOffset + 128; // 236
        public int OptionsMagicCookie =>
                BitHelper.ReadInt32(Buffer, MagicCookieOffset); // Should be [99, 130 (-126), 83, 99] or 0x63825363

        #endregion

        public PacketMessageTypes DhcpMessageType
        {
            get
            {
                if (TryGetOption(OptionTags.DhcpMessageType, out var option) && option.DataLength == 1)
                    return (PacketMessageTypes)option.DataAsByte();

                return PacketMessageTypes.Unknown;
            }
        }

        protected const int OptionsOffset = MagicCookieOffset + 4;
        public ReadOnlyCollection<DhcpServerPacketOption> Options => DhcpServerPacketOption.ParseAll(Buffer, OptionsOffset, GetLength());
        public bool TryGetOption(OptionTags tag, out DhcpServerPacketOption option)
        {
            if (TryGetOptionIndex(tag, out var optionIndex))
            {
                option = DhcpServerPacketOption.Parse(Buffer, ref optionIndex);
                return true;
            }

            option = default;
            return false;
        }
        public bool TryGetOption(byte tag, out DhcpServerPacketOption option) => TryGetOption((OptionTags)tag, out option);

        protected bool TryGetOptionIndex(OptionTags tag, out int optionIndex)
        {
            var buffer = this.buffer ?? Buffer;

            for (var offset = OptionsOffset; offset < buffer.Length;)
            {
                var optionTag = (OptionTags)buffer[offset];

                if (optionTag == tag)
                {
                    optionIndex = offset;
                    return true;
                }

                if (optionTag == OptionTags.End)
                    break;

                switch (optionTag)
                {
                    case OptionTags.Pad:
                    case OptionTags.End:
                        // 0-byte fixed length
                        offset++;
                        break;
                    case OptionTags.SubnetMask:
                    case OptionTags.TimeOffset:
                        // 4-byte fixed length
                        offset += 5;
                        break;
                    default:
                        // variable length
                        offset++;
                        offset += buffer[offset] + 1;
                        break;
                }
            }

            optionIndex = -1;
            return false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("Message Type (op): ").AppendLine(MessageType.ToString());
            sb.Append("Hardware Address Type (htype): ").AppendLine(HardwareAddressType.ToString());
            sb.Append("Hardware Address Length (hlen): ").AppendLine(HardwareAddressLength.ToString());
            sb.Append("Gateway Hops (hops): ").AppendLine(GatewayHops.ToString());
            sb.Append("Transaction Id (xid): ").AppendLine(TransactionId.ToString());
            sb.Append("Seconds Elapsed (secs): ").AppendLine(SecondsElapsed.ToString());
            sb.Append("Flags (flags): ").AppendLine(Convert.ToString((int)Flags, 2));
            foreach (PacketFlags flag in Enum.GetValues(typeof(PacketFlags)))
            {
                sb.Append("    ");
                var mask = Convert.ToString((short)flag, 2).Replace('0', '.');
                if (Flags.HasFlag(flag))
                    sb.Append(mask).Append(": ").AppendLine(flag.ToString());
                else
                    sb.Append(mask.Replace('1', '0')).Append(": No ").AppendLine(flag.ToString());
            }
            sb.Append("Client IP Address (ciaddr): ").AppendLine(ClientIpAddress.ToString());
            sb.Append("Your IP Address (yiaddr): ").AppendLine(YourIpAddress.ToString());
            sb.Append("Next Server IP Address (siaddr): ").AppendLine(NextServerIpAddress.ToString());
            sb.Append("Relay Agent IP Address (giaddr): ").AppendLine(RelayAgentIpAddress.ToString());
            sb.Append("Client Hardware Address (chaddr): ").AppendLine(ClientHardwareAddress.ToString());
            sb.Append("Server Host Name (sname): ").AppendLine(ServerHostName);
            sb.Append("File Name (file): ").AppendLine(FileName);
            sb.Append("Options Magic Cookie: ").AppendLine(OptionsMagicCookie.ToString());

            var options = Options.ToList();
            if (options.Count == 0)
                sb.Append("Options: None");
            else
            {
                sb.Append("Options:");
                foreach (var option in options)
                {
                    sb.AppendLine();
                    sb.Append("    ");
                    sb.Append(((byte)option.Tag).ToString("000"));
                    sb.Append(" ");
                    sb.Append(option.Tag.ToString());
                    sb.Append(" [");
                    sb.Append(option.Type.ToString());
                    sb.AppendLine("]");
                    sb.Append("        ");
                    option.DataAsFormatted(sb);
                }
            }

            return sb.ToString();
        }
    }
}
