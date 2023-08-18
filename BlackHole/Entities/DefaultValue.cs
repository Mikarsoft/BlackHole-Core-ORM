
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
        public object? ValueDefault { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDatetimeValue { get; set; }

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
                ValueDefault = new DateTime(year, month, day).ToString("MM-dd-yyyy");
                IsDatetimeValue = true;
            }
            catch
            {
                ValueDefault = null;            
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="dateFormat"></param>
        public DefaultValue(int year, int month, int day, string dateFormat)
        {
            try
            {
                ValueDefault = new DateTime(year, month, day).ToString(dateFormat);
                IsDatetimeValue = true;
            }
            catch
            {
                ValueDefault = null;
            }
        }
    }
}
