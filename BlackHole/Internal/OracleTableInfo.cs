
namespace BlackHole.Internal
{
    /// <summary>
    /// Information for oracle Table Columns
    /// </summary>
    public class OracleTableInfo
    {
        /// <summary>
        /// Column Name
        /// </summary>
        public string COLUMN_NAME { get; set; } = string.Empty;
        /// <summary>
        /// Data type of column
        /// </summary>
        public string DATA_TYPE { get; set; } = string.Empty;
        /// <summary>
        /// Data length of column
        /// </summary>
        public int DATA_LENGTH { get; set; }
        /// <summary>
        /// Numeric precision
        /// </summary>
        public int DATA_PRECISION { get; set; } 
        /// <summary>
        /// Nullability
        /// </summary>
        public string NULLABLE { get; set; } = string.Empty;
    }
}
