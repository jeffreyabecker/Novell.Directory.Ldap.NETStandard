using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Suport;
using LightDirectory.Asn1.Universal;

namespace LightDirectory.Asn1.Serialization
{

    public abstract class Asn1ObjectSerializer
    {
        //protected Asn1ClassTagSerializer(){}

        protected Asn1ObjectSerializer(TagClass tagClass, int tag)
        {
            TagClass = tagClass;
            Tag = tag;
        }
        public virtual TagClass TagClass { get; protected set; }
        public virtual int Tag { get; protected set; }

        public virtual bool ShouldDeserialize(Identifier identifier)
        {
            return identifier.TagClass == TagClass && identifier.Tag == Tag;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine((int) TagClass, Tag);
            
        }

        public abstract Task<(Asn1Object, int)> Read(IAsn1Serializer serializer, Identifier id, int length,
            Stream stream,
            CancellationToken cancellationToken);


        public abstract Task<int> Write(IAsn1Serializer asn1Serializer, Asn1Object item, Stream stream, CancellationToken cancellationToken);
    }

    public abstract class Asn1ObjectSerializer<T> : Asn1ObjectSerializer
    where T: Asn1Object
    {
        protected Asn1ObjectSerializer(TagClass tagClass, int tag) : base(tagClass, tag)
        {
        }

        public override Task<int> Write(IAsn1Serializer asn1Serializer, Asn1Object item, Stream stream, CancellationToken cancellationToken)
        {
            var casted = item as T;
            if (casted == null)
            {
                var type = item != null ? item.GetType().ToString() : "null";
                throw new SerializationException($"Unable to convert the passed object, a {type} to a {typeof(T)}");
            }

            return Write(asn1Serializer, casted, stream, cancellationToken);
        }

        public abstract Task<int> Write(IAsn1Serializer asn1Serializer, T item, Stream stream,
            CancellationToken cancellationToken);
    }
}
