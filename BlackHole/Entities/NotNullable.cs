namespace BlackHole.Entities
{
    /// <summary>
    /// It turns the property to a Non Nullable Column in the Table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotNullable : Attribute
    {
        /// <summary>
        /// Nullability
        /// </summary>
        public string IsNotNull = "NOT NULL";

        /// <summary>
        /// Default Value of Column in Database
        /// </summary>
        public object? ValueDefault { get; set; }

        /// <summary>
        /// Checks if Default Value is DateTime
        /// </summary>
        public bool IsDatetimeValue { get; set; }


        /// <summary>
        /// It turns the property to a Non Nullable Column in the Table.
        /// </summary>
        public NotNullable()
        {

        }

        /// <summary>
        /// It turns the property to a Non Nullable Column and Setting Default Value on the Column
        /// </summary>
        /// <param name="defaultValue"></param>
        public NotNullable(object defaultValue)
        {
            ValueDefault = defaultValue;
        }

        /// <summary>
        /// It turns the DateTime property to a Non Nullable Column and Setting Default Value on the Column
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
        /// Setting Default Value on Not Nullable DateTime Column with specific format
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
