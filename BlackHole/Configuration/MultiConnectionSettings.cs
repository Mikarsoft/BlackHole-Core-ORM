using BlackHole.Enums;
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiConnectionSettings
    {
        internal string ConnectionString { get; set; } = string.Empty;
        internal string TableSchema { get; set; } = string.Empty;
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal ConnectionAdditionalSettings additionalSettings = new();
        internal bool UseQuotedDb { get; set; }

        /// <summary>
        /// <para>Use the data provider for Microsoft Sql Server.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseSqlServer(string connectionString, string fromNamespace)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Microsoft Sql Server.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <param name="quotedDb">Use quotes for Naming</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseSqlServer(string connectionString, string fromNamespace, bool quotedDb)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            UseQuotedDb = quotedDb;
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
        public ConnectionAdditionalSettings UseNpgSql(string connectionString, string fromNamespace)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for MySql.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseMySql(string connectionString, string fromNamespace)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.MySql;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Sqlite.</para>
        /// <para>The Sqlite database is stored in the Default
        /// BlackHole DataPath. You can only choose the file name here.</para>
        /// <para>If you need to move it elsewhere you have to use 'SetDataPath()'</para>
        /// </summary>
        /// <param name="databaseName">Just the name of the database</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseSqlite(string databaseName, string fromNamespace)
        {
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlLite;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Sqlite.</para>
        /// <para>The Sqlite database is stored in the Default
        /// BlackHole DataPath. You can only choose the file name here.</para>
        /// <para>If you need to move it elsewhere you have to use 'SetDataPath()'</para>
        /// </summary>
        /// <param name="databaseName">Just the name of the database</param>
        /// <param name="quotedDb">Use quotes for Naming</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseSqlite(string databaseName, string fromNamespace, bool quotedDb)
        {
            ConnectionString = databaseName;
            ConnectionType = BlackHoleSqlTypes.SqlLite;
            UseQuotedDb = quotedDb;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Oracle database.</para>
        /// <para>BlackHole can not setup an oracle database
        /// on your system.</para>
        /// <para>Make sure to install the database first and
        /// then this library will create the tables.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseOracle(string connectionString, string fromNamespace)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Oracle;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Microsoft Sql Server.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <param name="schema">The name of the schema</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseSqlServer(string connectionString, Assembly fromAssembly)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Microsoft Sql Server.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <param name="schema">The name of the schema</param>
        /// <param name="quotedDb">Use quotes for Naming</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseSqlServer(string connectionString, Assembly fromAssembly, bool quotedDb)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.SqlServer;
            UseQuotedDb = quotedDb;
            return additionalSettings;
        }

        /// <summary>
        /// <para>Use the data provider for Postgresql.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <param name="schema">The name of the schema</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseNpgSql(string connectionString, Assembly fromAssembly)
        {
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }
    }
}
