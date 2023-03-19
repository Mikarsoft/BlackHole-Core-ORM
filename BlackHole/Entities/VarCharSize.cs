namespace BlackHole.Entities
{
    /// <summary>
    /// Specifies the Size of a Varchar column in the database
    /// The default size is 255
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VarCharSize : Attribute
    {
        public int Charlength { get; set; }

        /// <summary>
        /// Specifies the Size of a Varchar column in the database
        /// The default size is 255
        /// </summary>
        /// <param name="Characters">The number of Characters. Varchar(n)</param>
        public VarCharSize(int Characters)
        {
            Charlength = Characters;
        }
    }
}
