
namespace BlackHole.Internal
{
    public class TableParsingInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool Nullable { get; set; }
        public bool PrimaryKey { get; set; }
        public string DeleteRule { get; set; } = string.Empty;
        public string ReferencedTable { get; set; } = string.Empty;
    }
}
