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

            if (blackHoleSettings.dataPath == string.Empty)
            {
                blackHoleSettings.dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"BlackHoleData");
            }

            ScanConnectionString(blackHoleSettings.connectionConfig.ConnectionType,
                blackHoleSettings.connectionConfig.ConnectionString, blackHoleSettings.dataPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));
                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));
                services.AddScoped(typeof(IBHConnection), typeof(BHConnection));

                tableBuilder.BuildMultipleTables(ScanForEntities(blackHoleSettings.connectionConfig.additionalSettings.EntityNamespaces,
                    blackHoleSettings.connectionConfig.additionalSettings.AssembliesToUse, assembly));

                services.ScanForServices(blackHoleSettings.connectionConfig.additionalSettings.ServicesNamespaces,
                    blackHoleSettings.connectionConfig.additionalSettings.AssembliesToUse, assembly);
            }

            return services;
        }

        private static void ScanForServices( this IServiceCollection services,
            List<ServicesWithNamespace> servicesToUse, List<AssembliesUsed> assembliesToUse, Assembly callinAssembly)
        {
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            if (servicesToUse.Count > 0)
            {
                foreach (ServicesWithNamespace namespaceToUse in servicesToUse)
                {
                    if (namespaceToUse.AssemblyNameUsed != string.Empty)
                    {
                        AssembliesUsed? fromAssembly = assembliesToUse.Where(x => x.AssName == namespaceToUse.AssemblyNameUsed).FirstOrDefault();

                        if (fromAssembly != null && fromAssembly.ScanAssembly != null)
                        {
                            services.RegisterBHServicesByList(namespaceSelector.GetBHServicesInNamespace(namespaceToUse.NamespaceUsed, fromAssembly.ScanAssembly));
                        }
                        else
                        {
                            services.RegisterBHServicesByList(namespaceSelector.GetBHServicesInNamespace(namespaceToUse.NamespaceUsed, callinAssembly));
                        }
                    }
                    else
                    {
                        services.RegisterBHServicesByList(namespaceSelector.GetBHServicesInNamespace(namespaceToUse.NamespaceUsed, callinAssembly));
                    }
                }
            }
            else
            {
                services.RegisterBHServices(callinAssembly);
            }
        }

        private static List<Type> ScanForEntities(List<EntitiesWithNamespace> entitiesToUse, List<AssembliesUsed> assembliesToUse ,Assembly callinAssembly)
        {
            List<Type> Entities = new List<Type>();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            if (entitiesToUse.Count > 0)
            {
                foreach (EntitiesWithNamespace entity in entitiesToUse)
                {
                    if (entity.AssemblyNameUsed != string.Empty)
                    {
                        AssembliesUsed? fromAssembly = assembliesToUse.Where(x => x.AssName == entity.AssemblyNameUsed).FirstOrDefault();

                        if (fromAssembly != null && fromAssembly.ScanAssembly != null)
                        {
                            Entities.AddRange(namespaceSelector.GetBHEntitiesInNamespace(entity.NamespaceUsed, fromAssembly.ScanAssembly));
                        }
                        else
                        {
                            Entities.AddRange(namespaceSelector.GetBHEntitiesInNamespace(entity.NamespaceUsed, callinAssembly));
                        }
                    }
                    else
                    {
                        Entities.AddRange(namespaceSelector.GetBHEntitiesInNamespace(entity.NamespaceUsed, callinAssembly));
                    }
                }
            }
            else
            {
                Entities = namespaceSelector.GetAllBHEntities(callinAssembly);
            }

            return Entities;
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
