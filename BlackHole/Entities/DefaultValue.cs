
namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValue : Attribute
    {
        internal object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueDefault"></param>
        public DefaultValue(object valueDefault)
        {
            Value = valueDefault;
        }
    }
}
