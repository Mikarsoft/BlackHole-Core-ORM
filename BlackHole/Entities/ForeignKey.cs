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
        /// This Overload of the Constructor Sets by Default the corresponding column
        /// on the Primary Table as Id. You Can choose the Primary Table and
        /// if the Foreign Key is Nullable
        /// </summary>
        /// <param name="table">Type of the parent Table</param>
        public ForeignKey(Type table)
        {
            TableName = table.Name;
            Column = "Id";

            //if (isNullable)
            //{
            //    CascadeInfo = "on delete set null";
            //}
            //else
            //{
            //    CascadeInfo = "on delete cascade";
            //}
        }

        /// <summary>
        /// Set the Column as Foreign Key that points to specific Table and Column.
        /// </summary>
        /// <param name="table">Type of the other Table</param>
        /// <param name="columnName">Name of the other Table's Column</param>
        public ForeignKey(Type table, string columnName)
        {
            TableName = table.Name;
            Column = columnName;
        }
    }
}
