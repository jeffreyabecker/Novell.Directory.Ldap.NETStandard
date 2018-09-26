using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Serialization;
using LightDirectory.Asn1.Suport;

namespace LightDirectory.Asn1.Universal.Serializers
{
    public class Asn1OctetStringSerializer : Asn1ObjectSerializer<Asn1OctetString>
    {
        public Asn1OctetStringSerializer() : base(TagClass.Universal, (int) UniversalTags.OctetString)
        {
        }

        public override async Task<(Asn1Object, int)> Read(IAsn1Serializer serializer, Identifier id, int length,
            Stream stream, CancellationToken cancellationToken)
        {
            var val = await stream.ReadBERBytesAsync(length, cancellationToken);
            return (new Asn1OctetString(val), val.Length);
        }

        public override async Task<int> Write(IAsn1Serializer asn1Serializer, Asn1OctetString item, Stream stream, CancellationToken cancellationToken)
        {
            var identBytes = await SerializerUtility.WriteIdentifier(item.Id, stream, cancellationToken);
            var lenBytes = await SerializerUtility.WriteLength(item.Data.Length, stream, cancellationToken);
             await stream.WriteBERBytes(item.Data, cancellationToken: cancellationToken);
            return identBytes + lenBytes+ item.Data.Length;

        }
    }
}