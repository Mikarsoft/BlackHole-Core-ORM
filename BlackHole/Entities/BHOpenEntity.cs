using BlackHole.Identifiers;

namespace BlackHole.Entities
{
    /// <summary>
    /// Flexible Black Hole Entity with more options. The table in database is based on this.
    /// <para>It is Suggested for more advanced developers.</para>
    /// </summary>
    public abstract class BHOpenEntity<Self> : IBHEntityIdentifier where Self : BHOpenEntity<Self>
    {
        /// <summary>
        /// Configuration method for the BHOpenEntity.
        /// <para><b>Important</b> => The result of this method is Required. All 'builder' methods will return a Settings Object.</para>
        /// </summary>
        /// <param name="builder">Entity's Settings Builder</param>
        public abstract EntitySettings<Self> EntityOptions(EntityOptionsBuilder<Self> builder);
    }
}
