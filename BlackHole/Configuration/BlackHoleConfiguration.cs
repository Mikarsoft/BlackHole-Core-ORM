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
        /// <returns>IService Collection with BlackHole Services added</returns>
        public static IServiceCollection SuperNova(this IServiceCollection services, Action<BlackHoleSettings> settings)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            BlackHoleSettings blackHoleSettings = new();
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

            SetMode(blackHoleSettings.isInDevMode, blackHoleSettings.AutoUpdate);

            bool cliMode = BHCliCommandReader.ReadCliJson(assembly, blackHoleSettings.connectionConfig.ConnectionString);
            if (cliMode)
            {
                useLogsCleaner = false;
                blackHoleSettings.directorySettings.UseLogger = true;
                blackHoleSettings.connectionConfig.additionalSettings.ConnectionTimeOut = 300;
            }

            ScanConnectionString(blackHoleSettings.connectionConfig.ConnectionType, blackHoleSettings.connectionConfig.ConnectionString,
                blackHoleSettings.directorySettings.DataPath, blackHoleSettings.connectionConfig.TableSchema,
                blackHoleSettings.connectionConfig.additionalSettings.ConnectionTimeOut,
                blackHoleSettings.connectionConfig.UseQuotedDb);

            DataPathAndLogs(blackHoleSettings.directorySettings.DataPath, useLogsCleaner, daysToClean, blackHoleSettings.directorySettings.UseLogger);
            CliCommandSettings cliSettings = BHCliCommandReader.GetCliCommandSettings();

            int exitCode = 0;

            switch (cliSettings.commandType)
            {
                case CliCommandTypes.Update:
                    exitCode = BuildOrUpdateDatabaseCliProcess(blackHoleSettings.connectionConfig.additionalSettings, assembly);
                    break;
                case CliCommandTypes.Drop:
                    exitCode = DropDatabaseCliProcess();
                    break;
                case CliCommandTypes.Parse:
                    exitCode = ParseDatabaseCliProcess();
                    break;
                case CliCommandTypes.Default:
                    services.BuildDatabaseAndServices(blackHoleSettings.connectionConfig.additionalSettings, assembly);
                    break;
            }

            if (cliMode)
            {
                Environment.Exit(exitCode);
            }
            return services;
        }

        private static int BuildOrUpdateDatabaseCliProcess(ConnectionAdditionalSettings additionalSettings, Assembly callingAssembly)
        {
            BHDatabaseBuilder databaseBuilder = new();
            bool dbExists = databaseBuilder.CreateDatabase();
            databaseBuilder.CreateDatabaseSchema();
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

        private static void BuildDatabaseAndServices(this IServiceCollection services, ConnectionAdditionalSettings additionalSettings, Assembly callingAssembly)
        {
            BHDatabaseBuilder databaseBuilder = new();

            bool dbExists = databaseBuilder.CreateDatabase();
            databaseBuilder.CreateDatabaseSchema();

            if (dbExists)
            {
                services.AddScoped(typeof(IBHDataProvider<,>), typeof(BHDataProvider<,>));
                services.AddScoped(typeof(IBHOpenDataProvider<>), typeof(BHOpenDataProvider<>));
                services.AddScoped(typeof(IBHViewStorage), typeof(BHViewStorage));
                services.AddScoped(typeof(IBHConnection), typeof(BHConnection));
                services.AddServicesAndTables(additionalSettings, callingAssembly, databaseBuilder);
            }
            else
            {
                throw new Exception("The Host of the database is inaccessible... If you are using Oracle database,make sure to grand permission to your User on the v$instance table. Connect as Sysdba and execute the command => 'grant select on v_$instance to {Username};'");
            }
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

                    if (databaseBuilder.IsCreatedFirstTime())
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
                    }
                }

                foreach(Assembly assembly in additionalSettings.AssembliesToUse)
                {
                    services.RegisterBHServices(assembly);
                    tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(assembly), namespaceSelector.GetOpenAllBHEntities(assembly));

                    if (databaseBuilder.IsCreatedFirstTime())
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

                    if (databaseBuilder.IsCreatedFirstTime())
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(assembliesToUse.ScanAssembly));
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
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetBHEntitiesInNamespaces(entityNamespaces, callingAssembly),
                            namespaceSelector.GetOpenBHEntitiesInNamespaces(entityNamespaces, callingAssembly));
                    }
                    else
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly),
                            namespaceSelector.GetOpenAllBHEntities(callingAssembly));
                    }

                    if (databaseBuilder.IsCreatedFirstTime())
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
                    }
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

                    if (databaseBuilder.IsCreatedFirstTime())
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
                    }
                }

                foreach (Assembly assembly in additionalSettings.AssembliesToUse)
                {
                    tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(assembly), namespaceSelector.GetOpenAllBHEntities(assembly));

                    if (databaseBuilder.IsCreatedFirstTime())
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(assembly));
                    }
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

                    if (databaseBuilder.IsCreatedFirstTime())
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(assembliesToUse.ScanAssembly));
                    }
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

                    if (databaseBuilder.IsCreatedFirstTime())
                    {
                        dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
                    }
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

            ScanConnectionString(blackHoleSettings.ConnectionType, blackHoleSettings.ConnectionString,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlackHoleData"),
                blackHoleSettings.TableSchema,
                blackHoleSettings.additionalSettings.ConnectionTimeOut,
                blackHoleSettings.UseQuotedDb);

            return UpdateManually(blackHoleSettings.additionalSettings, assembly);
        }

        internal static bool UpdateManually(ConnectionAdditionalSettings additionalSettings, Assembly callingAssembly)
        {
            BHDatabaseBuilder databaseBuilder = new();
            bool dbExists = databaseBuilder.CreateDatabase();
            databaseBuilder.CreateDatabaseSchema();
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
            ScanConnectionString(blackHoleSettings.ConnectionType, blackHoleSettings.ConnectionString,
                dataPath, blackHoleSettings.TableSchema,
                blackHoleSettings.additionalSettings.ConnectionTimeOut,
                blackHoleSettings.UseQuotedDb);

            return UpdateManually(blackHoleSettings.additionalSettings, assembly);
        }

        /// <summary>
        /// Checks the database's condition
        /// </summary>
        /// <returns>Database is Up</returns>
        public static bool TestDatabase()
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DoesDbExists();
        }

        /// <summary>
        /// Closes all connections and drops the database. Works only in Developer Mode
        /// </summary>
        /// <returns>Success</returns>
        public static bool DropDatabase()
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DropDatabase();
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

        internal static void ScanConnectionString(BlackHoleSqlTypes SqlType, string ConnectionString, string DataPath, string databaseSchema, int timoutSeconds, bool isQuoted)
        {
            if(SqlType == BlackHoleSqlTypes.SqlLite)
            {
                ConnectionString = Path.Combine(DataPath, $"{ConnectionString}.db3");
            }

            DatabaseConfiguration.ScanConnectionString(ConnectionString, SqlType, databaseSchema, timoutSeconds,isQuoted);
        }

        internal static void DataPathAndLogs(string DataPath, bool useLogsCleaner, int daysToClean, bool useLogger)
        {
            DatabaseConfiguration.LogsSettings(DataPath, useLogsCleaner, daysToClean, useLogger);
        }
    }
}
