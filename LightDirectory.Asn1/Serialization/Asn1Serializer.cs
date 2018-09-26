using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightDirectory.Asn1.Suport;
using LightDirectory.Asn1.Universal.Serializers;

namespace LightDirectory.Asn1.Serialization
{
    public abstract class Asn1Serializer : IAsn1Serializer
    {
        protected readonly  Lazy<ICollection<Asn1ObjectSerializer>> Serializers ;

        protected Asn1Serializer()
        {
            Serializers = new Lazy<ICollection<Asn1ObjectSerializer>>(this.BuildSerializers);
        }
        
        protected virtual ICollection<Asn1ObjectSerializer> BuildSerializers()
        {
            var serializers = new List<Asn1ObjectSerializer>();
            InitializeUniversal(serializers);
            InitializeApplicationSerializers(serializers);
            return serializers;
        }
        protected abstract void InitializeApplicationSerializers(ICollection<Asn1ObjectSerializer> serializers);
        protected virtual void InitializeUniversal(ICollection<Asn1ObjectSerializer> serializers)
        {
            serializers.Add(new Asn1BooleanSerializer());
            serializers.Add(new Asn1EnumeratedSerializer());
            serializers.Add(new Asn1NullSerializer());
            serializers.Add(new Asn1OctetStringSerializer());
            serializers.Add(new Asn1SequenceSerializer());
            serializers.Add(new Asn1SetSerializer());
            serializers.Add(new Asn1IntegerSerializer());
        }
        public async Task<(Asn1Object, int)> Read(Stream stream, CancellationToken cancellationToken)
        {
            var (identifier, identifierBytes) = await SerializerUtility.ReadIdentifier(stream, cancellationToken);
            var (length, lengthBytes) = await SerializerUtility.ReadLength(stream, cancellationToken);
            var serializerToUse = FindSerializer(identifier);
            var (item, itemBytes) = await serializerToUse.Read(this, identifier, length, stream, cancellationToken);
            return (item, itemBytes + identifierBytes + lengthBytes);
        }

        protected Asn1ObjectSerializer FindSerializer(Identifier identifier)
        {
            var serializerToUse = Serializers.Value.FirstOrDefault(s => s.ShouldDeserialize(identifier));
            if (serializerToUse == null)
                throw new DeserializationException($"Unable to find deserializer for {identifier.TagClass}, {identifier.Tag}");
            return serializerToUse;
        }

        public Task<int> Write(Asn1Object item, Stream stream, CancellationToken cancellationToken)
        {
            var serializerToUser = FindSerializer(item.Id);
            return serializerToUser.Write(this, item, stream, cancellationToken);
        }
    }
}