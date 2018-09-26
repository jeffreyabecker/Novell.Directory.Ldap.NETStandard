using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Serialization;
using LightDirectory.Asn1.Suport;

namespace LightDirectory.Asn1.Universal.Serializers
{
    public class Asn1IntegerSerializer : Asn1ObjectSerializer<Asn1Integer>
    {
        public Asn1IntegerSerializer() : base(TagClass.Universal, (int) UniversalTags.Integer)
        {
        }

        public override async Task<(Asn1Object, int)> Read(IAsn1Serializer serializer, Identifier id, int length,
            Stream stream, CancellationToken cancellationToken)
        {
            var val = await stream.ReadBERInteger(length, cancellationToken);
            return (new Asn1Integer(val), length);
        }

        public override async Task<int> Write(IAsn1Serializer asn1Serializer, Asn1Integer item, Stream stream, CancellationToken cancellationToken)
        {
            var integer = (Asn1Integer)item;
            var identBytes = await SerializerUtility.WriteIdentifier(item.Id, stream, cancellationToken);
            var bytes = await SerializerUtility.WriteIntegerValue(integer.Value, stream, cancellationToken);
            return identBytes + bytes;

        }
    }
}