using BlackHole.Core;
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
        /// The easiest way to setup a single file standalone database
        /// Just insert the file name and it will be created in the 
        /// current user's folder.
        /// It uses the BlackHole Entities and Services of the Calling Assembly
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="databaseName">The filename of the SQLite database</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection SuperNovaLite(this IServiceCollection services, string databaseName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            string defaultBlackHoleFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole";

            if (!Directory.Exists(defaultBlackHoleFolder))
            {
                Directory.CreateDirectory(defaultBlackHoleFolder);
            }

            string dbPath = defaultBlackHoleFolder + $"\\{databaseName}.db3";
            string LogsPath = defaultBlackHoleFolder + "\\Logs";
            ScanConnectionString(BHSqlType.SqlLite, dbPath, LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                var Entities = namespaceSelector.GetAllBHEntities(assembly);
                tableBuilder.BuildMultipleTables(Entities);

                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));

                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));

                services.RegisterBHServices(assembly);
            }
            return services;
        }

        /// <summary>
        /// Generates a database , based on the inserted connection string, to an
        /// Existing Sql Server.The connection string Must lead to the server and the 
        /// Database part of the connection string will be used to create the database.
        /// It uses the BlackHole Entities and Services of the Calling Assembly
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="settings">Simple Configuration</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, BHConfig settings)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            //if (settings.LogsPath == string.Empty)
            //{
            //    settings.LogsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole\\Logs";
            //}

            //ScanConnectionString(settings.SqlType, settings.ConnectionString, settings.LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                var Entities = namespaceSelector.GetAllBHEntities(assembly);
                tableBuilder.BuildMultipleTables(Entities);
                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));
                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));
                services.RegisterBHServices(assembly);
            }
            return services;
        }

        public static IServiceCollection SuperNova(this IServiceCollection services, Action<BlackHoleSettings> settings)
        {
            BlackHoleSettings blackHoleSettings = new BlackHoleSettings();
            settings.Invoke(blackHoleSettings);
            Assembly assembly = Assembly.GetCallingAssembly();

            if (blackHoleSettings.logsPath == string.Empty)
            {
                blackHoleSettings.logsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole\\Logs";
            }

            //ScanConnectionString(stt..SqlType, settings.ConnectionString, settings.LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                var Entities = namespaceSelector.GetAllBHEntities(assembly);
                tableBuilder.BuildMultipleTables(Entities);
                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));
                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));
                services.RegisterBHServices(assembly);
            }
            return services;
        }

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
        /// <param name="services">IServiceCollection</param>
        /// <param name="settings">Extra Options Configuration</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, BlackHoleExtraConfig settings)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            if(settings.LogsPath == string.Empty)
            {
                settings.LogsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole\\Logs";
            }

            ScanConnectionString(settings.SqlType, settings.ConnectionString, settings.LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                if(settings.SpecificEntityNamespace != string.Empty)
                {
                    var Entities = namespaceSelector.GetBHEntitiesInNamespace(settings.SpecificEntityNamespace,assembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }
                else
                {
                    var Entities = namespaceSelector.GetAllBHEntities(assembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }

                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));

                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespace != string.Empty)
                {
                    var BlazarServices = namespaceSelector.GetBHServicesInNamespace(settings.SpecificServicesNamespace,assembly);
                    services.RegisterBHServicesByList(BlazarServices);
                }

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespace == string.Empty)
                {
                    services.RegisterBHServices(assembly);
                }
            }
            return services;
        }

        /// <summary>
        /// Generates a database , based on the inserted connection string, to an
        /// Existing Sql Server.The connection string Must lead to the server and the 
        /// Database part of the connection string will be used to create the database.
        /// It uses the BlackHole Entities and Services of the Calling Assembly
        /// With Advanced Config you can choose specific namespaces for the entities 
        /// and for the services that will be used.
        /// You can also choose if the services will be automatically or manually registered.
        /// If you don't specify these options , all entities and services in the assebly will
        /// be used automatically.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="settings">Advanced Options Configuration</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, BlackHoleAdvancedConfig settings)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            if (settings.LogsPath == string.Empty)
            {
                settings.LogsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole\\Logs";
            }

            ScanConnectionString(settings.SqlType, settings.ConnectionString, settings.LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                if (settings.SpecificEntityNamespaces.Count > 0)
                {
                    var Entities = namespaceSelector.GetBHEntitiesInNamespaces(settings.SpecificEntityNamespaces, assembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }
                else
                {
                    var Entities = namespaceSelector.GetAllBHEntities(assembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }

                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));

                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespaces.Count > 0)
                {
                    var BlazarServices = namespaceSelector.GetBHServicesInNamespaces(settings.SpecificServicesNamespaces, assembly);
                    services.RegisterBHServicesByList(BlazarServices);
                }

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespaces.Count == 0)
                {
                    services.RegisterBHServices(assembly);
                }
            }
            return services;
        }


        /// <summary>
        /// The easiest way to setup a single file standalone database
        /// Just insert the file name and it will be created in the 
        /// current user's folder.
        /// It uses the BlackHole Entities and Services of the Inserted Assembly
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="databaseName">The filename of the SQLite database</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection SuperNovaLite(this IServiceCollection services, string databaseName, Assembly useOtherAssembly)
        {
            string defaultBlackHoleFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole";

            if (!Directory.Exists(defaultBlackHoleFolder))
            {
                Directory.CreateDirectory(defaultBlackHoleFolder);
            }

            string dbPath = defaultBlackHoleFolder + $"\\{databaseName}.db3";
            string LogsPath = defaultBlackHoleFolder + "\\Logs";

            //ScanConnectionString(BHSqlTypes.SqlLite, dbPath,LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                var Entities = namespaceSelector.GetAllBHEntities(useOtherAssembly);
                tableBuilder.BuildMultipleTables(Entities);
                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));

                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));
                services.RegisterBHServices(useOtherAssembly);
            }
            return services;
        }


        /// <summary>
        /// Generates a database , based on the inserted connection string, to an
        /// Existing Sql Server.The connection string Must lead to the server and the 
        /// Database part of the connection string will be used to create the database.
        /// It uses the BlackHole Entities and Services of the Inserted Assembly
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="settings">Simple Configuration</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, BHConfig settings, Assembly useOtherAssembly)
        {
            //if (settings.LogsPath == string.Empty)
            //{
            //    settings.LogsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole\\Logs";
            //}

            //ScanConnectionString(settings.SqlType, settings.ConnectionString, settings.LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                var Entities = namespaceSelector.GetAllBHEntities(useOtherAssembly);
                tableBuilder.BuildMultipleTables(Entities);
                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));

                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));
                services.RegisterBHServices(useOtherAssembly);
            }
            return services;
        }


        /// <summary>
        /// Generates a database , based on the inserted connection string, to an
        /// Existing Sql Server.The connection string Must lead to the server and the 
        /// Database part of the connection string will be used to create the database.
        /// It uses the BlackHole Entities and Services of the Inserted Assembly.
        /// With Extta Config you can choose a specific namespace for the entities 
        /// and another one for the services that will be used.
        /// You can also choose if the services will be automatically or manually registered.
        /// If you don't specify these options , all entities and services in the assebly will
        /// be used automatically.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="settings">Extra Options Configuration</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, BlackHoleExtraConfig settings, Assembly useOtherAssembly)
        {
            if (settings.LogsPath == string.Empty)
            {
                settings.LogsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole\\Logs";
            }

            ScanConnectionString(settings.SqlType, settings.ConnectionString, settings.LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                if (settings.SpecificEntityNamespace != string.Empty)
                {
                    var Entities = namespaceSelector.GetBHEntitiesInNamespace(settings.SpecificEntityNamespace, useOtherAssembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }
                else
                {
                    var Entities = namespaceSelector.GetAllBHEntities(useOtherAssembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }

                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));

                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespace != string.Empty)
                {
                    var BlazarServices = namespaceSelector.GetBHServicesInNamespace(settings.SpecificServicesNamespace, useOtherAssembly);
                    services.RegisterBHServicesByList(BlazarServices);
                }

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespace == string.Empty)
                {
                    services.RegisterBHServices(useOtherAssembly);
                }
            }
            return services;
        }


        /// <summary>
        /// Generates a database , based on the inserted connection string, to an
        /// Existing Sql Server.The connection string Must lead to the server and the 
        /// Database part of the connection string will be used to create the database.
        /// It uses the BlackHole Entities and Services of the Inserted Assembly
        /// With Advanced Config you can choose specific namespaces for the entities 
        /// and for the services that will be used.
        /// You can also choose if the services will be automatically or manually registered.
        /// If you don't specify these options , all entities and services in the assebly will
        /// be used automatically.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="settings">Advanced Options Configuration</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, BlackHoleAdvancedConfig settings,Assembly useOtherAssembly)
        {
            if (settings.LogsPath == string.Empty)
            {
                settings.LogsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole\\Logs";
            }

            ScanConnectionString(settings.SqlType, settings.ConnectionString, settings.LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                if (settings.SpecificEntityNamespaces.Count > 0)
                {
                    var Entities = namespaceSelector.GetBHEntitiesInNamespaces(settings.SpecificEntityNamespaces, useOtherAssembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }
                else
                {
                    var Entities = namespaceSelector.GetAllBHEntities(useOtherAssembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }

                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));

                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespaces.Count > 0)
                {
                    var BlazarServices = namespaceSelector.GetBHServicesInNamespaces(settings.SpecificServicesNamespaces, useOtherAssembly);
                    services.RegisterBHServicesByList(BlazarServices);
                }

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespaces.Count == 0)
                {
                    services.RegisterBHServices(useOtherAssembly);
                }
            }
            return services;
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

        internal static void ScanConnectionString(BHSqlType SqlType, string ConnectionString, string LogsPath)
        {
            DatabaseConfiguration.ScanConnectionString(ConnectionString, SqlType,LogsPath);
        }
    }
}
