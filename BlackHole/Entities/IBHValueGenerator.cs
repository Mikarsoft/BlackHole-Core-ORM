namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHValueGenerator<T> where T : IComparable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T GenerateValue();
    }
}
