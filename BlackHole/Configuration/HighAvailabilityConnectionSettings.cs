using BlackHole.Enums;

namespace BlackHole.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class HighAvailabilityConnectionSettings
    {
        internal HighAvailabilitySecondDbConfig secondDbConfig = new();

        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal string ConnectionString { get; set; } = string.Empty;
        internal bool QuotedDb { get; set; } = false;
        internal string DatabaseIdentity { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseSqlServerAsMain(string connectionString, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="quotedDb"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseSqlServerAsMain(string connectionString, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseNpgSqlAsMain(string connectionString, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseMySqlAsMain(string connectionString, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseOracleAsMain(string connectionString, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            ConnectionString = connectionString;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseSqliteAsMain(string databaseName, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return secondDbConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="quotedDb"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public HighAvailabilitySecondDbConfig UseSqliteAsMain(string databaseName, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
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
        internal string DatabaseIdentity { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionAdditionalSettings additionalConfig { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseSqlServerAsStandBy(string connectionString, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
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
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseSqlServerAsStandBy(string connectionString, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
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
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseNpgSqlAsStandBy(string connectionString, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
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
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseMySqlAsStandBy(string connectionString, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
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
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseOracleAsStandBy(string connectionString, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            BackUpIsSelected = true;
            QuotedDb = true;
            ConnectionString = connectionString;
            return additionalConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseSqliteAsStandBy(string databaseName, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
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
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public ConnectionAdditionalSettings UseSqliteAsStandBy(string databaseName, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            BackUpIsSelected = true;
            QuotedDb = quotedDb;
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalConfig;
        }
    }
}
