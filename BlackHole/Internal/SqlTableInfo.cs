
namespace BlackHole.Internal
{
    public class SqlTableInfo
    {
        public string table_name { get; set; } = string.Empty;
        public string column_name { get; set; } = string.Empty;
        public int ordinal_position { get; set; }
        public string data_type { get; set; } = string.Empty;
        public string column_type { get; set; } = string.Empty;
        public string udt_name { get; set; } = string.Empty;
        public int character_maximum_length { get; set; }
        public string is_nullable { get; set; } = string.Empty;
    }
}
