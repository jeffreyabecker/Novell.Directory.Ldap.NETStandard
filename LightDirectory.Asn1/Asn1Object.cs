namespace LightDirectory.Asn1
{
    public abstract class Asn1Object
    {
        public virtual Identifier Id { get; set; }

        protected Asn1Object(Identifier id)
        {
            Id = id;
        }
    }
}
