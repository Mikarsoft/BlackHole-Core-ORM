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
    }
}
