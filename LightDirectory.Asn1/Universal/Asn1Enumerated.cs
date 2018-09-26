namespace LightDirectory.Asn1.Universal
{
    public class Asn1Enumerated : Asn1Object
    {
        public long Value { get; }

        public Asn1Enumerated(long value) : base(new Identifier(TagClass.Universal, false, (int)UniversalTags.Enumerated))
        {
            Value = value;
        }
    }
}
