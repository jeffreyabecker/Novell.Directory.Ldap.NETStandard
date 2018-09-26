namespace LightDirectory.Asn1.Universal
{
    public enum TagClass
    {
        /// <summary>
        ///     IsUniversal tag class.
        ///     UNIVERSAL = 0.
        /// </summary>
        Universal = 0,

        /// <summary>
        ///     IsApplication-wide tag class.
        ///     APPLICATION = 1.
        /// </summary>
        Application = 1,

        /// <summary>
        ///     IsContext-specific tag class.
        ///     CONTEXT = 2.
        /// </summary>
        Context = 2,

        /// <summary>
        ///     IsPrivate-use tag class.
        ///     PRIVATE = 3.
        /// </summary>
        Private = 3,
    }

    public enum Asn1UniversalTagKind
    {
        Boolean = 1,
        Integer = 2,
        OctetString = 4,
        Null = 5,
        Enumerated = 10,
        Sequence = 16,
        Set = 17,
        
    }
}
