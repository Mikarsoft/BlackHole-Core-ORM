
using BlackHole.Enums;

namespace BlackHole.FunctionalObjects
{
    public class BlackHoleAdvancedConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public BHSqlTypes SqlType { get; set; }
        public bool AutoRegisterBlazarServices { get; set; } = true;
        public List<string> SpecificEntityNamespaces { get; set; } = new List<string>();
        public List<string> SpecificServicesNamespaces { get; set; } = new List<string>();
        public string LogsPath { get; set; } = string.Empty;
    }
}
