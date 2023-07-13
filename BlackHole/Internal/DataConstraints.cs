
namespace BlackHole.Internal
{
    /// <summary>
    /// Constrains of the table
    /// </summary>
    public class DataConstraints
    {
        /// <summary>
        /// Main Table
        /// </summary>
        public string TABLE_NAME { get; set; } = "";
        /// <summary>
        /// Column 
        /// </summary>
        public string COLUMN_NAME { get; set; } = "";
        /// <summary>
        /// Constraint Name
        /// </summary>
        public string CONSTRAINT_NAME { get; set; } = "";
        /// <summary>
        /// Referenced table of foreign key
        /// </summary>
        public string REFERENCED_TABLE_NAME { get; set; } = "";
        /// <summary>
        /// Delete occasion action
        /// </summary>
        public string DELETE_RULE { get; set; } = "";
    }
}
