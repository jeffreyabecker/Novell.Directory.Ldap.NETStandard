using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LightDirectory.Asn1.Suport
{
    internal static class StreamExtensions
    {
        public static async Task WriteBERBytes(this Stream s, byte[] buffer, int offset = 0, int count = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken = cancellationToken == default(CancellationToken)
                ? CancellationToken.None
                : cancellationToken;
            count = count == -1 ? count = buffer.Length - offset : count;
            await s.WriteAsync(buffer, offset, count, cancellationToken);
        }
        public static async Task<byte> ReadBERByte(this Stream s, CancellationToken cancellationToken)
        {
            var buff = new byte[1];
            var bytesRead = await s.ReadAsync(buff, 0, 1, cancellationToken);
            if(bytesRead != 1) throw new EndOfStreamException("Premature end of stream");
            return buff[0];
        }

        public static async Task<long> ReadBERInteger(this Stream s, int len, CancellationToken cancellationToken)
        {
            if (len == 0) return 0;
            long l = 0;
            var octets = await s.ReadBERBytesAsync(len, cancellationToken);
            

            if ((octets[0] & 0b10000000) != 0)
            {
                // check for negative number
                l = -1;
            }

            l = (l << 8) | octets[0];

            for (var i = 1; i < len; i++)
            {

                l = (l << 8) | octets[i];
            }

            return l;
        }
        private const int BufferLength = 2048;

        public static async Task<byte[]> ReadBERBytesAsync(this Stream s, int len, CancellationToken cancellationToken)
        {
            var bytesRemaining = len;
            byte[] buffer = new byte[BufferLength]; // read in chunks of 2KB
            using (var stream = new MemoryStream())
            {
                do
                {
                    var bytesToRead = bytesRemaining > BufferLength ? BufferLength : bytesRemaining;
                    var bytesRead = await s.ReadAsync(buffer, 0, bytesToRead, cancellationToken);
                    if (bytesToRead != bytesRead)
                    {
                        throw new EndOfStreamException(
                            $"Premature end of stream. Read {bytesRead} of {len} expected");
                    }
                    stream.Write(buffer, 0, bytesRead);
                    bytesRemaining -= bytesRead;
                } while (bytesRemaining > 0);

                return stream.ToArray();
            }
        }


    }
}
