namespace BlackHole.Entities
{
    /// <summary>
    /// It turns the property to a Non Nullable Column in the Table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotNullable : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string IsNotNull = "NOT NULL";

        /// <summary>
        /// 
        /// </summary>
        public object? ValueDefault { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDatetimeValue { get; set; }


        /// <summary>
        /// It turns the property to a Non Nullable Column in the Table.
        /// </summary>
        public NotNullable()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        public NotNullable(object defaultValue)
        {
            ValueDefault = defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public NotNullable(int year, int month, int day)
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
        public NotNullable(int year, int month, int day, string dateFormat)
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
