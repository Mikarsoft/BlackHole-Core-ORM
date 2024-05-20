

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class BlackHoleBaseConfig
    {
        internal BlackHoleBaseConfig()
        {
            DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BlackHoleData");
            UseLogger = true;
            UseLogsCleaner = true;
            CleanUpDays = 60;
        }

        internal BHDatabaseConfig? _databaseConfig {  get; set; }

        internal string DataPath { get; set; }

        internal bool UseLogger { get; set; }

        internal bool UseLogsCleaner { get; set; }

        internal int CleanUpDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public SqlServerConfig UseSqlServer(int connectionTimeout = 60)
        {
            _databaseConfig = new SqlServerConfig(false , connectionTimeout);
            return (SqlServerConfig)_databaseConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public SqlServerConfig UseQuotedSqlServer(int connectionTimeout = 60)
        {
            _databaseConfig = new SqlServerConfig(true, connectionTimeout);
            return (SqlServerConfig)_databaseConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public NpgsqlConfig UseNpgsql(int connectionTimeout = 60)
        {
            _databaseConfig = new NpgsqlConfig(true , connectionTimeout);
            return (NpgsqlConfig)_databaseConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public OracleDbConfig UseOracleDb(int connectionTimeout = 60)
        {
            _databaseConfig = new OracleDbConfig(true , connectionTimeout);
            return (OracleDbConfig)_databaseConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public SqLiteConfig UseSqLite(int connectionTimeout = 60)
        {
            _databaseConfig = new SqLiteConfig(false, connectionTimeout);
            return (SqLiteConfig)_databaseConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public SqLiteConfig UseQuotedSqLite(int connectionTimeout = 60)
        {
            _databaseConfig = new SqLiteConfig(true, connectionTimeout);
            return (SqLiteConfig)_databaseConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public MySqlConfig UseMySql(int connectionTimeout = 60)
        {
            _databaseConfig = new MySqlConfig(false, connectionTimeout);
            return (MySqlConfig)_databaseConfig;
        }
    }
}
