using BlackHole.Enums;

namespace BlackHole.FunctionalObjects
{
    public class BlackHoleExtraConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public BHSqlTypes SqlType { get; set; }
        public bool AutoRegisterBlazarServices { get; set; } = true;
        public string SpecificEntityNamespace { get; set; } = string.Empty;
        public string SpecificServicesNamespace { get; set; } = string.Empty;
        public string LogsPath { get; set; } = string.Empty;
    }
}
