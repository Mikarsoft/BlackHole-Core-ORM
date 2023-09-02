
namespace BlackHole.Internal
{
    /// <summary>
    /// Generic table parsing info
    /// </summary>
    public class TableParsingInfo
    {
        /// <summary>
        /// Table Name
        /// </summary>
        public string TableName { get; set; } = string.Empty;
        /// <summary>
        /// Current Column Name
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Referenced Column
        /// </summary>
        public string ReferencedColumn { get; set; } = string.Empty;

        /// <summary>
        /// Default value of the column
        /// </summary>
        public string DefaultValue { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Extra { get; set; } = string.Empty;

        /// <summary>
        /// Current Column Data Type
        /// </summary>
        public string DataType { get; set; } = string.Empty;
        /// <summary>
        /// Current Column Data Length
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// Numeric Precision of the Column
        /// </summary>
        public int NumPrecision { get; set; }
        /// <summary>
        /// Numeric Scale of the Column
        /// </summary>
        public int NumScale { get; set; }
        /// <summary>
        /// Nullability
        /// </summary>
        public bool Nullable { get; set; }
        /// <summary>
        /// Is Primary Key
        /// </summary>
        public bool PrimaryKey { get; set; }
        /// <summary>
        /// Delete case action
        /// </summary>
        public string DeleteRule { get; set; } = string.Empty;
        /// <summary>
        /// Foreign key Referenced table
        /// </summary>
        public string ReferencedTable { get; set; } = string.Empty;
        /// <summary>
        /// Constraint Name
        /// </summary>
        public string ConstraintName { get; set; } = string.Empty;
    }

    internal class TableAspectsInfo
    {
        internal string TableName { get; set; } = string.Empty;
        internal List<TableParsingInfo> TableColumns { get; set; } = new List<TableParsingInfo>();
        internal bool InCompatibleIdColumn { get; set; }
        internal bool MissingInactiveColumn { get; set; }
        internal bool GeneralError { get; set; }
        internal bool IncompatibleIdDataType { get; set; }

        internal bool BothIncompatible
        {
            get
            {
                return InCompatibleIdColumn && MissingInactiveColumn;
            }
        }
    }
}
