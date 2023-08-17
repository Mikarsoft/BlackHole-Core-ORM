
namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValue : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public object ValueDefault { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueDefault"></param>
        public DefaultValue(object valueDefault)
        {
            ValueDefault = valueDefault;
        }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="year"></param>
         /// <param name="month"></param>
         /// <param name="day"></param>
        public DefaultValue(int year, int month, int day)
        {
            try
            {
                ValueDefault = new DateTime(year, month, day);
            }
            catch
            {
                ValueDefault = DateTime.MinValue;
            }
        }
    }
}
