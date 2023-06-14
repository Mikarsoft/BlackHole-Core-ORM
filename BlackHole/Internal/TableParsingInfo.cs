
namespace BlackHole.Internal
{
    public class TableParsingInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public int MaxLength { get; set; }
        public int NumPrecision { get; set; }
        public int NumScale { get; set; }
        public bool Nullable { get; set; }
        public bool PrimaryKey { get; set; }
        public string DeleteRule { get; set; } = string.Empty;
        public string ReferencedTable { get; set; } = string.Empty;
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
