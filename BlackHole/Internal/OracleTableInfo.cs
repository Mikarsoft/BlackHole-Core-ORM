
namespace BlackHole.Internal
{
    public class OracleTableInfo
    {
        public string COLUMN_NAME { get; set; } = string.Empty;
        public string DATA_TYPE { get; set; } = string.Empty;
        public int DATA_LENGTH { get; set; }
        public int DATA_PRECISION { get; set; } 
        public string NULLABLE { get; set; } = string.Empty;
    }
}
