
namespace BlackHole.Internal
{
    /// <summary>
    /// Generic Sql Table Info
    /// </summary>
    public class SqlTableInfo
    {
        /// <summary>
        /// Table Name
        /// </summary>
        public string table_name { get; set; } = string.Empty;
        /// <summary>
        /// Current column Name
        /// </summary>
        public string column_name { get; set; } = string.Empty;
        /// <summary>
        /// Ordinal position
        /// </summary>
        public int ordinal_position { get; set; }
        /// <summary>
        /// Column Data Type
        /// </summary>
        public string data_type { get; set; } = string.Empty;
        /// <summary>
        /// Column Type
        /// </summary>
        public string column_type { get; set; } = string.Empty;
        /// <summary>
        /// udt Name
        /// </summary>
        public string udt_name { get; set; } = string.Empty;
        /// <summary>
        /// Character Length Size
        /// </summary>
        public int character_maximum_length { get; set; }
        /// <summary>
        /// Nullability
        /// </summary>
        public string is_nullable { get; set; } = string.Empty;
    }
}
