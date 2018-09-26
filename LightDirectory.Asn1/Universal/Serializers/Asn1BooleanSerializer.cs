using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Serialization;
using LightDirectory.Asn1.Suport;

namespace LightDirectory.Asn1.Universal.Serializers
{
    public class Asn1BooleanSerializer : Asn1ObjectSerializer<Asn1Boolean>
    {
        public Asn1BooleanSerializer() : base(TagClass.Universal, (int) UniversalTags.Boolean){}
        public override async Task<(Asn1Object, int)> Read(IAsn1Serializer serializer, Identifier id, int length,
            Stream stream, CancellationToken cancellationToken)
        {
            var b = await stream.ReadBERByte(cancellationToken);
            var val =  (b != 0x00);
            return (new Asn1Boolean(val),1);
        }

        public override async Task<int> Write(IAsn1Serializer serializer, Asn1Boolean item, Stream stream,
            CancellationToken cancellationToken)
        {
            var boolean = (Asn1Boolean)item;
            var identBytes = await SerializerUtility.WriteIdentifier(item.Id, stream, cancellationToken);
            var buffer = new byte[] {1, boolean.Value?(byte)0xff :(byte) 0x00};
            await stream.WriteBERBytes(buffer, cancellationToken: cancellationToken);
            return identBytes + buffer.Length;

        }
    }
}
