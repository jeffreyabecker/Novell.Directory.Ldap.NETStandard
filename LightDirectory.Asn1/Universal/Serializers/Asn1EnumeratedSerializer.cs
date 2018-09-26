using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Serialization;
using LightDirectory.Asn1.Suport;

namespace LightDirectory.Asn1.Universal.Serializers
{
    public class Asn1EnumeratedSerializer : Asn1ObjectSerializer<Asn1Enumerated>
    {
        public Asn1EnumeratedSerializer() : base(TagClass.Universal, (int) UniversalTags.Enumerated)
        {
        }

        public override async Task<(Asn1Object, int)> Read(IAsn1Serializer serializer, Identifier id, int length,
            Stream stream, CancellationToken cancellationToken)
        {
            var val = await stream.ReadBERInteger(length, cancellationToken);
            return (new Asn1Enumerated(val), length);
        }

        public override async Task<int> Write(IAsn1Serializer asn1Serializer, Asn1Enumerated item, Stream stream, CancellationToken cancellationToken)
        {

            var identBytes = await SerializerUtility.WriteIdentifier(item.Id, stream, cancellationToken);
            var bytes = await SerializerUtility.WriteIntegerValue(item.Value, stream, cancellationToken);
            return identBytes + bytes;

        }
    }
}