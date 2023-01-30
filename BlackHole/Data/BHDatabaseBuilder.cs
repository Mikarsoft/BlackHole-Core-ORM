using BlackHole.Interfaces;
using BlackHole.Services;
using BlackHole.Statics;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace BlackHole.Data
{
    internal class BHDatabaseBuilder : IBHDatabaseBuilder
    {
        private readonly IBHDatabaseSelector _multiDatabaseSelector;
        private readonly ILoggerService _loggerService;

        internal BHDatabaseBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            _loggerService = new LoggerService();
        }

        bool IBHDatabaseBuilder.DropDatabase()
        {
            bool success = false;
            string serverConnection = _multiDatabaseSelector.GetServerConnection();

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

                    success = true;
                }
                else
                {
                    using (IDbConnection connection = _multiDatabaseSelector.CreateConnection(serverConnection))
                    {
                        string databaseName = _multiDatabaseSelector.GetDatabaseName();
                        string CheckDb = "";
                        bool dbExists = false;
                        string DropDb = $@"DROP DATABASE IF EXISTS ""{databaseName}""";

                        switch (_multiDatabaseSelector.GetSqlTypeId())
                        {
                            case 0:
                                CheckDb = $"select count(*) from master.dbo.sysdatabases where name='{databaseName}'";
                                dbExists = connection.ExecuteScalar<int>(CheckDb) == 1;
                                DropDb = $@"USE master;ALTER DATABASE ""{databaseName}"" SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE ""{databaseName}"";";
                                break;
                            case 1:
                                string DeleteMyShit = $"DROP DATABASE IF EXISTS {databaseName}";
                                connection.Execute(DeleteMyShit);
                                break;
                            case 2:
                                CheckDb = $"SELECT 1 FROM pg_database WHERE datname='{databaseName}';";
                                dbExists = connection.ExecuteScalar<int>(CheckDb) == 1;
                                DropDb = $@"UPDATE pg_database SET datallowconn = 'false' WHERE datname = '{databaseName}'; SELECT pg_terminate_backend(pg_stat_activity.pid)
                                FROM pg_stat_activity WHERE pg_stat_activity.datname = '{databaseName}'; DROP DATABASE ""{databaseName}""";
                                break;
                        }

                        if (dbExists)
                        {
                            connection.Execute(DropDb);
                        }
                    }
                    success = true;
                }
            }
            catch(Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString());
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Creates a new Database if it doesn't exist, in the specified Server or location
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseBuilder.CheckDatabaseExistance()
        {
            bool success = false;
            string serverConnection = _multiDatabaseSelector.GetServerConnection();

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
                    success = true;
                }
                else
                {
                    using (IDbConnection connection = _multiDatabaseSelector.CreateConnection(serverConnection))
                    {
                        string databaseName = _multiDatabaseSelector.GetDatabaseName();
                        string CheckDb = "";
                        bool dbExists = true;
                        string CreateDb = $@"CREATE DATABASE ""{databaseName}""";

                        switch (_multiDatabaseSelector.GetSqlTypeId())
                        {
                            case 0:
                                CheckDb = $"select count(*) from master.dbo.sysdatabases where name='{databaseName}'";
                                dbExists = connection.ExecuteScalar<int>(CheckDb) == 1;
                                break;
                            case 1:
                                CheckDb = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName.ToLower()}'";
                                CreateDb = $"CREATE DATABASE IF NOT EXISTS {databaseName}";
                                dbExists = connection.ExecuteScalar<string>(CheckDb) == databaseName.ToLower();
                                break;
                            case 2:
                                CheckDb = $"SELECT 1 FROM pg_database WHERE datname='{databaseName}';";
                                dbExists = connection.ExecuteScalar<int>(CheckDb) == 1;
                                break;
                        }

                        if (!dbExists)
                        {
                            connection.Execute(CreateDb);
                        }

                    }
                    success = true;
                }
            }
            catch(Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString());
                success = false;
            }

            return success;
        }

        bool IBHDatabaseBuilder.DoesDbExists()
        {
            bool success = false;

            try
            {
                string serverConnection = _multiDatabaseSelector.GetServerConnection();

                bool isLite = _multiDatabaseSelector.IsLite();

                if (isLite)
                {
                    string databaseLocation = _multiDatabaseSelector.GetServerConnection();

                    if (!File.Exists(databaseLocation))
                    {
                        success = false;
                    }
                    else
                    {
                        success = true;
                    }
                }
                else
                {
                    bool dbExists = true;

                    using (IDbConnection connection = _multiDatabaseSelector.CreateConnection(serverConnection))
                    {
                        string databaseName = _multiDatabaseSelector.GetDatabaseName();
                        string CheckDb = "";

                        switch (_multiDatabaseSelector.GetSqlTypeId())
                        {
                            case 0:
                                CheckDb = $"select count(*) from master.dbo.sysdatabases where name='{databaseName}'";
                                dbExists = connection.ExecuteScalar<int>(CheckDb) == 1;
                                break;
                            case 1:
                                CheckDb = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";
                                string dbName = connection.ExecuteScalar<string>(CheckDb);
                                dbExists = dbName.ToLower() == databaseName.ToLower();
                                break;
                            case 2:
                                CheckDb = $"SELECT 1 FROM pg_database WHERE datname='{databaseName}';";
                                dbExists = connection.ExecuteScalar<int>(CheckDb) == 1;
                                break;
                        }
                    }

                    success = dbExists;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString());
                success = false;
            }

            return success;
        }
    }
}
