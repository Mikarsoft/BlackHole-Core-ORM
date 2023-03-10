namespace BlackHole.Attributes.ColumnAttributes
{
    /// <summary>
    /// It turns the property to a Non Nullable Column in the Table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullable : Attribute
    {
        public string IsNotNull = "NOT NULL";

        /// <summary>
        /// It turns the property to a Non Nullable Column in the Table.
        /// </summary>
        public NotNullable()
        {

        }
    }
}
