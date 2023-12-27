namespace BlackHole.Internal
{
    internal class FKInfo
    {
        internal string PropertyName { get; set; } = string.Empty;
        internal string ReferencedTable { get; set; } = string.Empty;
        internal string ReferencedColumn { get; set; } = string.Empty;
        internal bool IsNullable { get; set; }
    }

    internal class PKInfo
    {
        internal string MainPrimaryKey { get; set; } = string.Empty;
        internal bool HasAutoIncrement { get; set; }
        internal List<string> PKPropertyNames { get; set; } = new();
    }

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

    internal class UniqueInfo
    {
        internal string PropertyName { get; set; } = string.Empty;
        internal int GroupId { get; set; }
    }

    /// <summary>
    /// SqLite autoincrement info.
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
        public int pk { get; set; }
    }

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
        /// Autoincrement on MySql
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

        /// <summary>
        /// Autoincrement for sql server check
        /// </summary>
        public bool IsIdentity { get; set; }
    }

    internal class TableAspectsInfo
    {
        internal string TableName { get; set; } = string.Empty;
        internal List<TableParsingInfo> TableColumns { get; set; } = new List<TableParsingInfo>();
        internal bool GeneralError { get; set; }
        internal bool IncompatibleIdDataType { get; set; }
        internal bool UseOpenEntity { get; set; }
        internal bool CheckPrimaryKeys { get; set; }
        internal PKConfiguration? Configuration { get; set; }
    }

    internal class PKConfiguration
    {
        internal string MainPrimaryKey { get; set; } = string.Empty;
        internal bool HasAutoIncrement { get; set; }
        internal string WarningMessage { get; set; } = string.Empty;
        internal int PKCount { get; set; }
    }

    internal class ColumnScanResult
    {
        internal bool UnidentifiedColumn { get; set; }
        internal string DefaultValue { get; set; } = string.Empty;
        internal string PropertyNameForColumn { get; set; } = string.Empty;
    }
}
