namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDefaultValueGenerator<T> where T : IComparable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GenerateValue();
    }
}
