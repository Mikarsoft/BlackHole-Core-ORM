
namespace Mikarsoft.BlackHoleCore.Connector
{
    public class BHDatabaseConfig
    {
        public bool UseQuotedDb { get; set; }

        public int ConnectionTimeout { get; set; }

        public bool UseAutomaticUpdate { get; set; }

        public bool UseDevMode { get; set; }

        public BHModeConfig? _bhBase { get; set; }


        public BHDatabaseConfig(bool useQuotedDb, int connectionTimeout)
        {
            UseQuotedDb = useQuotedDb;
            ConnectionTimeout = connectionTimeout > 59 ? connectionTimeout : 60;
        }
    }
}
