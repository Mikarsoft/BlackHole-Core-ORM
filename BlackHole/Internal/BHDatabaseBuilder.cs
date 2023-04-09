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

        internal BHDatabaseBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            _loggerService = new LoggerService();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ServerConnection);
        }

        /// <summary>
        /// Closes all open connections and drops the database
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseBuilder.DropDatabase()
        {
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
                    string CheckDb = "";
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
                            CheckDb = $"SELECT table_name FROM all_tables WHERE owner = 'ROOT';";
                            List<string> existingTables = connection.Query<string>(CheckDb, null);

                            foreach(string table in existingTables)
                            {
                                connection.JustExecute($"DROP TABLE {table} CASCADE CONSTRAINTS;", null);
                            }

                            break;
                    }

                    if (dbExists)
                    {
                        connection.JustExecute(DropDb, null);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Creates a new Database if it doesn't exist, in the specified Server or location
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseBuilder.CreateDatabase()
        {
            try
            {
                bool isLite = _multiDatabaseSelector.IsLite();

                if (isLite)
                {
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
                    string CheckDb = "";
                    bool dbExists = true;
                    string CreateDb = $@"CREATE DATABASE ""{databaseName}""";
                    bool isOracle = false;

                    switch (_multiDatabaseSelector.GetSqlTypeId())
                    {
                        case 0:
                            CheckDb = $"select count(*) from master.dbo.sysdatabases where name='{databaseName}'";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            break;
                        case 1:
                            CheckDb = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";
                            CreateDb = $"CREATE DATABASE IF NOT EXISTS {databaseName}";
                            string? dbName = connection.ExecuteScalar<string>(CheckDb, null);
                            dbExists = dbName?.ToLower() == databaseName.ToLower();
                            break;
                        case 2:
                            CheckDb = $"SELECT 1 FROM pg_database WHERE datname='{databaseName}';";
                            dbExists = connection.ExecuteScalar<int>(CheckDb, null) == 1;
                            break;
                        case 4:
                            CheckDb = "select status from v$instance;";
                            dbExists = connection.ExecuteScalar<string>(CheckDb, null) == "OPEN";
                            isOracle = true;
                            break;
                    }

                    if (isOracle)
                    {
                        return dbExists;
                    }

                    if (!dbExists)
                    {
                        connection.JustExecute(CreateDb, null);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Checks if the database exists and returns a boolean
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseBuilder.DoesDbExists()
        {
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
                    string CheckDb = "";

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
                            CheckDb = "select status from v$instance;";
                            dbExists = connection.ExecuteScalar<string>(CheckDb, null) == "OPEN";
                            break;
                    }

                    return dbExists;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString());
                return false;
            }
        }
    }
}
