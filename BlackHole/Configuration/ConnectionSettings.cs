

using BlackHole.Enums;

namespace BlackHole.Configuration
{
    public class ConnectionSettings
    {
        internal string ConnectionString { get; set; } = string.Empty;
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal ConnectionAdditionalSettings additionalSettings = new ConnectionAdditionalSettings();

        /// <summary>
        /// Use the data provider for Microsoft Sql Server
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Connection additional settings</returns>
        public ConnectionAdditionalSettings UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalSettings;
        }

        /// <summary>
        /// Use the data provider for Postgresql
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Connection additional settings</returns>
        public ConnectionAdditionalSettings UseNpgSql(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }

        /// <summary>
        /// Use the data provider for MySql
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Connection additional settings</returns>
        public ConnectionAdditionalSettings UseMySql(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.MySql;
            return additionalSettings;
        }

        /// <summary>
        /// Use the data provider for Sqlite
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Connection additional settings</returns>
        public ConnectionAdditionalSettings UseSqlite(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlLite;
            return additionalSettings;
        }

        /// <summary>
        /// Use the data provider for Oracle database
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Connection additional settings</returns>
        public ConnectionAdditionalSettings UseOracle(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Oracle;
            return additionalSettings;
        }
    }
}
