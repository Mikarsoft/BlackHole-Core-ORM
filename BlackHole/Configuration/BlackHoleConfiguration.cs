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
        /// <para>Generates a Database , based on the inserted connection string, to an
        /// Existing Database Server.</para><para>The connection string Must lead to the server and the 
        /// Database part of the connection string will be used to create the database.</para>
        /// <para>It uses the BlackHole Entities and Services of the Calling Assembly or
        /// other Assemblies depending on the settings.</para>
        /// <para>You can choose to use only specific namespaces for the Entities and the Services.</para>
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="settings">Black Hole Settings Class</param>
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

            bool useLogsCleaner = false;
            int daysToClean = 60;

            if (blackHoleSettings.directorySettings.UseLogsCleaner)
            {
                useLogsCleaner = true;
                daysToClean = blackHoleSettings.directorySettings.DaysForCleanUp;
            }

            ScanConnectionString(blackHoleSettings.connectionConfig.ConnectionType,
                blackHoleSettings.connectionConfig.ConnectionString, blackHoleSettings.directorySettings.DataPath,useLogsCleaner,daysToClean);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();

            bool dbExists = databaseBuilder.CreateDatabase();

            if (dbExists)
            {
                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));
                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));
                services.AddScoped(typeof(IBHConnection), typeof(BHConnection));
                services.AddServicesAndTables(blackHoleSettings.connectionConfig.additionalSettings, assembly);
            }
            else
            {
                throw new Exception("The Host of the database is inaccessible...");
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

        internal static void ScanConnectionString(BlackHoleSqlTypes SqlType, string ConnectionString, string DataPath, bool useLogsCleaner , int daysToClean)
        {
            if(SqlType == BlackHoleSqlTypes.SqlLite)
            {
                ConnectionString = Path.Combine(DataPath, $"{ConnectionString}.db3");
            }

            DatabaseConfiguration.ScanConnectionString(ConnectionString, SqlType, DataPath,useLogsCleaner,daysToClean);
        }
    }
}
