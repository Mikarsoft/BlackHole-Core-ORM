namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHOpenEntity<Self> where Self : IBHOpenEntity<Self>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public PKSettings<Self> PrimaryKeyOptions(PKOptionsBuilder<Self> builder);
    }
}
