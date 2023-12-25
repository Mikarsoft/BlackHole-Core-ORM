namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnDisplayName : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        public ColumnDisplayName(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
