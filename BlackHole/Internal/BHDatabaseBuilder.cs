using BlackHole.Core;
using BlackHole.Engine;
using BlackHole.Logger;
using BlackHole.Statics;
using Microsoft.Data.Sqlite;

namespace BlackHole.Internal
{
    internal class BHDatabaseBuilder
    {
        private readonly BHDatabaseSelector _multiDatabaseSelector;
        private bool ExistingDb { get; set; } = false;
        private string SchemaCreationCommand { get; set; } = string.Empty;
        private BHSqlExportWriter SqlWriter { get; set; }

        internal BHDatabaseBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            SqlWriter = new BHSqlExportWriter("1_DatabaseSql", "SqlFiles", "sql");
        }

        internal bool DropDatabase(int connectionIndex)
        {
            string CheckDb = "";

            if (!BHStaticSettings.IsDevMove && !CliCommand.ForceAction)
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
                IDataProvider connection = _multiDatabaseSelector.GetExecutionProvider(connectionIndex);

                bool isLite = _multiDatabaseSelector.IsLite(connectionIndex);

                if (isLite)
                {
                    string databaseLocation = _multiDatabaseSelector.GetServerConnection(connectionIndex);

                    if (File.Exists(databaseLocation))
                    {

                        SqliteConnection.ClearPool(new SqliteConnection(WormHoleData.ConnectionStrings[connectionIndex]));
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        File.Delete(databaseLocation);
                    }

                    return true;
                }
                else
                {
                    string databaseName = _multiDatabaseSelector.GetDatabaseName(connectionIndex);
                    bool dbExists = false;
                    string DropDb = $@"DROP DATABASE IF EXISTS ""{databaseName}""";

                    switch (_multiDatabaseSelector.GetSqlTypeId(connectionIndex))
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
                            if (dbExists)
                            {
                                connection.JustExecute($@"UPDATE pg_database SET datallowconn = 'false' WHERE datname = '{databaseName}'; SELECT pg_terminate_backend(pg_stat_activity.pid)
                                    FROM pg_stat_activity WHERE pg_stat_activity.datname = '{databaseName}'; ",null);
                            }
                            DropDb = $@" DROP DATABASE ""{databaseName}""";
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
                        SqlWriter.DeleteSqlFolder();
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
                Task.Factory.StartNew(() => CheckDb.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString()));

                if (CliCommand.CliExecution)
                {
                    Console.WriteLine("_bhLog_ Drop Database Error: "+ex.Message, Console.ForegroundColor = ConsoleColor.Red);
                    Console.WriteLine("_bhLog_", Console.ForegroundColor = ConsoleColor.White);
                }

                return false;
            }
        }

        internal bool CreateDatabase(int connectionIndex)
        {
            string CheckDb = "";
            SchemaCreationCommand = string.Empty;
            ExistingDb = false;

            try
            {
                IDataProvider connection = _multiDatabaseSelector.GetExecutionProvider(connectionIndex);
                _multiDatabaseSelector.SetDbDateFormat(connection, connectionIndex);

                bool isLite = _multiDatabaseSelector.IsLite(connectionIndex);

                if (isLite)
                {
                    string databaseLocation = _multiDatabaseSelector.GetServerConnection(connectionIndex);

                    if (!File.Exists(databaseLocation))
                    {
                        var stream = File.Create(databaseLocation);
                        stream.Dispose();
                    }
                    return true;
                }
                else
                {
                    string databaseName = _multiDatabaseSelector.GetDatabaseName(connectionIndex);
                    string ownerName = _multiDatabaseSelector.GetOwnerName(connectionIndex);
                    bool dbExists = false;
                    string CreateDb = $@"CREATE DATABASE ""{databaseName}""";
                    bool isOracle = false;

                    switch (_multiDatabaseSelector.GetSqlTypeId(connectionIndex))
                    {
                        case 0:
                            SchemaCreationCommand = $"IF NOT EXISTS ( SELECT  * FROM sys.schemas WHERE name = '{WormHoleData.DbDefaultSchemas[connectionIndex]}' ) BEGIN EXEC('CREATE SCHEMA [{BHStaticSettings.DatabaseSchema}]') END";
                            CheckDb = $"select count(*) from master.dbo.sysdatabases where name='{databaseName}'";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            break;
                        case 1:
                            SchemaCreationCommand = $"CREATE SCHEMA IF NOT EXISTS {WormHoleData.DbDefaultSchemas[connectionIndex]} AUTHORIZATION {ownerName}";
                            CheckDb = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";
                            CreateDb = $"CREATE DATABASE IF NOT EXISTS {databaseName}";
                            string? dbName = connection.ExecuteScalar<string>(CheckDb, null);
                            dbExists = dbName?.ToLower() == databaseName.ToLower();
                            break;
                        case 2:
                            SchemaCreationCommand = $"CREATE SCHEMA IF NOT EXISTS {WormHoleData.DbDefaultSchemas[connectionIndex]} AUTHORIZATION {ownerName}";
                            CheckDb = $"SELECT 1 FROM pg_database WHERE datname='{databaseName}';";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            break;
                        case 4:
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
                            SqlWriter.AddSqlCommand($"{CreateDb};");
                        }

                        return connection.JustExecute(CreateDb, null);
                    }

                    ExistingDb = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => CheckDb.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString()));

                if (CliCommand.CliExecution)
                {
                    Console.WriteLine("_bhLog_ Create Database Error: " + ex.Message, Console.ForegroundColor = ConsoleColor.Red);
                    Console.WriteLine("_bhLog_", Console.ForegroundColor = ConsoleColor.White);
                }

                return false;
            }
        }

        internal bool DoesDbExists(int connectionIndex)
        {
            string CheckDb = "";

            try
            {
                IDataProvider connection = _multiDatabaseSelector.GetExecutionProvider(connectionIndex);

                string serverConnection = _multiDatabaseSelector.GetServerConnection(connectionIndex);

                bool isLite = _multiDatabaseSelector.IsLite(connectionIndex);

                if (isLite)
                {
                    string databaseLocation = _multiDatabaseSelector.GetServerConnection(connectionIndex);

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

                    string databaseName = _multiDatabaseSelector.GetDatabaseName(connectionIndex);

                    switch (_multiDatabaseSelector.GetSqlTypeId(connectionIndex))
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
                Task.Factory.StartNew(() => CheckDb.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString()));
                return false;
            }
        }

        internal bool CreateDatabaseSchema(int connectionIndex)
        {
            string dbSchema = WormHoleData.DbDefaultSchemas[connectionIndex];

            if (dbSchema != string.Empty && SchemaCreationCommand != string.Empty)
            {
                IBHConnection connection = new BHConnection(connectionIndex);

                if (connection.JustExecute(SchemaCreationCommand))
                {
                    if (CliCommand.CliExecution)
                    {
                        Console.WriteLine("_bhLog_");
                        Console.WriteLine($"_bhLog_ Created Schema : {dbSchema}.");
                    }

                    if (CliCommand.ExportSql)
                    {
                        SqlWriter.AddSqlCommand(SchemaCreationCommand);
                    }
                }
                else
                {
                    if (CliCommand.CliExecution)
                    {
                        Console.WriteLine("_bhLog_");
                        Console.WriteLine($"_bhLog_ Error : Failed to Created Schema {dbSchema}.");
                    }
                }
            }

            if (CliCommand.ExportSql && !ExistingDb)
            {
                SqlWriter.CreateSqlFile();
            }

            return false;
        }

        internal bool IsCreatedFirstTime(int connectionIndex)
        {
            return WormHoleData.InitializeData[connectionIndex];
        }
    }
}
