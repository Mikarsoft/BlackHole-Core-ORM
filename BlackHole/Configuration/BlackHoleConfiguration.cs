using BlackHole.Core;
using BlackHole.Enums;
using BlackHole.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// All Black Hole Configuration
    /// </summary>
    public static class BlackHoleConfiguration
    {

        /// <summary>
        /// Generates a database , based on the inserted connection string, to an
        /// Existing Sql Server.The connection string Must lead to the server and the 
        /// Database part of the connection string will be used to create the database.
        /// It uses the BlackHole Entities and Services of the Calling Assembly.
        /// With Extta Config you can choose a specific namespace for the entities 
        /// and another one for the services that will be used.
        /// You can also choose if the services will be automatically or manually registered.
        /// If you don't specify these options , all entities and services in the assebly will
        /// be used automatically.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, Action<BlackHoleSettings> settings)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            BlackHoleSettings blackHoleSettings = new BlackHoleSettings();
            settings.Invoke(blackHoleSettings);

            if (blackHoleSettings.directorySettings.DataPath == string.Empty)
            {
                blackHoleSettings.directorySettings.DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"BlackHoleData");
            }

            ScanConnectionString(blackHoleSettings.connectionConfig.ConnectionType,
                blackHoleSettings.connectionConfig.ConnectionString, blackHoleSettings.directorySettings.DataPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));
                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));
                services.AddScoped(typeof(IBHConnection), typeof(BHConnection));
                services.AddServicesAndTables(blackHoleSettings.connectionConfig.additionalSettings, assembly);
            }

            return services;
        }

        private static void AddServicesAndTables(this IServiceCollection services, ConnectionAdditionalSettings additionalSettings, Assembly callingAssembly)
        {
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            if (additionalSettings.AssembliesToUse.Count > 0)
            {
                if (additionalSettings.useCallingAssembly)
                {
                    services.RegisterBHServices(callingAssembly);
                    tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly));
                }

                foreach(Assembly assembly in additionalSettings.AssembliesToUse)
                {
                    services.RegisterBHServices(assembly);
                    tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(assembly));
                }
            }
            else
            {
                List<string> serviceNamespaces = new List<string>();
                List<string> entityNamespaces = new List<string>();

                AssembliesUsed assembliesToUse = new AssembliesUsed();

                if(additionalSettings.ServicesNamespaces != null)
                {
                    serviceNamespaces = additionalSettings.ServicesNamespaces.ServicesNamespaces;

                    if(additionalSettings.ServicesNamespaces.AssemblyToUse != null)
                    {
                        assembliesToUse = additionalSettings.ServicesNamespaces.AssemblyToUse;
                    }
                }

                if (additionalSettings.EntityNamespaces != null)
                {
                    entityNamespaces = additionalSettings.EntityNamespaces.EntitiesNamespaces;

                    if (additionalSettings.EntityNamespaces.AssemblyToUse != null)
                    {
                        assembliesToUse = additionalSettings.EntityNamespaces.AssemblyToUse;
                    }
                }

                if(assembliesToUse.ScanAssembly != null)
                {
                    if (serviceNamespaces.Count > 0)
                    {
                        services.RegisterBHServicesByList(
                            namespaceSelector.GetBHServicesInNamespaces(serviceNamespaces, assembliesToUse.ScanAssembly));
                    }
                    else
                    {
                        services.RegisterBHServices(assembliesToUse.ScanAssembly);
                    }

                    if (entityNamespaces.Count > 0)
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetBHEntitiesInNamespaces(entityNamespaces, assembliesToUse.ScanAssembly));
                    }
                    else
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(assembliesToUse.ScanAssembly));
                    }
                }
                else
                {
                    if (serviceNamespaces.Count > 0)
                    {
                        services.RegisterBHServicesByList(
                                namespaceSelector.GetBHServicesInNamespaces(serviceNamespaces, callingAssembly));
                    }
                    else
                    {
                        services.RegisterBHServices(callingAssembly);
                    }

                    if (entityNamespaces.Count > 0)
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetBHEntitiesInNamespaces(entityNamespaces, callingAssembly));
                    }
                    else
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly));
                    }
                }
            }
        }

        /// <summary>
        /// Checks the database's condition
        /// </summary>
        /// <returns></returns>
        public static bool TestDatabase()
        {
            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            return databaseBuilder.DoesDbExists();
        }

        /// <summary>
        /// Closes all connections and drops the database
        /// </summary>
        /// <returns></returns>
        public static bool DropDatabase()
        {
            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            return databaseBuilder.DropDatabase();
        }

        internal static void ScanConnectionString(BlackHoleSqlTypes SqlType, string ConnectionString, string DataPath)
        {
            if(SqlType == BlackHoleSqlTypes.SqlLite)
            {
                ConnectionString = Path.Combine(DataPath, $"{ConnectionString}.db3");
            }

            DatabaseConfiguration.ScanConnectionString(ConnectionString, SqlType, DataPath);
        }
    }
}
