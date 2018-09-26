using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LightDirectory.Asn1.Serialization
{
    public interface IAsn1Serializer
    {
        //Task<Asn1Object> Read(Stream s, CancellationToken cancellationToken);
        //Task<int> Write(Stream s, Asn1Object item, CancellationToken cancellationToken);
        Task<(Asn1Object, int)> Read(Stream stream, CancellationToken cancellationToken);
        Task<int> Write(Asn1Object item, Stream stream, CancellationToken cancellationToken);
    }
}