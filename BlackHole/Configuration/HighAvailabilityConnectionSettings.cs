using BlackHole.Enums;

namespace BlackHole.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class HighAvailabilityConnectionSettings
    {
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal string ConnectionString { get; set; } = string.Empty;
        internal bool QuotedDb { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public HighAvailabilitySecondDbConfig secondDbConfig { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseSqlServerAsMain(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="quotedDb"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseSqlServerAsMain(string connectionString, bool quotedDb)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseNpgSqlAsMain(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseMySqlAsMain(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseOracleAsMain(string connectionString)
        {
            ConnectionString = connectionString;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseSqliteAsMain(string databaseName)
        {
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="quotedDb"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseSqliteAsMain(string databaseName, bool quotedDb)
        {
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HighAvailabilitySecondDbConfig
    {
        internal bool BackUpIsSelected { get; set; }
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal string ConnectionString { get; set; } = string.Empty;
        internal bool QuotedDb { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionAdditionalSettings additionalConfig { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseSqlServerAsStandBy(string connectionString)
        {
            BackUpIsSelected = true;
            QuotedDb = false;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="quotedDb"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseSqlServerAsStandBy(string connectionString, bool quotedDb)
        {
            BackUpIsSelected = true;
            QuotedDb = quotedDb;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseNpgSqlAsStandBy(string connectionString)
        {
            BackUpIsSelected = true;
            QuotedDb = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseMySqlAsStandBy(string connectionString)
        {
            BackUpIsSelected = true;
            QuotedDb = false;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseOracleAsStandBy(string connectionString)
        {
            BackUpIsSelected = true;
            QuotedDb = true;
            ConnectionString = connectionString;
            return additionalConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseSqliteAsStandBy(string databaseName)
        {
            BackUpIsSelected = true;
            QuotedDb = false;
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="quotedDb"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseSqliteAsStandBy(string databaseName, bool quotedDb)
        {
            BackUpIsSelected = true;
            QuotedDb = quotedDb;
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalConfig;
        }
    }
}
