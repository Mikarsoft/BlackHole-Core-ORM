namespace BlackHole.Internal
{
    /// <summary>
    /// Information of Sqlite table
    /// </summary>
    public class SqLiteTableInfo
    {
        /// <summary>
        /// Cid of the column
        /// </summary>
        public int cid { get; set; }
        /// <summary>
        /// Name of a column
        /// </summary>
        public string name { get; set; } = string.Empty;
        /// <summary>
        /// type of column
        /// </summary>
        public string type { get; set; } = string.Empty;
        /// <summary>
        /// Nullability
        /// </summary>
        public bool notnull { get; set; }
        /// <summary>
        /// Default Value
        /// </summary>
        public string dflt_value { get; set; } = string.Empty;
        /// <summary>
        /// Is it primary key
        /// </summary>
        public bool pk { get; set; }
    }
}
