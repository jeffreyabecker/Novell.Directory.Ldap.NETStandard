using LightDirectory.Asn1.Universal;

namespace LightDirectory.Asn1
{
    public class Identifier
    {
        public TagClass TagClass { get; }
        public bool Constructed { get; }
        public int Tag { get; }

        public Identifier(Identifier that)
        {
            TagClass = that.TagClass;
            Constructed = that.Constructed;
            Tag = that.Tag;
        }
        public Identifier(TagClass tagClass, bool constructed, int tag)
        {
            TagClass = tagClass;
            Constructed = constructed;
            Tag = tag;
        }
    }
}
