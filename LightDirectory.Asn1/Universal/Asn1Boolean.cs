namespace LightDirectory.Asn1.Universal
{
    public class Asn1Boolean : Asn1Object
    {
        public bool Value { get; }

        public Asn1Boolean(bool value) : base(new Identifier(TagClass.Universal, false, (int) UniversalTags.Boolean))
        {
            Value = value;
        }
    }
}
