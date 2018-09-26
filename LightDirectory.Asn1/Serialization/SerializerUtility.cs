using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Serialization;
using LightDirectory.Asn1.Suport;

namespace LightDirectory.Asn1.Universal.Serializers
{
    public class SerializerUtility
    {
        public static async Task<(Identifier id, int bytesRead)> ReadIdentifier(Stream stream,
            CancellationToken cancellationToken)
        {
            var bytesRead = 0;
            var r = stream.ReadByte();
            bytesRead++;
            if (r < 0)
            {
                throw new EndOfStreamException("BERDecoder: decode: EOF in Identifier");
            }

            var tagClass = r >> 6;
            var constructed = (r & 0x20) != 0;
            if ((r & 0x1F) == 0x1F)

                // if true, its a multiple octet identifier.
            {
                var (t,b) =  await DecodeTagNumber(stream, cancellationToken);
                return (new Identifier((TagClass) tagClass, constructed, t), bytesRead + b);
            }
            else
            {
                return (new Identifier((TagClass) tagClass, constructed, r & 0x1F), bytesRead);
            }
        }
        private static async Task<(int tag, int bytesRead)> DecodeTagNumber(Stream stream, CancellationToken cancellationToken)
        {
            var n = 0;
            var bytesRead = 0;
            while (true)
            {
                var bytes = await stream.ReadBERBytesAsync(1, cancellationToken);
                var r = bytes[0];
                bytesRead++;

                n = (n << 7) + (r & 0x7F);
                if ((r & 0x80) == 0)
                {
                    break;
                }
            }

            return (n, bytesRead);
        }


        public static async Task<int> WriteIntegerValue(long value, Stream stream, CancellationToken cancellationToken)
        {
            var (len, data) = GetMinimumBigEndianRepresentation(value);
            var all = new byte[] {len}.Concat(data).ToArray();
            await stream.WriteBERBytes(all, cancellationToken: cancellationToken);
            return all.Length;
        }

        private static (byte length, byte[] data) GetMinimumBigEndianRepresentation(long value)
        {
            
            if (IsInt16(value))
            {
                var data = BitConverter.GetBytes((Int16) value);
                if(BitConverter.IsLittleEndian){Array.Reverse(data);}

                return (sizeof(Int16), data);
            }
            else if (IsInt32(value))
            {
                var data = BitConverter.GetBytes((Int32) value);
                if(BitConverter.IsLittleEndian){Array.Reverse(data);}

                return (sizeof(Int32), data);
            }
            else
            {
                var data = BitConverter.GetBytes(value);
                if(BitConverter.IsLittleEndian){Array.Reverse(data);}

                return (sizeof(Int64), data);
            }
        }


        private static bool IsInt16(long value) => short.MinValue <= value && value <= short.MaxValue;
        private static bool IsInt32(long value) => int.MinValue <= value && value <= int.MaxValue;


        public static async Task<int> WriteIdentifier(Identifier identifier, Stream stream, CancellationToken cancellationToken)
        {
            var c =(int) identifier.TagClass;
            var t = identifier.Tag;
            var firstOctet = (byte)((c << 6) | (identifier.Constructed ? 0x20 : 0));

            if (t < 30)
            {
                /* single octet */
                stream.WriteByte((byte)(firstOctet | t));
                return 1;
            }
            else
            {
                /* multiple octet */
                stream.WriteByte((byte)(firstOctet | 0x1F));
                int value = t;
                var octets = new byte[5];
                int n;
                for (n = 0; value != 0; n++)
                {
                    octets[n] = (byte)(value & 0x7F);
                    value = value >> 7;
                }

                var reversed = octets.Reverse().Cast<byte>().ToArray();
                await stream.WriteBERBytes(reversed, cancellationToken: cancellationToken);
                var len = octets.Length;
                return len + 1;
            }
        }

        public static async Task<int> WriteLength(int length, Stream stream, CancellationToken cancellationToken)
        {
            if (length < 128)
            {
                await stream.WriteBERBytes(new byte[] {(byte) length}, cancellationToken: cancellationToken);
                return 1;
            }
            else
            {
                var data = BitConverter.GetBytes(length);
                if(BitConverter.IsLittleEndian){Array.Reverse(data);}
                var lengthLength = (byte)(0x80 | (byte) data.Length);
                var bytes = new byte[] {lengthLength}.Concat(data).ToArray();
                await stream.WriteBERBytes(bytes, cancellationToken: cancellationToken);
                return bytes.Length;
            }
        }

        public static async Task<(MemoryStream ms, int innerLength)> WriteItemsToBufferedStream(
            IAsn1Serializer asn1Serializer, IEnumerable<Asn1Object> items, CancellationToken cancellationToken)
        {
            var ms = new MemoryStream();
            var sequenceItemsBytes = 0;
                
            foreach (var part in items)
            {
                sequenceItemsBytes += await asn1Serializer.Write(part, ms, cancellationToken);
            }

            await ms.FlushAsync(cancellationToken);
            ms.Position = 0;
            return (ms, sequenceItemsBytes);
        }
        public static async Task<(long length, int bytesRead)> ReadLength(Stream stream, CancellationToken cancellationToken)
        {
            var bytesRead = 0;
            var r = await stream.ReadBERByte( cancellationToken);
            bytesRead++;
            var length = 0;
            if (r == 0x80)
            {
                length = -1;
            }
            else if (r < 0x80)
            {
                length = r;
            }
            else
            {
                var lengthLength = 0x7F & r;
                if (lengthLength > sizeof(long))
                {
                    throw new DeserializationException($"Unsupported numeric data type: the stream specified an integer {lengthLength} bytes long but this platform only supports up to {sizeof(long)}");
                }
                var data = await stream.ReadBERBytesAsync(lengthLength, cancellationToken);
                data = Enumerable.Repeat((byte) 0x00, (sizeof(int)) - data.Length).Concat(data).ToArray();
                return (BitConverter.ToInt64(data), lengthLength+1);
            }

            return (length, bytesRead);
        }
    }
}
