
namespace BlackHole.Entities
{
    /// <summary>
    /// Sets Foreign Key for this Column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OpenForeignKey : Attribute
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
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="isNullable"></param>
        public OpenForeignKey(Type table,string columnName, bool isNullable)
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
        public OpenForeignKey(Type table, string columnName)
        {
            TableName = table.Name;
            Column = columnName;
            IsNullable = "NULL";
            CascadeInfo = "on delete set null";
        }
    }
}
