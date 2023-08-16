namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class BHPrimaryKey<T> where T : IComparable<T>
    {
        internal readonly IDefaultValueGenerator<T>? defaultValueGenerator;
        internal readonly Type? PropType;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        public BHPrimaryKey(IDefaultValueGenerator<T> defaultValue)
        {
            defaultValueGenerator = defaultValue;
            PropType = typeof(T);
        }

        /// <summary>
        /// 
        /// </summary>
        public BHPrimaryKey()
        {
            PropType = typeof(T);
        }
    }
}
