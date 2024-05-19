using BlackHole.Enums;

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class BHDatabaseConfig
    {
        internal bool UseQuotedDb {  get; set; }

        internal int ConnectionTimeout { get; set; }

        internal bool UseAutomaticUpdate { get; set; }

        internal bool UseDevMode { get; set; }

        internal BHModeConfig? _bhBase { get; set; }

        internal BlackHoleSqlTypes BHDatabaseType { get;}


        internal BHDatabaseConfig(bool useQuotedDb, int connectionTimeout, BlackHoleSqlTypes bhType)
        {
            UseQuotedDb = useQuotedDb;
            ConnectionTimeout = connectionTimeout > 59 ? connectionTimeout : 60;
            BHDatabaseType = bhType;
        }
    }
}
