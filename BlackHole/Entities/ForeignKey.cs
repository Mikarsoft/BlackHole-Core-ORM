namespace BlackHole.Entities
{
    /// <summary>
    /// Sets Foreign Key for this Column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignKey : Attribute
    {
        /// <summary>
        /// Name of the Foreign Table
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Name of the column
        /// </summary>
        public string Column { get; set; }
        /// <summary>
        /// Nullability
        /// </summary>
        public string IsNullable { get; set; }
        /// <summary>
        /// On Delete
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="isNullable"></param>
        public ForeignKey(Type table, string columnName, bool isNullable)
        {
            TableName = table.Name;
            Column = columnName;

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
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        public ForeignKey(Type table, string columnName)
        {
            TableName = table.Name;
            Column = columnName;
            IsNullable = "NULL";
            CascadeInfo = "on delete set null";
        }
    }
}
