
using BlackHole.Statics;

namespace BlackHole.Entities
{
    /// <summary>
    /// Sets Default Value to a column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValue : Attribute
    {
        /// <summary>
        /// The Default Value
        /// </summary>
        public object? ValueDefault { get; set; }

        /// <summary>
        /// Checks if Default value is DateTime
        /// </summary>
        public bool IsDatetimeValue { get; set; }

        /// <summary>
        /// Sets Default Value to a column.
        /// </summary>
        /// <param name="valueDefault"></param>
        public DefaultValue(object valueDefault)
        {
            ValueDefault = valueDefault;
        }

         /// <summary>
         /// Sets a default value to a Datetime column in the Database
         /// </summary>
         /// <param name="year">Year</param>
         /// <param name="month">Month</param>
         /// <param name="day">Day</param>
        public DefaultValue(int year, int month, int day)
        {
            try
            {
                ValueDefault = new DateTime(year, month, day).ToString(DatabaseStatics.DbDateFormat);
                IsDatetimeValue = true;
            }
            catch
            {
                ValueDefault = null;            
            }
        }

        /// <summary>
        /// Sets a default value to a Datetime column in the Database with specific format.
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <param name="day">Day</param>
        /// <param name="dateFormat">Date Format</param>
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
