using System.Collections.Generic;
using System.Text;

namespace LightDirectory.Asn1.Universal
{
    public class Asn1OctetString : Asn1Object
    {
        public byte[] Data { get; }

        public Asn1OctetString(byte[] data) : base(new Identifier(TagClass.Universal, false, (int)UniversalTags.OctetString))
        {
            Data = data;
        }

        public Asn1OctetString(string s) : this(Encoding.UTF8.GetBytes(s))
        {

        }
    }

    public class Asn1Sequence : Asn1Object
    {
        public ICollection<Asn1Object> Items { get; }

        public Asn1Sequence(ICollection<Asn1Object> items) : base(new Identifier(TagClass.Universal, true, (int)UniversalTags.Sequence))
        {
            Items = items ?? new List<Asn1Object>();
        }
    }

    public class Asn1Set : Asn1Object
    {
        public ICollection<Asn1Object> Items { get; }

        public Asn1Set(ICollection<Asn1Object> items) : base(new Identifier(TagClass.Universal, true, (int)UniversalTags.Sequence))
        {
            Items = items ?? new List<Asn1Object>();
        }
    }

}
