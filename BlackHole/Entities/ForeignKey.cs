namespace BlackHole.Entities
{
    /// <summary>
    /// Sets Foreign Key for this Column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKey : Attribute
    {
        public string TableName { get; set; }
        public string Column { get; set; }
        public string IsNullable { get; set; }
        public string CascadeInfo { get; set; }

        /// <summary>
        /// This Overload of the Constructor Sets by Default the corresponding column
        /// on the Primary Table as Id. You Can choose the Primary Table and
        /// if the Foreign Key is Nullable
        /// </summary>
        /// <param name="table">Type of the parent Table</param>
        /// <param name="isNullable">Is this Column Nullable?</param>
        public ForeignKey(Type table, bool isNullable)
        {
            TableName = table.Name;
            Column = "Id";

            if (isNullable)
            {
                IsNullable = "NULL";
                CascadeInfo = "on delete set null";
            }
            else
            {
                IsNullable = "NOT NULL";
                CascadeInfo = "on delete cascade";
            }
        }

        /// <summary>
        /// This Overload of the Constructor Sets by Default the corresponding column
        /// on the Primary Table as Id and makes the Foreign Key Column Nullable.
        /// You Can choose the Primary Table
        /// </summary>
        /// <param name="table">Type of the parent Table</param>
        public ForeignKey(Type table)
        {
            TableName = table.Name;
            Column = "Id";
            IsNullable = "NULL";
            CascadeInfo = "on delete set null";
        }
    }
}
