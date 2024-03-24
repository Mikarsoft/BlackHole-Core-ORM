using BlackHole.Enums;
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// Basic settings to connect to a database server
    /// </summary>
    public class ConnectionSettings
    {
        internal string ConnectionString { get; set; } = string.Empty;
        internal string TableSchema { get; set; } = string.Empty;
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal ConnectionAdditionalSettings additionalSettings { get; set; } = new();
        internal bool UseQuotedDb { get; set; } = false;

        /// <summary>
        /// <para>Use the data provider for Microsoft Sql Server.</para>
        /// <para>Do not use the Name of an Existing Database.</para>
        /// <para>BlackHole is going to create the database, based on the
        /// connection string.</para>
        /// </summary>
        /// <param name="connectionString">connection string to the database</param>
        /// <returns>Additional Settings</returns>
        public ConnectionAdditionalSettings UseSqlServer(string connectionString)
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
        public ConnectionAdditionalSettings UseSqlServer(string connectionString, bool quotedDb)
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
        public ConnectionAdditionalSettings UseNpgSql(string connectionString)
        {
            UseQuotedDb = true;
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
        public ConnectionAdditionalSettings UseMySql(string connectionString)
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
        public ConnectionAdditionalSettings UseSqlite(string databaseName)
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
        public ConnectionAdditionalSettings UseSqlite(string databaseName, bool quotedDb)
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
        public ConnectionAdditionalSettings UseOracle(string connectionString)
        {
            UseQuotedDb = true;
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
        public ConnectionAdditionalSettings UseSqlServer(string connectionString, string schema)
        {
            ConnectionString = connectionString;
            TableSchema = schema;
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
        public ConnectionAdditionalSettings UseSqlServer(string connectionString, string schema, bool quotedDb)
        {
            ConnectionString = connectionString;
            TableSchema = schema;
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
        public ConnectionAdditionalSettings UseNpgSql(string connectionString, string schema)
        {
            UseQuotedDb = true;
            ConnectionString = connectionString;
            TableSchema = schema;
            ConnectionType = BlackHoleSqlTypes.Postgres;
            return additionalSettings;
        }
    }

    /// <summary>
    /// Additional configuration object 
    /// for the Namespaces and the Assemblies that
    /// will be used by the BlackHole.
    /// </summary>
    public class ConnectionAdditionalSettings
    {
        internal EntitiesWithNamespace? EntityNamespaces;
        internal ServicesWithNamespace? ServicesNamespaces;

        internal List<Assembly> AssembliesToUse { get; set; } = new List<Assembly>();
        internal bool useCallingAssembly { get; set; } = true;
        internal int ConnectionTimeOut { get; set; } = 60;

        /// <summary>
        /// Change the Timeout of each command.The Default timeout of BlackHole is 60s.
        /// <para>This Feature does not apply to SqLite database</para>
        /// </summary>
        /// <param name="timoutInSeconds">The timtout in seconds that will be applied to each command</param>
        /// <returns>Connection Additional Settings</returns>
        public ConnectionAdditionalSettings SetConnectionTimeoutSeconds(int timoutInSeconds)
        {
            ConnectionTimeOut = timoutInSeconds;

            if (timoutInSeconds < 30)
            {
                ConnectionTimeOut = 30;
            }

            return this;
        }

        /// <summary>
        /// Using only the Entities that are in the specified 
        /// Namespace.
        /// </summary>
        /// <param name="entityNamespace">Namespace Full Name 'MyProject.Entities....'</param>
        /// <returns>Services Settings</returns>
        public ServicesWithNamespace UseEntitiesInNamespace(string entityNamespace)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            EntityNamespaces.EntitiesNamespaces.Add(entityNamespace);

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        /// <summary>
        /// Using only the Entities in the specified Namespaces.
        /// </summary>
        /// <param name="entityNamespaces">List of Namespaces Full Names</param>
        /// <returns>Services Settings</returns>
        public ServicesWithNamespace UseEntitiesInNamespaces(List<string> entityNamespaces)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            EntityNamespaces.EntitiesNamespaces = entityNamespaces;

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        /// <summary>
        /// Using only the Entities in the specified Namespaces.
        /// </summary>
        /// <param name="entityNamespaces">List of Namespaces Full Names</param>
        /// <returns>Services Settings</returns>
        public ServicesWithNamespace UseEntitiesInNamespaces(Action<List<string>> entityNamespaces)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            entityNamespaces.Invoke(EntityNamespaces.EntitiesNamespaces);

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        /// <summary>
        /// Using only the services that are in the
        /// specified Namespace and inherit from BlackHole Services Classes.
        /// </summary>
        /// <param name="servicesNamespace">Namespace Full Name 'MyProject.Services....'</param>
        /// <returns>Entities Settings</returns>
        public EntitiesWithNamespace AddServicesFromNamespace(string servicesNamespace)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            ServicesNamespaces.ServicesNamespaces.Add(servicesNamespace);

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        /// <summary>
        /// Using only the services that are in the
        /// specified Namespaces and inherit from BlackHole Services Classes.
        /// </summary>
        /// <param name="servicesNamespaces">List of Namespaces Full Names</param>
        /// <returns>Entities Settings</returns>
        public EntitiesWithNamespace AddServicesFromNamespaces(List<string> servicesNamespaces)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            ServicesNamespaces.ServicesNamespaces = servicesNamespaces;

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        /// <summary>
        /// Using only the services that are in the
        /// specified Namespaces and inherit from BlackHole Services Classes.
        /// </summary>
        /// <param name="servicesNamespaces">List of Namespaces Full Names</param>
        /// <returns>Entities Settings</returns>
        public EntitiesWithNamespace AddServicesFromNamespaces(Action<List<string>> servicesNamespaces)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            servicesNamespaces.Invoke(ServicesNamespaces.ServicesNamespaces);

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services ONLY from the 
        /// specified Assembly
        /// </summary>
        /// <param name="otherAssembly">Full Assembly</param>
        public void UseOtherAssembly(Assembly otherAssembly)
        {
            AssembliesToUse.Add(otherAssembly);
            useCallingAssembly = false;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services ONLY from the 
        /// specified Assemblies
        /// </summary>
        /// <param name="otherAssemblies">List of Assemblies</param>
        public void UseOtherAssemblies(List<Assembly> otherAssemblies)
        {
            AssembliesToUse = otherAssemblies;
            useCallingAssembly = false;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services ONLY from the 
        /// specified Assemblies
        /// </summary>
        /// <param name="otherAssemblies">List of Assemblies</param>
        public void UseOtherAssemblies(Action<List<Assembly>> otherAssemblies)
        {
            otherAssemblies.Invoke(AssembliesToUse);
            useCallingAssembly = false;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services from the 
        /// specified Assembly and the Calling Assembly
        /// </summary>
        /// <param name="additionalAssembly">Full Assembly</param>
        public void UseAdditionalAssembly(Assembly additionalAssembly)
        {
            AssembliesToUse.Add(additionalAssembly);
            useCallingAssembly = true;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services from the 
        /// specified Assemblies and the Calling Assembly
        /// </summary>
        /// <param name="additionalAssemblies">List of Assemblies</param>
        public void UseAdditionalAssemblies(List<Assembly> additionalAssemblies)
        {
            AssembliesToUse = additionalAssemblies;
            useCallingAssembly = true;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services from the 
        /// specified Assemblies and the Calling Assembly
        /// </summary>
        /// <param name="additionalAssemblies">List of Assemblies</param>
        public void UseAdditionalAssemblies(Action<List<Assembly>> additionalAssemblies)
        {
            additionalAssemblies.Invoke(AssembliesToUse);
            useCallingAssembly = true;
        }
    }
}
