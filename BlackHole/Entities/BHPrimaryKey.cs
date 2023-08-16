namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class BHPrimaryKey<T> where T : IComparable<T>
    {
        internal readonly IBHValueGenerator<T>? defaultValueGenerator;
        internal readonly Type? PropType;
        internal T? Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="generateValue"></param>
        public BHPrimaryKey(IBHValueGenerator<T> generateValue)
        {
            defaultValueGenerator = generateValue;
            PropType = typeof(T);
        }

        /// <summary>
        /// 
        /// </summary>
        public BHPrimaryKey()
        {
            PropType = typeof(T);
        }

        internal void Autogenerate()
        {
            if(defaultValueGenerator != null)
            {
                Value = defaultValueGenerator.GenerateValue();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        public void Set(T? thisValue) => Value = thisValue;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T? Get() => Value;
    }
}
