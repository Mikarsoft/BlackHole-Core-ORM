
namespace BlackHole.Internal
{
    public class DataConstraints
    {
        public string TABLE_NAME { get; set; } = "";
        public string COLUMN_NAME { get; set; } = "";
        public string CONSTRAINT_NAME { get; set; } = "";
        public string REFERENCED_TABLE_NAME { get; set; } = "";
        public string DELETE_RULE { get; set; } = "";
    }
}
