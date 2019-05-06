using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Dhcp
{
    internal static class BitHelper
    {
        public static readonly byte[] EmptyByteArray = new byte[0];

        public const int ByteSize = 1;
        public const int Int16Size = 2;
        public const int Int32Size = 4;
        public const int Int64Size = 8;

        private static readonly string[] hexStringTable = new string[]
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
            "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
            "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
            "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
            "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
        };

        #region Marshaling
        public static T MarshalToStructure<T>(this IntPtr ptr)
            => (T)Marshal.PtrToStructure(ptr, typeof(T));

        public static UnmanagedDisposer<T> StructureToPtr<T>(T structure)
            => new UnmanagedDisposer<T>(structure);

        public class UnmanagedDisposer<T> : IDisposable
        {
            public IntPtr Pointer { get; }

            public UnmanagedDisposer(T structure)
            {
                var size = Marshal.SizeOf(structure);
                Pointer = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.StructureToPtr(structure, Pointer, false);
                }
                catch (Exception)
                {
                    Marshal.FreeHGlobal(Pointer);
                    throw;
                }
            }

            public void Dispose()
            {
                if (Pointer != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(Pointer, typeof(T));
                    Marshal.FreeHGlobal(Pointer);
                }
            }

            public static implicit operator IntPtr(UnmanagedDisposer<T> disposer)
            {
                return disposer.Pointer;
            }
        }
        #endregion

        #region Read

        public static long ReadIntVar(byte[] buffer, int index, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length < index + length)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length > 8)
                throw new ArgumentOutOfRangeException(nameof(length));

            var value = default(long);
            var offset = (length - 1) * 8;

            for (var i = 0; i < length; i++)
            {
                value |= (long)buffer[index++] << offset;
                offset -= 8;
            }

            return value;
        }

        public static long ReadIntVar(IntPtr pointer, int offset, int length)
        {
            // read variable length integer from memory in network-order

            if (pointer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(pointer));
            if (length > 8 || length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == 0)
                return 0;

            var value = Marshal.ReadInt64(pointer, offset);

            // swap bytes
            value = HostToNetworkOrder(value);

            if (length == 8)
                return value;

            // mask
            var mask = unchecked((long)0xFF00000000000000) >> ((length - 1) * 8);

            return value & mask;
        }

        public static long ReadInt64(byte[] buffer, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                return ((long)buffer[index++] << 56) |
                    ((long)buffer[index++] << 48) |
                    ((long)buffer[index++] << 40) |
                    ((long)buffer[index++] << 32) |
                    ((long)buffer[index++] << 24) |
                    ((long)buffer[index++] << 16) |
                    ((long)buffer[index++] << 8) |
                    buffer[index];
            }
            else
            {
                return BitConverter.ToInt64(buffer, index);
            }
        }

        public static ulong ReadUInt64(byte[] buffer, int index) => (ulong)ReadInt64(buffer, index);

        public static int ReadInt32(byte[] buffer, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                return ((int)buffer[index++] << 24) |
                    ((int)buffer[index++] << 16) |
                    ((int)buffer[index++] << 8) |
                    buffer[index];
            }
            else
                return BitConverter.ToInt32(buffer, index);
        }

        public static uint ReadUInt32(byte[] buffer, int index) => (uint)ReadInt32(buffer, index);

        public static short ReadInt16(byte[] buffer, int index)
        {
            if (BitConverter.IsLittleEndian)
                return (short)((buffer[index++] << 8) | buffer[index]);
            else
                return BitConverter.ToInt16(buffer, index);
        }

        public static ushort ReadUInt16(byte[] buffer, int index)
            => (ushort)ReadInt16(buffer, index);

        public static IEnumerable<string> ReadDnsLabelsStrings(byte[] buffer, int index, int length)
        {
            var builder = new StringBuilder();
            for (var i = index; i < length;)
            {
                ReadDnsLabelsString(builder, buffer, i, out i);
                yield return builder.ToString();
                builder.Clear();
            }
        }

        public static string ReadDnsLabelsStrings(byte[] buffer, int index, int length, string seperator)
        {
            var builder = new StringBuilder();
            for (var i = index; i < length;)
            {
                if (builder.Length != 0)
                    builder.Append(seperator);

                ReadDnsLabelsString(builder, buffer, i, out i);
            }
            return builder.ToString();
        }

        public static string ReadDnsLabelsString(byte[] buffer, int index)
        {
            var builder = new StringBuilder();
            ReadDnsLabelsString(builder, buffer, index, out _);
            return builder.ToString();
        }

        public static void ReadDnsLabelsString(StringBuilder builder, byte[] buffer, int index, out int indexEnd)
        {
            var lc = 0;
            for (var i = index; i < buffer.Length; i++)
            {
                var ls = buffer[i];
                // is end?
                if (ls == 0)
                {
                    indexEnd = ++i;
                    return;
                }
                else
                {
                    if (lc++ != 0)
                        builder.Append('.');

                    // is pointer?
                    if ((ls & 0b11000000) != 0)
                    {
                        ReadDnsLabelsUncompressedString(builder, buffer, ls & 0b00111111, out _);
                        indexEnd = ++i;
                        return;
                    }
                    else
                    {
                        builder.Append(Encoding.ASCII.GetString(buffer, ++i, ls));
                        i += ls;
                    }
                }
            }

            throw new ArgumentException("Dns label not correctly terminated");
        }

        public static void ReadDnsLabelsUncompressedString(StringBuilder builder, byte[] buffer, int index, out int indexEnd)
        {
            var lc = 0;
            for (var i = index; i < buffer.Length; i++)
            {
                var ls = buffer[i];

                // is end?
                if (ls == 0)
                {
                    indexEnd = ++i;
                    return;
                }
                else
                {
                    // is pointer?
                    if ((ls & 0b11000000) != 0)
                        throw new ArgumentException("Dns label buffer references a pointer", nameof(buffer));
                    else
                    {
                        if (lc++ != 0)
                            builder.Append('.');

                        builder.Append(Encoding.ASCII.GetString(buffer, ++i, ls));
                        i += ls;
                    }
                }
            }

            throw new ArgumentException("Dns label not correctly terminated");
        }

        public static string ReadAsciiString(byte[] buffer, int index, int maxLength)
        {
            var length = index;
            for (; length < index + maxLength && buffer[length] != 0; length++) ;
            if (length == index)
                return string.Empty;
            else
                return Encoding.ASCII.GetString(buffer, index, length - index);
        }

        public static string ReadUtf8String(byte[] buffer, int index, int maxLength)
        {
            var length = index;
            for (; buffer[length] != 0 && length < index + maxLength; length++) ;
            if (length == index)
                return string.Empty;
            else
                return Encoding.UTF8.GetString(buffer, index, length - index);
        }

        public static DhcpServerIpAddress ReadIpAddress(byte[] buffer, int index) => DhcpServerIpAddress.FromNative(ReadInt32(buffer, index));

        public static string ReadHexString(ulong buffer, int index, int length)
        {
            if (index + length > 8)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (length == 0)
                return string.Empty;

            var builder = new StringBuilder(length * 2);
            var end = index + length;
            for (var i = index; i < end; i++)
            {
                var b = (byte)(buffer >> (56 - (i * 8)));

                builder.AppendHex(b);
            }

            return builder.ToString();
        }

        public static string ReadHexString(ulong buffer, int index, int length, char seperator)
        {
            if (index + length > 8)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (length == 0)
                return string.Empty;

            var builder = new StringBuilder(length * 2);
            var end = index + length;
            for (var i = index; i < end; i++)
            {
                if (i != index)
                    builder.Append(seperator);

                var b = (byte)(buffer >> (56 - (i * 8)));

                builder.AppendHex(b);
            }

            return builder.ToString();
        }

        public static string ReadHexString(ulong buffer1, ulong buffer2, int index, int length)
        {
            if (index + length > 16)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (length == 0)
                return string.Empty;

            var builder = new StringBuilder(length * 2);
            var end = index + length;
            for (var i = index; i < end; i++)
            {
                byte b;
                if (i < 8)
                    b = (byte)(buffer1 >> (56 - (i * 8)));
                else
                    b = (byte)(buffer2 >> (56 - ((i - 8) * 8)));

                builder.AppendHex(b);
            }

            return builder.ToString();
        }

        public static string ReadHexString(ulong buffer1, ulong buffer2, int index, int length, char seperator)
        {
            if (index + length > 16)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (length == 0)
                return string.Empty;

            var builder = new StringBuilder(length * 2 + (length - 1));
            var end = index + length;
            for (var i = index; i < end; i++)
            {
                if (i != index)
                    builder.Append(seperator);

                byte b;
                if (i < 8)
                    b = (byte)(buffer1 >> (56 - (i * 8)));
                else
                    b = (byte)(buffer2 >> (56 - ((i - 8) * 8)));

                builder.AppendHex(b);
            }

            return builder.ToString();
        }

        public static string ReadHexString(byte[] buffer, int index, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length < index + length)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (length == 0)
                return string.Empty;

            var builder = new StringBuilder(length * 2);
            var end = index + length;
            for (var i = index; i < end; i++)
                builder.AppendHex(buffer[i]);

            return builder.ToString();
        }

        public static string ReadHexString(byte[] buffer, int index, int length, char seperator)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length < index + length)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (length == 0)
                return string.Empty;

            var builder = new StringBuilder((length * 2) + length - 1);
            var end = index + length;
            for (var i = index; i < end; i++)
            {
                if (i != index)
                    builder.Append(seperator);

                builder.AppendHex(buffer[i]);
            }

            return builder.ToString();
        }

        public static string ReadHexString(byte buffer) => hexStringTable[buffer];

        public static string ReadHexString(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return ReadHexString(buffer, 0, buffer.Length);
        }

        public static string ReadHexString(byte[] buffer, char seperator)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return ReadHexString(buffer, 0, buffer.Length, seperator);
        }

        public static void AppendHex(this StringBuilder builder, byte buffer) => builder.Append(hexStringTable[buffer]);

        public static void AppendDnsLabelsString(this StringBuilder builder, byte[] buffer, int index)
            => ReadDnsLabelsString(builder, buffer, index, out _);

        public static void AppendDnsLabelsStrings(this StringBuilder builder, byte[] buffer, int index, int length, string seperator)
        {
            for (var i = index; i < length;)
            {
                if (builder.Length != 0)
                    builder.Append(seperator);

                ReadDnsLabelsString(builder, buffer, i, out i);
            }
        }

        #endregion

        #region Write

        public static void Write(byte[] buffer, int index, byte value) => buffer[index] = value;

        public static void Write(byte[] buffer, int index, byte[] value) => Array.Copy(value, 0, buffer, index, value.Length);

        public static void Write(byte[] buffer, int index, byte[] value, int valueIndex, int valueLength)
            => Array.Copy(value, valueIndex, buffer, index, valueLength);

        public static void Write(byte[] buffer, int index, ulong value, int valueIndex, int valueLength)
        {
            if (valueIndex + valueLength > Int64Size)
                throw new ArgumentOutOfRangeException(nameof(valueLength));
            if (valueLength > buffer.Length - index || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (valueLength == 0)
                return;

            var end = valueIndex + valueLength;
            for (var i = valueIndex; i < end; i++)
                buffer[index++] = (byte)(value >> (56 - (i * 8)));
        }

        public static void Write(byte[] buffer, int index, ulong value1, ulong value2, int valueIndex, int valueLength)
        {
            if (valueIndex + valueLength > Int64Size * 2)
                throw new ArgumentOutOfRangeException(nameof(valueLength));
            if (valueLength > buffer.Length - index || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (valueLength == 0)
                return;


            var end = valueIndex + valueLength;
            for (var i = valueIndex; i < end; i++)
            {
                if (i < 8)
                    buffer[index++] = (byte)(value1 >> (56 - (i * 8)));
                else
                    buffer[index++] = (byte)(value2 >> (56 - ((i - 8) * 8)));
            }
        }

        public static void Write(byte[] buffer, int index, ulong value)
        {
            buffer[index++] = (byte)(value >> 56);
            buffer[index++] = (byte)(value >> 48);
            buffer[index++] = (byte)(value >> 40);
            buffer[index++] = (byte)(value >> 32);

            buffer[index++] = (byte)(value >> 24);
            buffer[index++] = (byte)(value >> 16);
            buffer[index++] = (byte)(value >> 8);
            buffer[index] = (byte)(value);
        }

        public static void Write(byte[] buffer, int index, long value) => Write(buffer, index, (ulong)value);

        public static void Write(byte[] buffer, int index, uint value)
        {
            buffer[index++] = (byte)(value >> 24);
            buffer[index++] = (byte)(value >> 16);
            buffer[index++] = (byte)(value >> 8);
            buffer[index] = (byte)(value);
        }

        public static void Write(byte[] buffer, int index, int value) => Write(buffer, index, (uint)value);

        public static void Write(byte[] buffer, int index, ushort value)
        {
            buffer[index++] = (byte)(value >> 8);
            buffer[index] = (byte)(value);
        }

        public static void Write(byte[] buffer, int index, short value) => Write(buffer, index, (ushort)value);

        public static void Write(byte[] buffer, int index, DhcpServerIpAddress value) => Write(buffer, index, value.Native);

        public static void WriteAscii(byte[] buffer, int index, string value, int length)
        {
            if (value != null && length > 0)
            {
                var written = Encoding.ASCII.GetBytes(value, 0, value.Length > length ? length : value.Length, buffer, index);
                Array.Clear(buffer, index + written, length - written);
            }
            else
            {
                Array.Clear(buffer, index, length);
            }
        }

        public static void WriteAscii(byte[] buffer, int index, string value)
        {
            if (value != null && value.Length > 0)
                WriteAscii(buffer, index, value, value.Length);
        }

        #endregion

        #region Byte Reordering
        public static uint NetworkToHostOrder(uint bits)
        {
            if (BitConverter.IsLittleEndian)
            {
                // Swap
                return ((bits >> 24) & 0x0000_00FF) |
                       ((bits >> 08) & 0x0000_FF00) |
                       ((bits << 08) & 0x00FF_0000) |
                       ((bits << 24) & 0xFF00_0000);
            }
            else
            {
                return bits;
            }
        }

        public static int NetworkToHostOrder(int bits) => (int)NetworkToHostOrder((uint)bits);

        public static ulong NetworkToHostOrder(ulong bits)
        {
            if (BitConverter.IsLittleEndian)
            {
                // Swap
                return ((bits >> 56) & 0x00000000000000FF) |
                       ((bits >> 40) & 0x000000000000FF00) |
                       ((bits >> 24) & 0x0000000000FF0000) |
                       ((bits >> 08) & 0x00000000FF000000) |
                       ((bits << 08) & 0x000000FF00000000) |
                       ((bits << 24) & 0x0000FF0000000000) |
                       ((bits << 40) & 0x00FF000000000000) |
                       ((bits << 56) & 0xFF00000000000000);
            }
            else
            {
                return bits;
            }
        }

        public static long NetworkToHostOrder(long bits) => (long)NetworkToHostOrder((ulong)bits);
        public static uint HostToNetworkOrder(uint bits) => NetworkToHostOrder(bits);
        public static int HostToNetworkOrder(int bits) => (int)NetworkToHostOrder((uint)bits);
        public static ulong HostToNetworkOrder(ulong bits) => NetworkToHostOrder(bits);
        public static long HostToNetworkOrder(long bits) => (long)NetworkToHostOrder((ulong)bits);
        #endregion

        public static void UIntToString(char[] chars, ref int offset, int value)
        {
            do
            {
                value = Math.DivRem(value, 10, out var r);
                chars[--offset] = (char)('0' + r);
            } while (value != 0);
        }

        public static string IpAddressToString(uint nativeIpAddress)
        {
            var buffer = new char[15];
            var offset = 15;

            UIntToString(buffer, ref offset, (int)(nativeIpAddress & 0xFF));
            buffer[--offset] = '.';
            UIntToString(buffer, ref offset, (int)((nativeIpAddress >> 8) & 0xFF));
            buffer[--offset] = '.';
            UIntToString(buffer, ref offset, (int)((nativeIpAddress >> 16) & 0xFF));
            buffer[--offset] = '.';
            UIntToString(buffer, ref offset, (int)((nativeIpAddress >> 24) & 0xFF));

            return new string(buffer, offset, 15 - offset);
        }

        public static uint StringToIpAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));
            if (ipAddress.Length > 15 || ipAddress.Length < 7)
                throw new ArgumentOutOfRangeException(nameof(ipAddress));

            var ip = 0U;
            var os = 24;
            var oi = 0;
            var o = 0;

            for (var i = 0; i < ipAddress.Length; i++)
            {
                var c = ipAddress[i];

                if (c == '.')
                {
                    if ((i - oi) < 1 || (i - oi) > 3 || os <= 0 || o > 255)
                        throw new ArgumentOutOfRangeException(nameof(ipAddress));

                    ip |= (uint)(o << os);
                    os -= 8;
                    o = 0;
                    oi = i + 1;
                }
                else
                {
                    if (c < '0' || c > '9')
                        throw new ArgumentOutOfRangeException(nameof(ipAddress));

                    o = (o * 10) + (c - '0');
                }
            }
            if ((ipAddress.Length - oi) < 1 || (ipAddress.Length - oi) > 3 || os != 0 || o > 255)
                throw new ArgumentOutOfRangeException(nameof(ipAddress));

            ip |= (uint)o;

            return ip;
        }

        public static int HighSignificantBits(uint value)
        {
            var sb = 0;

            if ((value & 0xFFFF0000) == 0xFFFF0000)
                sb = 16;
            else
                value >>= 16;

            if ((value & 0xFF00) == 0xFF00)
                sb += 8;
            else
                value >>= 8;

            if ((value & 0xF0) == 0xF0)
                sb += 4;
            else
                value >>= 4;

            if ((value & 0xC) == 0xC)
                sb += 2;
            else
                value >>= 2;

            if ((value & 2) == 2)
                sb += 1;
            else
                value >>= 1;

            if ((value & 1) == 1)
                sb += 1;

            return sb;
        }

        public static int HighInsignificantBits(uint value) => HighSignificantBits(~value);

        public static int LowInsignificantBits(uint value)
        {
            var sb = 0;

            if ((value & 0xFFFF) == 0)
            {
                sb = 16;
                value >>= 16;
            }

            if ((value & 0xFF) == 0)
            {
                sb += 8;
                value >>= 8;
            }

            if ((value & 0x0F) == 0)
            {
                sb += 4;
                value >>= 4;
            }

            if ((value & 0x03) == 0)
            {
                sb += 2;
                value >>= 2;
            }

            if ((value & 1) == 0)
                sb += 1;

            return sb;
        }

        public static int LowSignificantBits(uint value) => LowInsignificantBits(~value);

        public static void DebugDump(IntPtr pointer, int length)
        {
            var buffer = new byte[length];
            Marshal.Copy(pointer, buffer, 0, length);
            var builder = new StringBuilder(length * 3);

            for (var i = 0; i < buffer.Length;)
            {
                for (var x = 0; x < 16 && i < buffer.Length; x++)
                {
                    builder.AppendHex(buffer[i++]);
                    builder.Append(' ');
                }
                builder.AppendLine();
            }

            System.Diagnostics.Debug.WriteLine(builder.ToString());
        }
    }
}
