
using BlackHole.Enums;

namespace BlackHole.Configuration
{
    /// <summary>
    /// An Advanced BlackHole Configuration with all the options.
    /// It let's you choose Multiple Namespaces for your Entities and  for your Black Hole
    /// Services in your Assembly and also let's you decide if the services 
    /// will be registered automatically.
    /// If you don't decleare any Namespaces then All Namespaces will be used
    /// </summary>
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
