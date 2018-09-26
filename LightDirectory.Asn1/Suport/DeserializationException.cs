using System;

namespace LightDirectory.Asn1.Suport
{
    public class DeserializationException : Exception
    {
        public DeserializationException(string message) : base(message){}
    }
    public class SerializationException : Exception
    {
        public SerializationException(string message) : base(message){}
    }
}
