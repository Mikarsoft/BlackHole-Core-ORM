using BlackHole.Enums;

namespace BlackHole.FunctionalObjects
{
    public class BlackHoleBaseConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public BHSqlTypes SqlType { get; set; }
        public string LogsPath { get; set; } = string.Empty;
    }
}
