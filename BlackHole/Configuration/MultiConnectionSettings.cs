using BlackHole.Enums;
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiConnectionSettings
    {
        internal MultiAdditionalSettings additionalSettings = new();

        internal string ConnectionString { get; set; } = string.Empty;
        internal string TableSchema { get; set; } = string.Empty;
        internal string SelectedNamespace {  get; set; } = string.Empty;
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal Assembly? Ass {  get; set; }
        internal bool IsUsingAssembly { get; set; }
        internal bool UseQuotedDb { get; set; }

        internal string DatabaseIdentity { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlServer(string connectionString, string fromNamespace, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            SelectedNamespace = fromNamespace;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="schema"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlServer(string connectionString, string fromNamespace, string schema, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            TableSchema = schema;
            SelectedNamespace = fromNamespace;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="quotedDb"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlServer(string connectionString, string fromNamespace, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            SelectedNamespace = fromNamespace;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            UseQuotedDb = quotedDb;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="schema"></param>
        /// <param name="quotedDb"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlServer(string connectionString, string fromNamespace, string schema, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            TableSchema = schema;
            SelectedNamespace = fromNamespace;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            UseQuotedDb = quotedDb;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseNpgSql(string connectionString, string fromNamespace, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            UseQuotedDb = true;
            SelectedNamespace = fromNamespace;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="schema"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseNpgSql(string connectionString, string fromNamespace, string schema, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            TableSchema = schema;
            UseQuotedDb = true;
            SelectedNamespace = fromNamespace;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseMySql(string connectionString, string fromNamespace, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            SelectedNamespace = fromNamespace;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.MySql;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromAssembly"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseMySql(string connectionString, Assembly fromAssembly, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            Ass = fromAssembly;
            IsUsingAssembly = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.MySql;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlite(string databaseName, string fromNamespace, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            SelectedNamespace = fromNamespace;
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlLite;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="quotedDb"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlite(string databaseName, string fromNamespace, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            SelectedNamespace = fromNamespace;
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlLite;
            UseQuotedDb = quotedDb;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseOracle(string connectionString, string fromNamespace, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            UseQuotedDb = true;
            SelectedNamespace = fromNamespace;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Oracle;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromAssembly"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseOracle(string connectionString, Assembly fromAssembly, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            Ass = fromAssembly;
            IsUsingAssembly = true;
            UseQuotedDb = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Oracle;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromAssembly"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlServer(string connectionString, Assembly fromAssembly, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            Ass = fromAssembly;
            IsUsingAssembly = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromAssembly"></param>
        /// <param name="schema"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlServer(string connectionString, Assembly fromAssembly, string schema, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            TableSchema = schema;
            Ass = fromAssembly;
            IsUsingAssembly = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromAssembly"></param>
        /// <param name="quotedDb"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlServer(string connectionString, Assembly fromAssembly, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            Ass = fromAssembly;
            IsUsingAssembly = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            UseQuotedDb = quotedDb;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromAssembly"></param>
        /// <param name="schema"></param>
        /// <param name="quotedDb"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseSqlServer(string connectionString, Assembly fromAssembly, string schema, bool quotedDb, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            TableSchema = schema;
            Ass = fromAssembly;
            IsUsingAssembly = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            UseQuotedDb = quotedDb;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromAssembly"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseNpgSql(string connectionString, Assembly fromAssembly, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            Ass = fromAssembly;
            IsUsingAssembly = true;
            UseQuotedDb = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fromAssembly"></param>
        /// <param name="schema"></param>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        public MultiAdditionalSettings UseNpgSql(string connectionString, Assembly fromAssembly, string schema, string dbIdentity)
        {
            DatabaseIdentity = dbIdentity;
            TableSchema = schema;
            Ass = fromAssembly;
            IsUsingAssembly = true;
            UseQuotedDb = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MultiAdditionalSettings
    {
        internal int ConnectionTimeOut { get; set; } = 60;
        internal bool SamePathServices { get; set; } = false;
        internal bool RegisterServices { get; set; } = false;
        internal bool ServicesInOtherAssembly { get; set; } = false;
        internal bool SkipDefaultProjectServices { get; set; } = false;
        internal string ServicesNamespace { get; set; } = string.Empty;
        internal Assembly? ServicesAssembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        public MultiAdditionalSettings SetConnectionTimeoutSeconds(int timeoutInSeconds)
        {
            ConnectionTimeOut = timeoutInSeconds;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ServicesIncluded()
        {
            RegisterServices = true;
            SamePathServices = true;
            SkipDefaultProjectServices = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromNamespace"></param>
        public void AddServicesFromNamespace(string fromNamespace)
        {
            RegisterServices = true;
            ServicesNamespace = fromNamespace;
            SkipDefaultProjectServices = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        public void AddServicesFromOtherAssembly(Assembly assembly)
        {
            RegisterServices = true;
            ServicesInOtherAssembly = true;
            ServicesAssembly = assembly;
            SkipDefaultProjectServices = true;
        }
    }
}
