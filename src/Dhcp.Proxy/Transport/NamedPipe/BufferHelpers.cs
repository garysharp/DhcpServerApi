using System;
using System.Collections.Generic;
using System.Text;

namespace Dhcp.Proxy.Transport.NamedPipe
{
    public static class BufferHelpers
    {
        public const int MessageHeaderLength = 8;

        public static void InitializeMessage(ref byte[] buffer, NamedPipeMessageInstruction instruction, int messageId, int dataLength, out int offset)
        {
            var headerOffset = 0;
            EnsureBufferCapacity(ref buffer, MessageHeaderLength + dataLength);
            buffer.WriteNamedPipeHeader(ref headerOffset, instruction, messageId, dataLength);
            offset = headerOffset;
        }

        public static void ReindexRemainder(ref byte[] buffer, ref int offset, int remainderLength, ref byte[] scratchBuffer)
        {
            if (remainderLength > 0)
            {
                // remainder present
                if (offset > remainderLength)
                {
                    Array.Copy(buffer, offset, buffer, 0, remainderLength);
                }
                else
                {
                    EnsureBufferCapacity(ref scratchBuffer, remainderLength);
                    Array.Copy(buffer, offset, scratchBuffer, 0, remainderLength);
                    Array.Copy(scratchBuffer, buffer, remainderLength);
                }
                offset = remainderLength;
            }
            else
            {
                // no remainder
                offset = 0;
            }
        }
        public static void ShrinkBuffer(ref byte[] buffer, int maxLength)
        {
            if (buffer.Length > maxLength)
            {
                buffer = new byte[maxLength];
            }
        }
        public static void ShrinkBufferPreserve(ref byte[] buffer, int bufferLength, int maxLength, ref byte[] scratchBuffer)
        {
            if (buffer.Length > maxLength && bufferLength < maxLength)
            {
                if (bufferLength == 0)
                {
                    // no need to preserve
                    buffer = new byte[maxLength];
                }
                else
                {
                    EnsureBufferCapacity(ref scratchBuffer, bufferLength);
                    Array.Copy(buffer, scratchBuffer, bufferLength);
                    buffer = new byte[maxLength];
                    Array.Copy(scratchBuffer, buffer, bufferLength);
                }
            }
        }

        public static void EnsureBufferCapacity(ref byte[] buffer, int length)
        {
            // check if buffer has capacity
            if (buffer.Length < length)
            {
                // need resize
                buffer = new byte[length + (length % 1024)];
            }
        }

        public static void EnsureBufferCapacityPreserve(ref byte[] buffer, ref byte[] scratchBuffer, int length)
        {
            // check if buffer has capacity
            if (buffer.Length < length)
            {
                // need resize

                // check if secondary can hold copy
                if (scratchBuffer.Length < buffer.Length)
                {
                    // enlarge secondary
                    scratchBuffer = new byte[buffer.Length + (buffer.Length % 1024)];
                }
                var originalSize = buffer.Length;
                Array.Copy(buffer, scratchBuffer, originalSize);
                buffer = new byte[length + (length % 1024)];
                Array.Copy(scratchBuffer, buffer, originalSize);
            }
        }

        public static void WriteNamedPipeHeader(this byte[] buffer, ref int offset, NamedPipeMessageInstruction instruction, int messageId, int dataLength)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (messageId < 0 || messageId > 0x7FFF_FFFF)
                throw new ArgumentOutOfRangeException(nameof(messageId));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length < offset + MessageHeaderLength)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            var instructionAndMessageId =
                ((int)instruction << 28) |
                (messageId & 0x0FFF_FFFF);

            buffer.WriteBigEndian(instructionAndMessageId, ref offset);
            buffer.WriteBigEndian(dataLength, ref offset);
        }

        public static void WriteNamedPipeEmbedded(this byte[] buffer, byte[] value, ref int offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length < offset + 4 + value?.Length)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            var length = value?.Length ?? unchecked((int)0x80000000);

            buffer.WriteBigEndian(length, ref offset);
            if (value != null && value.Length > 0)
            {
                Array.Copy(value, 0, buffer, offset, value.Length);
                offset += value.Length;
            }
        }

        public static ArraySegment<byte>? ReadNamedPipeEmbedded(this byte[] buffer, ref int offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length < offset + 4)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            var length = buffer.ReadBigEndian(ref offset);

            if (length == unchecked((int)0x80000000))
                return null;

            if (buffer.Length < offset + 4 + length)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            var embedOffset = offset;
            offset += length;

            return new ArraySegment<byte>(buffer, embedOffset, length);
        }

        public static (NamedPipeMessageInstruction instruction, int messageId, int dataLength) ReadNamedPipeHeader(this byte[] buffer, ref int offset, ref int length)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (length < MessageHeaderLength)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (buffer.Length < offset + length)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            var instructionAndMessageId = buffer.ReadBigEndian(ref offset);
            var dataLength = buffer.ReadBigEndian(ref offset);

            var instruction = (NamedPipeMessageInstruction)((instructionAndMessageId >> 28) & 0x0F);
            var messageId = instructionAndMessageId & 0x0FFF_FFFF;

            length -= MessageHeaderLength;
            return (instruction, messageId, dataLength);
        }

        public static int ReadBigEndian(this byte[] buffer, ref int offset)
        {
            return
                (int)((buffer[offset++] << 24) & 0xFF00_0000) |
                     ((buffer[offset++] << 16) & 0x00FF_0000) |
                     ((buffer[offset++] << 08) & 0x0000_FF00) |
                     (buffer[offset++] & 0x0000_00FF);
        }
        public static int ReadBigEndian(this byte[] buffer, int offset)
            => ReadBigEndian(buffer, ref offset);

        public static int ReadBigEndian(this byte[] buffer)
            => ReadBigEndian(buffer, 0);

        public static void WriteBigEndian(this byte[] buffer, int value, ref int offset)
        {
            buffer[offset++] = (byte)((value >> 24) & 0xFF);
            buffer[offset++] = (byte)((value >> 16) & 0xFF);
            buffer[offset++] = (byte)((value >> 8) & 0xFF);
            buffer[offset++] = (byte)(value & 0xFF);
        }

        public static void WriteBigEndian(this byte[] buffer, int value, int offset)
            => WriteBigEndian(buffer, value, ref offset);

        public static void WriteBigEndian(this byte[] buffer, int value)
            => WriteBigEndian(buffer, value, 0);

    }
}
