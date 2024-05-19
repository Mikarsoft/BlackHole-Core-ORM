using BlackHole.Configuration.ConfigTypes;
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
        public static IServiceCollection SupaNova(this IServiceCollection sv, Action<BlackHoleRootConfig> config)
        {
            return sv;
        }

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
        /// <returns>IService Collection with BlackHole Services added</returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, Action<BlackHoleSettings> settings)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            BlackHoleSettings blackHoleSettings = new();
            settings.Invoke(blackHoleSettings);

            if (blackHoleSettings.DirectorySettings.DataPath == string.Empty)
            {
                blackHoleSettings.DirectorySettings.DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"BlackHoleData");
            }
            
            bool useLogsCleaner = false;
            int daysToClean = 60;

            if (blackHoleSettings.DirectorySettings.UseLogsCleaner)
            {
                useLogsCleaner = true;
                daysToClean = blackHoleSettings.DirectorySettings.DaysForCleanUp;
            }

            SetMode(blackHoleSettings.IsInDevMode, blackHoleSettings.AutoUpdate);

            bool cliMode = BHCliCommandReader.ReadCliJson(assembly, blackHoleSettings.ConnectionConfig.ConnectionString);

            if (!blackHoleSettings.ValidateSettings(assembly.FullName))
            {
                if (cliMode)
                {
                    Console.WriteLine("_bhLog_ \t The settings are incorrect. Please make sure that you are using Each Assembly and Each Namespace, only once. \n" +
                        $"Errors: {blackHoleSettings.ValidationErrors}");

                    Environment.Exit(510);
                }
                else
                {
                    throw new Exception("The settings are incorrect. Please make sure that you are using Each Assembly and Each Namespace, only once." +
                                    $"Errors: {blackHoleSettings.ValidationErrors}");
                }
            }

            if (cliMode)
            {
                useLogsCleaner = false;
                blackHoleSettings.DirectorySettings.UseLogger = true;
            }

            DataPathAndLogs(blackHoleSettings.DirectorySettings.DataPath, useLogsCleaner, daysToClean, blackHoleSettings.DirectorySettings.UseLogger);

            CliCommandSettings cliSettings = BHCliCommandReader.GetCliCommandSettings();

            int exitCode = 0;
            switch (cliSettings.commandType)
            {
                case CliCommandTypes.Update:
                    services.BuildDatabase(blackHoleSettings, assembly, true);
                    break;
                case CliCommandTypes.Drop:
                    exitCode = DropDatabaseCliProcess();
                    break;
                case CliCommandTypes.Parse:
                    exitCode = ParseDatabaseCliProcess();
                    break;
                case CliCommandTypes.Default:
                    services.BuildDatabase(blackHoleSettings, assembly, false);
                    break;
            }

            if (cliMode)
            {
                Environment.Exit(exitCode);
            }

            return services;
        }

        private static void BuildDatabase(this IServiceCollection services, BlackHoleSettings settings, Assembly callingAssembly, bool cliMode)
        {
            switch (settings.DatabaseConfig)
            {
                case BHMode.Single:
                    services.BuildSingleDatabase(settings.ConnectionConfig, callingAssembly, cliMode);
                    break;
                case BHMode.MultiSchema:
                    services.BuildMultiSchemaDatabases(settings.MultiSchemaConfig, cliMode);
                    break;
                case BHMode.Multiple:
                    services.BuildMultipleDatabases(settings.MultipleConnectionsConfig, callingAssembly, cliMode);
                    break;
                case BHMode.HighAvailability:
                    services.BuildHighAvailabilityDatabase(settings.HighAvailabilityConfig, cliMode);
                    break;
            }
        }

        private static void BuildSingleDatabase(this IServiceCollection services , ConnectionSettings singleSettings, Assembly callingAssembly, bool cliMode)
        {
            DatabaseInitializer initializer = new();
            BHDatabaseBuilder databaseBuilder = new();

            initializer.SetBHMode(BHMode.Single);
            initializer.InitializeProviders(1);

            if (cliMode)
            {
                if (singleSettings.additionalSettings.ConnectionTimeOut < 300)
                {
                    singleSettings.additionalSettings.ConnectionTimeOut = 300;
                }
            }

            initializer.AssignWormholeSettings(singleSettings.ConnectionString, singleSettings.ConnectionType,
                singleSettings.TableSchema, "mainDb", singleSettings.UseQuotedDb,
                singleSettings.additionalSettings.ConnectionTimeOut, 0);

            services.AddBaseServices();

            if (!databaseBuilder.CreateDatabase(0))
            {
                throw new Exception("The Host of the database is inaccessible..." +
                    " If you are using Oracle database,make sure to grand permission to your User on the v$instance table. " +
                    "Connect as Sysdba and execute the command => 'grant select on v_$instance to 'Username';'");
            }

            if (!databaseBuilder.CreateDatabaseSchema(0))
            {
                throw new Exception("The schema of the database could not be created");
            }

            services.AddServicesAndTables(singleSettings.additionalSettings, callingAssembly, databaseBuilder);
        }

        private static void BuildMultipleDatabases(this IServiceCollection services, List<MultiConnectionSettings> multiSettings, Assembly callingAssembly, bool cliMode)
        {
            DatabaseInitializer initializer = new();
            BHTableBuilder tableBuilder = new();
            BHNamespaceSelector namespaceSelector = new();
            BHInitialDataBuilder dataBuilder = new();
            BHDatabaseBuilder databaseBuilder = new();

            initializer.SetBHMode(BHMode.Multiple);
            initializer.InitializeProviders(multiSettings.Count);

            services.AddBaseServices();

            List<Type> entityTypes = new();
            List<Type> openEntityTypes = new();
            List<string> nSpaces = new();
            List<string> serviceNSpaces = new();
            List<Type> serviceTypes = new();

            for (int i = 0; i < multiSettings.Count; i++)
            {
                if (cliMode)
                {
                    if (multiSettings[i].additionalSettings.ConnectionTimeOut < 300)
                    {
                        multiSettings[i].additionalSettings.ConnectionTimeOut = 300;
                    }
                }

                initializer.AssignWormholeSettings(multiSettings[i].ConnectionString, multiSettings[i].ConnectionType,
                    multiSettings[i].TableSchema,multiSettings[i].DatabaseIdentity, multiSettings[i].UseQuotedDb,
                    multiSettings[i].additionalSettings.ConnectionTimeOut, i);

                if (!databaseBuilder.CreateDatabase(i))
                {
                    throw new Exception($"The Host of the database {multiSettings[i].DatabaseIdentity} is inaccessible..." +
                        " If you are using Oracle database,make sure to grand permission to your User on the v$instance table. " +
                        "Connect as Sysdba and execute the command => 'grant select on v_$instance to 'Username';'");
                }

                if (!databaseBuilder.CreateDatabaseSchema(i))
                {
                    throw new Exception($"The schema of the database {multiSettings[i].DatabaseIdentity} could not be created");
                }

                if (multiSettings[i].IsUsingAssembly && multiSettings[i].Ass is Assembly asbl)
                {
                    entityTypes.AddRange(namespaceSelector.GetAllBHEntities(asbl));
                    openEntityTypes.AddRange(namespaceSelector.GetOpenAllBHEntities(asbl));

                    if (multiSettings[i].additionalSettings.SamePathServices)
                    {
                        serviceTypes.AddRange(namespaceSelector.GetAllServices(asbl));
                    }

                    if (multiSettings[i].additionalSettings.ServicesInOtherNamespace)
                    {
                        serviceNSpaces.Add(multiSettings[i].additionalSettings.ServicesNamespace);
                        serviceTypes.AddRange(namespaceSelector.GetBHServicesInNamespaces(serviceNSpaces, asbl));
                    }
                }
                else
                {
                    nSpaces.Add(multiSettings[i].SelectedNamespace);
                    entityTypes.AddRange(namespaceSelector.GetBHEntitiesInNamespaces(nSpaces, callingAssembly));
                    openEntityTypes.AddRange(namespaceSelector.GetOpenBHEntitiesInNamespaces(nSpaces, callingAssembly));

                    if (multiSettings[i].additionalSettings.SamePathServices)
                    {
                        serviceTypes.AddRange(namespaceSelector.GetBHServicesInNamespaces(nSpaces, callingAssembly));
                    }

                    if (multiSettings[i].additionalSettings.ServicesInOtherNamespace)
                    {
                        serviceNSpaces.Add(multiSettings[i].additionalSettings.ServicesNamespace);
                        serviceTypes.AddRange(namespaceSelector.GetBHServicesInNamespaces(serviceNSpaces, callingAssembly));
                    }
                }

                if (multiSettings[i].additionalSettings.ServicesInOtherAssembly && multiSettings[i].additionalSettings.ServicesAssembly is Assembly srvAsbl)
                {
                    serviceTypes.AddRange(namespaceSelector.GetAllServices(srvAsbl));
                }

                services.RegisterBHServicesByList(serviceTypes);
                tableBuilder.SwitchConnection(i);
                tableBuilder.BuildMultipleTables(entityTypes, openEntityTypes);
                tableBuilder.CleanupConstraints();

                entityTypes.Clear();
                openEntityTypes.Clear();
                serviceTypes.Clear();
                nSpaces.Clear();
                serviceNSpaces.Clear();
            }
        }

        private static void BuildMultiSchemaDatabases(this IServiceCollection services, MultiSchemaConnectionSettings multiSchemaSettings, bool cliMode)
        {
            DatabaseInitializer initializer = new();
            BHTableBuilder tableBuilder = new();
            BHNamespaceSelector namespaceSelector = new();
            BHInitialDataBuilder dataBuilder = new();
            BHDatabaseBuilder databaseBuilder = new();

            initializer.SetBHMode(BHMode.MultiSchema);
            initializer.InitializeProviders(1);
        }

        private static void BuildHighAvailabilityDatabase(this IServiceCollection services, HighAvailabilityConnectionSettings haSettings, bool cliMode)
        {
            DatabaseInitializer initializer = new();

            initializer.SetBHMode(BHMode.HighAvailability);
            initializer.InitializeProviders(2);
        }

        private static int BuildOrUpdateDatabaseCliProcess(ConnectionAdditionalSettings additionalSettings, Assembly callingAssembly, int connectionIndex)
        {
            BHDatabaseBuilder databaseBuilder = new();
            bool dbExists = databaseBuilder.CreateDatabase(connectionIndex);
            databaseBuilder.CreateDatabaseSchema(connectionIndex);
            if (dbExists)
            {
                Console.WriteLine("_bhLog_ \t The database is ready.");
                Console.WriteLine("_bhLog_ \t Creating or Updating the tables..");
                CreateOrUpdateTables(additionalSettings, callingAssembly, databaseBuilder);
                Console.WriteLine("_bhLog_");
                return 0;
            }
            else
            {
                Console.WriteLine("_bhLog_ \t An error occured while creating the database.");
                return 508;
            }
        }

        private static int ParseDatabaseCliProcess()
        {
            BHDatabaseParser parser = new(); 
            return parser.ParseDatabase();
        }

        private static int DropDatabaseCliProcess()
        {
            if (DropDatabase())
            {
                Console.WriteLine("_bhLog_ \t Database was successfully dropped.");
                Console.WriteLine("_bhLog_");
                return 0;
            }
            else
            {
                Console.WriteLine("_bhLog_ \t There was a problem with database drop.");
                Console.WriteLine("_bhLog_");
                return 307;
            }
        }

        private static void AddBaseServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));
            services.AddScoped(typeof(IBHOpenDataProvider<>), typeof(BHOpenDataProvider<>));
            services.AddScoped(typeof(IBHViews), typeof(BHViews));
        }

        private static void AddServicesAndTables(this IServiceCollection services, ConnectionAdditionalSettings additionalSettings, Assembly callingAssembly, BHDatabaseBuilder databaseBuilder)
        {
            BHTableBuilder tableBuilder = new();
            BHNamespaceSelector namespaceSelector = new();
            BHInitialDataBuilder dataBuilder = new();

            if (additionalSettings.AssembliesToUse.Count > 0)
            {
                if (additionalSettings.useCallingAssembly)
                {
                    services.RegisterBHServices(callingAssembly);
                    tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly),namespaceSelector.GetOpenAllBHEntities(callingAssembly));

                    if (databaseBuilder.IsCreatedFirstTime(0))
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
                    }
                }

                foreach(Assembly assembly in additionalSettings.AssembliesToUse)
                {
                    services.RegisterBHServices(assembly);
                    tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(assembly), namespaceSelector.GetOpenAllBHEntities(assembly));

                    if (databaseBuilder.IsCreatedFirstTime(0))
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(assembly));
                    }
                }
            }
            else
            {
                List<string> serviceNamespaces = new();
                List<string> entityNamespaces = new();

                AssembliesUsed assembliesToUse = new();

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
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetBHEntitiesInNamespaces(entityNamespaces, assembliesToUse.ScanAssembly),
                            namespaceSelector.GetOpenBHEntitiesInNamespaces(entityNamespaces, assembliesToUse.ScanAssembly));
                    }
                    else
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(assembliesToUse.ScanAssembly),
                            namespaceSelector.GetOpenAllBHEntities(assembliesToUse.ScanAssembly));
                    }

                    //if (databaseBuilder.IsCreatedFirstTime())
                    //{
                    //    dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(assembliesToUse.ScanAssembly));
                    //}
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
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetBHEntitiesInNamespaces(entityNamespaces, callingAssembly),
                            namespaceSelector.GetOpenBHEntitiesInNamespaces(entityNamespaces, callingAssembly));
                    }
                    else
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly),
                            namespaceSelector.GetOpenAllBHEntities(callingAssembly));
                    }

                    //if (databaseBuilder.IsCreatedFirstTime())
                    //{
                    //    dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
                    //}
                }
            }

            tableBuilder.CleanupConstraints();
        }

        private static void CreateOrUpdateTables(ConnectionAdditionalSettings additionalSettings, Assembly callingAssembly, BHDatabaseBuilder databaseBuilder)
        {
            DatabaseConfiguration.SetAutoUpdateMode(true);
            BHTableBuilder tableBuilder = new();
            BHNamespaceSelector namespaceSelector = new();
            BHInitialDataBuilder dataBuilder = new();

            if (additionalSettings.AssembliesToUse.Count > 0)
            {
                if (additionalSettings.useCallingAssembly)
                {
                    tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly), namespaceSelector.GetOpenAllBHEntities(callingAssembly));

                    //if (databaseBuilder.IsCreatedFirstTime())
                    //{
                    //    dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
                    //}
                }

                foreach (Assembly assembly in additionalSettings.AssembliesToUse)
                {
                    tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(assembly), namespaceSelector.GetOpenAllBHEntities(assembly));

                    //if (databaseBuilder.IsCreatedFirstTime())
                    //{
                    //    dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(assembly));
                    //}
                }
            }
            else
            {
                List<string> entityNamespaces = new();
                AssembliesUsed assembliesToUse = new();

                if (additionalSettings.ServicesNamespaces != null)
                {
                    if (additionalSettings.ServicesNamespaces.AssemblyToUse != null)
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

                if (assembliesToUse.ScanAssembly != null)
                {
                    if (entityNamespaces.Count > 0)
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetBHEntitiesInNamespaces(entityNamespaces, assembliesToUse.ScanAssembly),
                            namespaceSelector.GetOpenBHEntitiesInNamespaces(entityNamespaces,assembliesToUse.ScanAssembly));
                    }
                    else
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(assembliesToUse.ScanAssembly),
                            namespaceSelector.GetOpenAllBHEntities(assembliesToUse.ScanAssembly));
                    }

                    //if (databaseBuilder.IsCreatedFirstTime())
                    //{
                    //    dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(assembliesToUse.ScanAssembly));
                    //}
                }
                else
                {
                    if (entityNamespaces.Count > 0)
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetBHEntitiesInNamespaces(entityNamespaces, callingAssembly),
                            namespaceSelector.GetOpenBHEntitiesInNamespaces(entityNamespaces,callingAssembly));
                    }
                    else
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly),
                            namespaceSelector.GetOpenAllBHEntities(callingAssembly));
                    }

                    //if (databaseBuilder.IsCreatedFirstTime())
                    //{
                    //    dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
                    //}
                }
            }
            tableBuilder.UpdateWithoutForceWarning();
            tableBuilder.CleanupConstraints();
        }

        /// <summary>
        /// Manually Update the database at any point
        /// </summary>
        /// <param name="connectionSettings">Connection Settings</param>
        /// <param name="isDevMode">Set Developer mode</param>
        /// <returns>Success</returns>
        public static bool UpdateDatabase(Action<ConnectionSettings> connectionSettings, bool isDevMode)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            ConnectionSettings blackHoleSettings = new();
            connectionSettings.Invoke(blackHoleSettings);

            DataPathAndLogs(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlackHoleData"), false, 60, true);
            DatabaseConfiguration.SetMode(isDevMode);

            //ScanConnectionString(blackHoleSettings.ConnectionType, blackHoleSettings.ConnectionString,
            //    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlackHoleData"),
            //    blackHoleSettings.TableSchema,
            //    blackHoleSettings.additionalSettings.ConnectionTimeOut,
            //    blackHoleSettings.UseQuotedDb);

            return UpdateManually(blackHoleSettings.additionalSettings, assembly);
        }

        internal static bool UpdateManually(ConnectionAdditionalSettings additionalSettings, Assembly callingAssembly)
        {
            BHDatabaseBuilder databaseBuilder = new();
            bool dbExists = databaseBuilder.CreateDatabase(0);
            databaseBuilder.CreateDatabaseSchema(0);
            if (dbExists)
            {
                CreateOrUpdateTables(additionalSettings, callingAssembly, databaseBuilder);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Manually Update the database at any point
        /// </summary>
        /// <param name="connectionSettings">Connection Settings</param>
        /// <param name="isDevMode">Set Developer mode</param>
        /// <param name="dataPath">Path for Logs and SqLite</param>
        /// <returns>Success</returns>
        public static bool UpdateDatabase(Action<ConnectionSettings> connectionSettings, bool isDevMode, string dataPath)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            ConnectionSettings blackHoleSettings = new();
            connectionSettings.Invoke(blackHoleSettings);
            DataPathAndLogs(dataPath, false, 1, true);
            DatabaseConfiguration.SetMode(isDevMode);
            //ScanConnectionString(blackHoleSettings.ConnectionType, blackHoleSettings.ConnectionString,
            //    dataPath, blackHoleSettings.TableSchema,
            //    blackHoleSettings.additionalSettings.ConnectionTimeOut,
            //    blackHoleSettings.UseQuotedDb);

            return UpdateManually(blackHoleSettings.additionalSettings, assembly);
        }

        /// <summary>
        /// Checks the database's condition
        /// </summary>
        /// <returns>Database is Up</returns>
        public static bool TestDatabase()
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DoesDbExists(0);
        }

        /// <summary>
        /// Closes all connections and drops the database. Works only in Developer Mode
        /// </summary>
        /// <returns>Success</returns>
        public static bool DropDatabase()
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DropDatabase(0);
        }

        /// <summary>
        /// Initialize BlackHole without Host.
        /// It is suggested to use this in Console and Desktop applications.
        /// <para><b>Important</b> => The automatic Dependency injection of BlackHole Interfaces and Services doesn't work with this method.
        /// You have to instanciate the Services in order to use them.</para>
        /// </summary>
        /// <param name="settings">Black Hole Settings Class</param>
        public static IServiceCollection SuperNova(Action<BlackHoleSettings> settings)
        {
            IServiceCollection newServices = new ServiceCollection();
            return newServices.SuperNova(settings);
        }

        internal static void SetMode(bool isDevMode, bool automaticUpdate)
        {
            DatabaseConfiguration.SetMode(isDevMode);
            DatabaseConfiguration.SetAutoUpdateMode(automaticUpdate);
        }

        internal static void ScanConnectionString(BlackHoleSqlTypes SqlType, string ConnectionString, string databaseSchema, int timoutSeconds, bool isQuoted, int connectionIndex)
        {
            DatabaseConfiguration.ScanConnectionString(ConnectionString, SqlType, databaseSchema, timoutSeconds,isQuoted, connectionIndex);
        }

        internal static void DataPathAndLogs(string DataPath, bool useLogsCleaner, int daysToClean, bool useLogger)
        {
            DatabaseConfiguration.LogsSettings(DataPath, useLogsCleaner, daysToClean, useLogger);
        }
    }
}
