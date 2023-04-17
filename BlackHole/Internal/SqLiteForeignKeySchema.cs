

namespace BlackHole.Internal
{
    public class SqLiteForeignKeySchema
    {
        public int id { get; set; }
        public int seq { get; set; }
        public string table { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public string on_update { get; set; } = string.Empty;
        public string on_delete { get; set; } = string.Empty;
        public string match { get; set; } = string.Empty;
    }
}
