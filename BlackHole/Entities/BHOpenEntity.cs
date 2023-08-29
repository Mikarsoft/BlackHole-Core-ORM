namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface BHOpenEntity<Self> : IBHEntityIdentifier where Self : BHOpenEntity<Self>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public PKSettings<Self> PrimaryKeyOptions(PKOptionsBuilder<Self> builder);
    }
}
