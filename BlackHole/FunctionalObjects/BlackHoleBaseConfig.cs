using BlackHole.Enums;

namespace BlackHole.FunctionalObjects
{
    /// <summary>
    /// Simple Configuration that is using all Black Hole Entities
    /// in the Calling Assembly and automatically registers all
    /// Black Hole Services and Interfaces
    /// </summary>
    public class BlackHoleBaseConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public BHSqlTypes SqlType { get; set; }
        public string LogsPath { get; set; } = string.Empty;
    }
}
