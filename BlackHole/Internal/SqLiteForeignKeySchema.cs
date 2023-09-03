

namespace BlackHole.Internal
{
    /// <summary>
    /// SqLite  foreign key mapping
    /// </summary>
    public class SqLiteForeignKeySchema
    {
        /// <summary>
        /// Index of column
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Sequence
        /// </summary>
        public int seq { get; set; }
        /// <summary>
        /// Current table
        /// </summary>
        public string table { get; set; } = string.Empty;
        /// <summary>
        /// Current table column
        /// </summary>
        public string from { get; set; } = string.Empty;
        /// <summary>
        /// Referenced Table
        /// </summary>
        public string to { get; set; } = string.Empty;
        /// <summary>
        /// Update case action
        /// </summary>
        public string on_update { get; set; } = string.Empty;
        /// <summary>
        /// Delete case action
        /// </summary>
        public string on_delete { get; set; } = string.Empty;
        /// <summary>
        /// matching columns
        /// </summary>
        public string match { get; set; } = string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    public class LiteAutoIncrementInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int seq { get; set; }
    }
}
