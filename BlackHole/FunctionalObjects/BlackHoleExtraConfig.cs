using BlackHole.Enums;

namespace BlackHole.FunctionalObjects
{
    /// <summary>
    /// A BlackHole Configuration with some extra options.
    /// It let's you choose specific Namespace for your Entities and  for your Black Hole
    /// Services in your Assembly and also let's you decide if the services 
    /// will be registered automatically
    /// If you don't decleare any Namespaces then All Namespaces will be used
    /// </summary>
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
