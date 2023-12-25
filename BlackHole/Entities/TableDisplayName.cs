namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableDisplayName : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public TableDisplayName(string tableName)
        {
            TableName = tableName;
        }
    }
}
