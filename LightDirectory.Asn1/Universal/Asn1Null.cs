namespace LightDirectory.Asn1.Universal
{
    public class Asn1Null : Asn1Object
    {
        public Asn1Null() : base(new Identifier(TagClass.Universal, false, (int)UniversalTags.Null))
        {
        }
    }
}