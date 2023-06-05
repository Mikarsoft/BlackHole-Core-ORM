using BlackHole.Core;
using BlackHole.CoreSupport;
using BlackHole.Logger;
using BlackHole.Statics;
using Microsoft.Data.Sqlite;

namespace BlackHole.Internal
{
    internal class BHDatabaseBuilder : IBHDatabaseBuilder
    {
        private readonly IBHDatabaseSelector _multiDatabaseSelector;
        private readonly ILoggerService _loggerService;
        private readonly IExecutionProvider connection;
        private string SchemaCreationCommand { get; set; } = string.Empty;

        private SqlExportWriter sqlWriter { get; set; }

        internal BHDatabaseBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            _loggerService = new LoggerService();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ServerConnection);
            sqlWriter = new SqlExportWriter("1_DatabaseSql");
        }

        /// <summary>
        /// Closes all open connections and drops the database
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseBuilder.DropDatabase()
        {
            string CheckDb = "";

            if (!DatabaseStatics.IsDevMove && !CliCommand.ForceAction)
            {
                if (CliCommand.CliExecution)
                {
                    Console.WriteLine("_bhLog_");
                    Console.WriteLine("_bhLog_ \t Warning : ", Console.ForegroundColor = ConsoleColor.Yellow);
                    Console.WriteLine("_bhLog_ \t BlackHole is not in Dev mode. If you want to drop the database,", Console.ForegroundColor = ConsoleColor.White);
                    Console.WriteLine("_bhLog_ \t either set BlackHole in Dev mode, or use the argument '-f' or '--force' after the drop command.");
                    Console.WriteLine("_bhLog_");
                    Console.WriteLine("_bhLog_ \t Example : 'bhl drop -f");
                }

                return false;
            }

            try
            {
                bool isLite = _multiDatabaseSelector.IsLite();

                if (isLite)
                {
                    string databaseLocation = _multiDatabaseSelector.GetServerConnection();

                    if (File.Exists(databaseLocation))
                    {

                        SqliteConnection.ClearPool(new SqliteConnection(DatabaseStatics.ConnectionString));
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        File.Delete(databaseLocation);
                    }

                    return true;
                }
                else
                {
                    string databaseName = _multiDatabaseSelector.GetDatabaseName();
                    bool dbExists = false;
                    string DropDb = $@"DROP DATABASE IF EXISTS ""{databaseName}""";

                    switch (_multiDatabaseSelector.GetSqlTypeId())
                    {
                        case 0:
                            CheckDb = $"select count(*) from master.dbo.sysdatabases where name='{databaseName}'";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            DropDb = $@"USE master;ALTER DATABASE ""{databaseName}"" SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE ""{databaseName}"";";
                            break;
                        case 1:
                            string DeleteMyShit = $"DROP DATABASE IF EXISTS {databaseName}";
                            connection.JustExecute(DeleteMyShit, null);
                            break;
                        case 2:
                            CheckDb = $"SELECT 1 FROM pg_database WHERE datname='{databaseName}';";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            DropDb = $@"UPDATE pg_database SET datallowconn = 'false' WHERE datname = '{databaseName}'; SELECT pg_terminate_backend(pg_stat_activity.pid)
                            FROM pg_stat_activity WHERE pg_stat_activity.datname = '{databaseName}'; DROP DATABASE ""{databaseName}""";
                            break;
                        case 4:
                            if(!string.IsNullOrWhiteSpace(databaseName))
                            {
                                CheckDb = $"SELECT table_name FROM all_tables WHERE owner = '{databaseName}'";
                                List<string> existingTables = connection.Query<string>(CheckDb, null);

                                foreach (string table in existingTables)
                                {
                                    connection.JustExecute($@"DROP TABLE ""{table}"" CASCADE CONSTRAINTS", null);
                                }
                            }
                            break;
                    }

                    if (CliCommand.ExportSql)
                    {
                        sqlWriter.DeleteSqlFolder();
                    }

                    if (dbExists)
                    {
                        return connection.JustExecute(DropDb, null);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", CheckDb, ex.Message, ex.ToString());

                if (CliCommand.CliExecution)
                {
                    Console.WriteLine("_bhLog_ Drop Database Error: "+ex.Message, Console.ForegroundColor = ConsoleColor.Red);
                    Console.WriteLine("_bhLog_", Console.ForegroundColor = ConsoleColor.White);
                }

                return false;
            }
        }

        /// <summary>
        /// Creates a new Database if it doesn't exist, in the specified Server or location
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseBuilder.CreateDatabase()
        {
            string CheckDb = "";

            try
            {
                bool isLite = _multiDatabaseSelector.IsLite();

                if (isLite)
                {
                    SchemaCreationCommand = "";
                    string databaseLocation = _multiDatabaseSelector.GetServerConnection();

                    if (!File.Exists(databaseLocation))
                    {
                        var stream = File.Create(databaseLocation);
                        stream.Dispose();
                    }
                    return true;
                }
                else
                {
                    string databaseName = _multiDatabaseSelector.GetDatabaseName();
                    bool dbExists = false;
                    string CreateDb = $@"CREATE DATABASE ""{databaseName}""";
                    bool isOracle = false;

                    switch (_multiDatabaseSelector.GetSqlTypeId())
                    {
                        case 0:
                            SchemaCreationCommand = "";
                            CheckDb = $"select count(*) from master.dbo.sysdatabases where name='{databaseName}'";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            break;
                        case 1:
                            SchemaCreationCommand = "";
                            CheckDb = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";
                            CreateDb = $"CREATE DATABASE IF NOT EXISTS {databaseName}";
                            string? dbName = connection.ExecuteScalar<string>(CheckDb, null);
                            dbExists = dbName?.ToLower() == databaseName.ToLower();
                            break;
                        case 2:
                            SchemaCreationCommand = "";
                            CheckDb = $"SELECT 1 FROM pg_database WHERE datname='{databaseName}';";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            break;
                        case 4:
                            SchemaCreationCommand = $"CREATE SCHEMA IF NOT EXISTS {DatabaseStatics.DatabaseSchema} AUTHORIZATION {databaseName}";
                            CheckDb = "select status from v$instance";
                            string? result = connection.ExecuteScalar<string>(CheckDb, null);
                            dbExists = result?.ToUpper() == "OPEN";
                            isOracle = true;
                            break;
                    }

                    if (isOracle)
                    {
                        return dbExists;
                    }

                    if (!dbExists)
                    {
                        if (CliCommand.ExportSql)
                        {
                            sqlWriter.AddSqlCommand($"{CreateDb};");
                            sqlWriter.CreateSqlFile();
                        }

                        return connection.JustExecute(CreateDb, null);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", CheckDb, ex.Message, ex.ToString());

                if (CliCommand.CliExecution)
                {
                    Console.WriteLine("_bhLog_ Create Database Error: " + ex.Message, Console.ForegroundColor = ConsoleColor.Red);
                    Console.WriteLine("_bhLog_", Console.ForegroundColor = ConsoleColor.White);
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if the database exists and returns a boolean
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseBuilder.DoesDbExists()
        {
            string CheckDb = "";

            try
            {
                string serverConnection = _multiDatabaseSelector.GetServerConnection();

                bool isLite = _multiDatabaseSelector.IsLite();

                if (isLite)
                {
                    string databaseLocation = _multiDatabaseSelector.GetServerConnection();

                    if (!File.Exists(databaseLocation))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    bool dbExists = true;

                    string databaseName = _multiDatabaseSelector.GetDatabaseName();

                    switch (_multiDatabaseSelector.GetSqlTypeId())
                    {
                        case 0:
                            CheckDb = $"select count(*) from master.dbo.sysdatabases where name='{databaseName}'";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            break;
                        case 1:
                            CheckDb = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";
                            string? dbName = connection.ExecuteScalar<string>(CheckDb, null);
                            dbExists = dbName?.ToLower() == databaseName.ToLower();
                            break;
                        case 2:
                            CheckDb = $"SELECT 1 FROM pg_database WHERE datname='{databaseName}';";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            break;
                        case 4:
                            CheckDb = "select status from v$instance";
                            string? result = connection.ExecuteScalar<string>(CheckDb, null);
                            dbExists = result?.ToUpper() == "OPEN";
                            break;
                    }

                    return dbExists;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", CheckDb, ex.Message, ex.ToString());
                return false;
            }
        }

        bool IBHDatabaseBuilder.CreateDatabaseSchema()
        {
            if(DatabaseStatics.DatabaseSchema != string.Empty)
            {
                IBHConnection connection = new BHConnection();
            }

            return false;
        }

        bool IBHDatabaseBuilder.IsCreatedFirstTime()
        {
            return DatabaseStatics.InitializeData;
        }
    }
}
