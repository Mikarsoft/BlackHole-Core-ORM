namespace BlackHole.Attributes.ColumnAttributes
{
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
