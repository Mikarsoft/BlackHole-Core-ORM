using BlackHole.Data;
using BlackHole.Enums;
using BlackHole.Interfaces;
using BlackHole.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlackHole.FunctionalObjects
{
    public static class BlackHoleConfiguration
    {
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
            ScanConnectionString(BHSqlTypes.SqlLite, dbPath, LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                var Entities = namespaceSelector.GetAllBlazarEntities(assembly);
                tableBuilder.BuildMultipleTables(Entities);

                services.AddScoped(typeof(IBlackHoleProvider<>), typeof(BlackHoleProvider<>));
                services.AddScoped(typeof(IBlackHoleProviderG<>), typeof(BlackHoleProviderG<>));
                services.AddScoped(typeof(IBlackHoleViewStorage), typeof(BlackHoleViewStorage));

                services.RegisterBlazarServices(assembly);
            }
            return services;
        }

        public static IServiceCollection SuperNova(this IServiceCollection services, BlackHoleBaseConfig settings)
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
                var Entities = namespaceSelector.GetAllBlazarEntities(assembly);
                tableBuilder.BuildMultipleTables(Entities);
                services.AddScoped(typeof(IBlackHoleProvider<>), typeof(BlackHoleProvider<>));
                services.AddScoped(typeof(IBlackHoleProviderG<>), typeof(BlackHoleProviderG<>));
                services.AddScoped(typeof(IBlackHoleViewStorage), typeof(BlackHoleViewStorage));
                services.RegisterBlazarServices(assembly);
            }
            return services;
        }

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
                    var Entities = namespaceSelector.GetBlazarEntitiesInNamespace(settings.SpecificEntityNamespace,assembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }
                else
                {
                    var Entities = namespaceSelector.GetAllBlazarEntities(assembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }

                services.AddScoped(typeof(IBlackHoleProvider<>), typeof(BlackHoleProvider<>));
                services.AddScoped(typeof(IBlackHoleProviderG<>), typeof(BlackHoleProviderG<>));
                services.AddScoped(typeof(IBlackHoleViewStorage), typeof(BlackHoleViewStorage));

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespace != string.Empty)
                {
                    var BlazarServices = namespaceSelector.GetBlazarServicesInNamespace(settings.SpecificServicesNamespace,assembly);
                    services.RegisterBlazarServicesByList(BlazarServices);
                }

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespace == string.Empty)
                {
                    services.RegisterBlazarServices(assembly);
                }
            }
            return services;
        }

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
                    var Entities = namespaceSelector.GetBlazarEntitiesInNamespaces(settings.SpecificEntityNamespaces, assembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }
                else
                {
                    var Entities = namespaceSelector.GetAllBlazarEntities(assembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }

                services.AddScoped(typeof(IBlackHoleProvider<>), typeof(BlackHoleProvider<>));
                services.AddScoped(typeof(IBlackHoleProviderG<>), typeof(BlackHoleProviderG<>));
                services.AddScoped(typeof(IBlackHoleViewStorage), typeof(BlackHoleViewStorage));

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespaces.Count > 0)
                {
                    var BlazarServices = namespaceSelector.GetBlazarServicesInNamespaces(settings.SpecificServicesNamespaces, assembly);
                    services.RegisterBlazarServicesByList(BlazarServices);
                }

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespaces.Count == 0)
                {
                    services.RegisterBlazarServices(assembly);
                }
            }
            return services;
        }

        public static IServiceCollection SuperNovaLite(this IServiceCollection services, string databaseName, Assembly useOtherAssembly)
        {
            string defaultBlackHoleFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\BlackHole";

            if (!Directory.Exists(defaultBlackHoleFolder))
            {
                Directory.CreateDirectory(defaultBlackHoleFolder);
            }

            string dbPath = defaultBlackHoleFolder + $"\\{databaseName}.db3";
            string LogsPath = defaultBlackHoleFolder + "\\Logs";

            ScanConnectionString(BHSqlTypes.SqlLite, dbPath,LogsPath);

            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            IBHTableBuilder tableBuilder = new BHTableBuilder();
            IBHNamespaceSelector namespaceSelector = new BHNamespaceSelector();

            bool dbExists = databaseBuilder.CheckDatabaseExistance();

            if (dbExists)
            {
                var Entities = namespaceSelector.GetAllBlazarEntities(useOtherAssembly);
                tableBuilder.BuildMultipleTables(Entities);
                services.AddScoped(typeof(IBlackHoleProvider<>), typeof(BlackHoleProvider<>));
                services.AddScoped(typeof(IBlackHoleProviderG<>), typeof(BlackHoleProviderG<>));
                services.AddScoped(typeof(IBlackHoleViewStorage), typeof(BlackHoleViewStorage));
                services.RegisterBlazarServices(useOtherAssembly);
            }
            return services;
        }

        public static IServiceCollection SuperNova(this IServiceCollection services, BlackHoleBaseConfig settings, Assembly useOtherAssembly)
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
                var Entities = namespaceSelector.GetAllBlazarEntities(useOtherAssembly);
                tableBuilder.BuildMultipleTables(Entities);
                services.AddScoped(typeof(IBlackHoleProvider<>), typeof(BlackHoleProvider<>));
                services.AddScoped(typeof(IBlackHoleProviderG<>), typeof(BlackHoleProviderG<>));
                services.AddScoped(typeof(IBlackHoleViewStorage), typeof(BlackHoleViewStorage));
                services.RegisterBlazarServices(useOtherAssembly);
            }
            return services;
        }

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
                    var Entities = namespaceSelector.GetBlazarEntitiesInNamespace(settings.SpecificEntityNamespace, useOtherAssembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }
                else
                {
                    var Entities = namespaceSelector.GetAllBlazarEntities(useOtherAssembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }

                services.AddScoped(typeof(IBlackHoleProvider<>), typeof(BlackHoleProvider<>));
                services.AddScoped(typeof(IBlackHoleProviderG<>), typeof(BlackHoleProviderG<>));
                services.AddScoped(typeof(IBlackHoleViewStorage), typeof(BlackHoleViewStorage));

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespace != string.Empty)
                {
                    var BlazarServices = namespaceSelector.GetBlazarServicesInNamespace(settings.SpecificServicesNamespace, useOtherAssembly);
                    services.RegisterBlazarServicesByList(BlazarServices);
                }

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespace == string.Empty)
                {
                    services.RegisterBlazarServices(useOtherAssembly);
                }
            }
            return services;
        }

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
                    var Entities = namespaceSelector.GetBlazarEntitiesInNamespaces(settings.SpecificEntityNamespaces, useOtherAssembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }
                else
                {
                    var Entities = namespaceSelector.GetAllBlazarEntities(useOtherAssembly);
                    tableBuilder.BuildMultipleTables(Entities);
                }

                services.AddScoped(typeof(IBlackHoleProvider<>), typeof(BlackHoleProvider<>));
                services.AddScoped(typeof(IBlackHoleProviderG<>), typeof(BlackHoleProviderG<>));
                services.AddScoped(typeof(IBlackHoleViewStorage), typeof(BlackHoleViewStorage));

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespaces.Count > 0)
                {
                    var BlazarServices = namespaceSelector.GetBlazarServicesInNamespaces(settings.SpecificServicesNamespaces, useOtherAssembly);
                    services.RegisterBlazarServicesByList(BlazarServices);
                }

                if (settings.AutoRegisterBlazarServices && settings.SpecificServicesNamespaces.Count == 0)
                {
                    services.RegisterBlazarServices(useOtherAssembly);
                }
            }
            return services;
        }

        public static bool TestDatabase()
        {
            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            return databaseBuilder.DoesDbExists();
        }

        public static bool DropDatabase()
        {
            IBHDatabaseBuilder databaseBuilder = new BHDatabaseBuilder();
            return databaseBuilder.DropDatabase();
        }

        internal static void ScanConnectionString(BHSqlTypes SqlType, string ConnectionString, string LogsPath)
        {
            DatabaseConfiguration.ScanConnectionString(ConnectionString, SqlType,LogsPath);
        }
    }
}
