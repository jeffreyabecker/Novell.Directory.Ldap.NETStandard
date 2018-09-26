namespace LightDirectory.Asn1.Universal
{
    public class Asn1Integer : Asn1Object
    {
        public long Value { get; }

        public Asn1Integer(long value) : base(new Identifier(TagClass.Universal, false, (int)UniversalTags.Integer))
        {
            Value = value;
        }
    }
}
