using BlackHole.Enums;
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiSchemaConnectionSettings
    {
        internal string ConnectionString { get; set; } = string.Empty;
        internal string TableSchema { get; set; } = string.Empty;
        internal BlackHoleSqlTypes ConnectionType { get; set; }

        internal MultiSchemaSettings additionalSettings = new();
        internal bool UseQuotedDb { get; set; }

        /// <summary>
        /// <para>Use the data provider for Microsoft Sql Server.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <param name="quotedDb">Use quotes for Naming</param>
        /// <returns>Additional Settings</returns>
        public MultiSchemaSettings UseSqlServer(string connectionString, bool quotedDb)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            UseQuotedDb = quotedDb;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Microsoft Sql Server.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Additional Settings</returns>
        public MultiSchemaSettings UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            UseQuotedDb = false;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Postgresql.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Additional Settings</returns>
        public MultiSchemaSettings UseNpgSql(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }
    }

    public class MultiSchemaSettings
    {
        internal List<Assembly> AssembliesToUse { get; set; } = new List<Assembly>();
        internal bool useCallingAssembly { get; set; } = true;
        internal int ConnectionTimeOut { get; set; } = 60;

        /// <summary>
        /// Change the Timeout of each command.The Default timeout of BlackHole is 60s.
        /// <para>This Feature does not apply to SqLite database</para>
        /// </summary>
        /// <param name="timoutInSeconds">The timtout in seconds that will be applied to each command</param>
        /// <returns>Connection Additional Settings</returns>
        public MultiSchemaSettings SetConnectionTimeoutSeconds(int timoutInSeconds)
        {
            ConnectionTimeOut = timoutInSeconds;
            return this;
        }
    }
}
