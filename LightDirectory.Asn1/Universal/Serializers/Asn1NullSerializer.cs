using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Serialization;

namespace LightDirectory.Asn1.Universal.Serializers
{
    public class Asn1NullSerializer : Asn1ObjectSerializer<Asn1Null>
    {
        public Asn1NullSerializer() : base(TagClass.Universal, (int) UniversalTags.Null)
        {
        }

        public override Task<(Asn1Object, int)> Read(IAsn1Serializer serializer, Identifier id, int length,
            Stream stream,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(((Asn1Object) new Asn1Null(),0));
        }

        public override async Task<int> Write(IAsn1Serializer asn1Serializer, Asn1Null item, Stream stream, CancellationToken cancellationToken)
        {

            return await SerializerUtility.WriteIdentifier(item.Id, stream, cancellationToken);


        }
    }
}