using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Serialization;

namespace LightDirectory.Asn1.Universal.Serializers
{
    public class Asn1SequenceSerializer : Asn1ObjectSerializer<Asn1Sequence>
    {
        public Asn1SequenceSerializer() : base(TagClass.Universal, (int) UniversalTags.Sequence){}
        public override async Task<(Asn1Object, int)> Read(IAsn1Serializer serializer, Identifier id, int length,
            Stream stream, CancellationToken cancellationToken)
        {
            var totalLength = 0;
            var lst = new List<Asn1Object>();
            while (length > 0)
            {
                var (obj, bytesRead) = await serializer.Read(stream, cancellationToken);
                lst.Add(obj);
                length -= bytesRead;
                totalLength += bytesRead;
            }
            return (new Asn1Sequence(lst), totalLength);
        }

        public override async Task<int> Write(IAsn1Serializer asn1Serializer, Asn1Sequence item, Stream stream, CancellationToken cancellationToken)
        {
            var (ms, sequenceItemsBytes) =
                await SerializerUtility.WriteItemsToBufferedStream(asn1Serializer, item.Items, cancellationToken);
            var identBytes = await SerializerUtility.WriteIdentifier(item.Id, stream, cancellationToken);
            var lenBytes = await SerializerUtility.WriteLength(sequenceItemsBytes, stream, cancellationToken);
            await ms.CopyToAsync(stream, cancellationToken);
            return identBytes + lenBytes + sequenceItemsBytes;
        }
    }
}