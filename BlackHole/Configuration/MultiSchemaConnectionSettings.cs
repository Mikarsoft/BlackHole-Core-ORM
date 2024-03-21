using BlackHole.Enums;
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiSchemaConnectionSettings
    {
        internal MultiSchemaSettings additionalSettings = new();

        internal string ConnectionString { get; set; } = string.Empty;
        internal string TableSchema { get; set; } = string.Empty;
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal bool UseQuotedDb { get; set; } = false;

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
        /// <para>Use the data provider for PostgresSql.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Additional Settings</returns>
        public MultiSchemaSettings UseNpgSql(string connectionString)
        {
            UseQuotedDb = true;
            ConnectionString = connectionString;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MultiSchemaSettings
    {
        internal List<Assembly> AssembliesToUse { get; set; } = new List<Assembly>();
        internal bool SeparateSchemaByNamespaceInAssembly { get; set; } = false;
        internal bool SeparateSchemaByAssembly { get; set; } = false;
        internal bool SeparateSchemaByNamespace { get; set; } = false;
        internal bool SchemaSeparationSelected { get; set; } = false;
        internal int ConnectionTimeOut { get; set; } = 60;

        internal bool IncludeCallingAssembly { get; set; } = true;
        internal MultiServicesSettings ServicesSettings { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembliesToUse"></param>
        /// <param name="includeCallingAssembly"></param>
        public void SeparateByAssemblies(Action<List<Assembly>> assembliesToUse, bool includeCallingAssembly)
        {
            SchemaSeparationSelected = true;
            assembliesToUse.Invoke(AssembliesToUse);
            SeparateSchemaByAssembly = true;
            IncludeCallingAssembly = includeCallingAssembly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembliesToUse"></param>
        /// <param name="includeCallingAssembly"></param>
        public void SeparateByAssemblies(List<Assembly> assembliesToUse, bool includeCallingAssembly)
        {
            SchemaSeparationSelected = true;
            AssembliesToUse = assembliesToUse;
            SeparateSchemaByAssembly = true;
            IncludeCallingAssembly = includeCallingAssembly;
        }

        /// <summary>
        /// 
        /// </summary>
        public MultiServicesSettings SeparateByNamespaces()
        {
            SchemaSeparationSelected = true;
            SeparateSchemaByNamespace = true;
            return ServicesSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ass"></param>
        public MultiServicesSettings SeparateByNamespaceInOtherAssembly(Assembly ass)
        {
            SchemaSeparationSelected = true;
            AssembliesToUse.Add(ass);
            SeparateSchemaByNamespaceInAssembly = true;
            IncludeCallingAssembly = false;
            return ServicesSettings;
        }

        /// <summary>
        /// Change the Timeout of each command.The Default timeout of BlackHole is 60s.
        /// <para>This Feature does not apply to SqLite database</para>
        /// </summary>
        /// <param name="timeoutInSeconds">The timeout in seconds that will be applied to each command</param>
        /// <returns>Connection Additional Settings</returns>
        public MultiSchemaSettings SetConnectionTimeoutSeconds(int timeoutInSeconds)
        {
            ConnectionTimeOut = timeoutInSeconds;
            return this;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MultiServicesSettings
    {
        internal List<string> ServicesNamespaces = new List<string>();
        internal Assembly? AssemblyToUse { get; set; }
        internal bool UseDefaultProjectServices { get; set; } = true;

        /// <summary>
        /// Using only the services that are in the
        /// specified Namespace and inherit from BlackHole Services Classes.
        /// </summary>
        /// <param name="servicesNamespace"></param>
        /// <returns></returns>
        public void AddServicesFromNamespace(string servicesNamespace)
        {
            ServicesNamespaces.Add(servicesNamespace);
            UseDefaultProjectServices = false;
        }

        /// <summary>
        /// Using only the services that are in the
        /// specified Namespaces and inherit from BlackHole Services Classes.
        /// </summary>
        /// <param name="servicesNamespaces"></param>
        /// <returns></returns>
        public void AddServicesFromNamespaces(List<string> servicesNamespaces)
        {
            ServicesNamespaces = servicesNamespaces;
            UseDefaultProjectServices = false;
        }

        /// <summary>
        /// Scans a specified assembly for BlackHole Entities and Services
        /// and uses only them.
        /// </summary>
        /// <param name="otherAssembly">Full Assembly</param>
        public void AddServicesFromOtherAssembly(Assembly otherAssembly)
        {
            AssemblyToUse = otherAssembly;
            UseDefaultProjectServices = false;
        }
    }
}
