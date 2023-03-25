﻿using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Statics;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data;
using System.Data.SqlClient;

namespace BlackHole.Internal
{
    internal class BHDatabaseSelector : IBHDatabaseSelector
    {
        /// <summary>
        /// Returns and IDBConnection with the specified database
        /// </summary>
        /// <returns></returns>
        IDbConnection IBHDatabaseSelector.GetConnection()
        {
            string _connectionString = DatabaseStatics.ConnectionString;
            IDbConnection _Sconnection = new SqlConnection();

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    _Sconnection = new SqlConnection(_connectionString);
                    break;
                case BlackHoleSqlTypes.MySql:
                    _Sconnection = new MySqlConnection(_connectionString);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    _Sconnection = new NpgsqlConnection(_connectionString);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    _Sconnection = new SqliteConnection(_connectionString);
                    break;
            }

            return _Sconnection;
        }

        /// <summary>
        /// Creates and returns a connection to a database  using the given connection string
        /// </summary>
        /// <param name="connectionString">connection string for the database</param>
        /// <returns></returns>
        IDbConnection IBHDatabaseSelector.CreateConnection(string connectionString)
        {
            IDbConnection _Sconnection = new SqlConnection();

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    _Sconnection = new SqlConnection(connectionString);
                    break;
                case BlackHoleSqlTypes.MySql:
                    _Sconnection = new MySqlConnection(connectionString);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    _Sconnection = new NpgsqlConnection(connectionString);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    _Sconnection = new SqliteConnection(connectionString);
                    break;
            }

            return _Sconnection;
        }


        /// <summary>
        /// Depending on the type of the database , returns a command that allows the return of the
        /// autogenerated id after a successful entry insert
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        string[] IBHDatabaseSelector.IdOutput(string tableName, string columnName, bool g)
        {
            string[] InsertOutputs = new string[2];
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    InsertOutputs[0] = $"output inserted.{columnName}";
                    break;
                case BlackHoleSqlTypes.MySql:
                    if (g)
                    {
                        InsertOutputs[1] = $";SELECT CAST(LAST_INSERT_ID() AS VARCHAR);";
                    }
                    else
                    {
                        InsertOutputs[1] = $";SELECT LAST_INSERT_ID();";
                    }
                    break;
                case BlackHoleSqlTypes.Postgres:
                    InsertOutputs[1] = $"returning {tableName}.{columnName};";
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    InsertOutputs[1] = $"returning {columnName}";
                    break;
            }

            return InsertOutputs;
        }

        /// <summary>
        /// Checks if the server that will host the datbase exists
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseSelector.TestServerConnection()
        {
            bool success = false;
            string serverConnectionString = GetServerConnectionString();

            try
            {
                bool isLite = CheckSqlLite();

                if (isLite)
                {
                    string sqLiteConnection = DatabaseStatics.ServerConnection;
                    if (!File.Exists(sqLiteConnection))
                    {
                        var stream = File.Create(sqLiteConnection);
                        stream.Dispose();
                    }
                }
                else
                {
                    IDbConnection _Sconnection = new SqlConnection();

                    switch (DatabaseStatics.DatabaseType)
                    {
                        case BlackHoleSqlTypes.SqlServer:
                            _Sconnection = new SqlConnection(serverConnectionString);
                            break;
                        case BlackHoleSqlTypes.MySql:
                            _Sconnection = new MySqlConnection(serverConnectionString);
                            break;
                        case BlackHoleSqlTypes.Postgres:
                            _Sconnection = new NpgsqlConnection(serverConnectionString);
                            break;
                        case BlackHoleSqlTypes.SqlLite:
                            _Sconnection = new SqliteConnection(serverConnectionString);
                            break;
                    }
                }
                success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Checks if the selected Sql Type is SQLite and returns boolean
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseSelector.IsLite()
        {
            return CheckSqlLite();
        }


        bool CheckSqlLite()
        {
            bool lite = false;

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlLite)
            {
                lite = true;
            }

            return lite;
        }

        /// <summary>
        /// Checks if the selected Sql Type id SQL Server and returns boolean
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseSelector.IsTransact()
        {
            bool transact = false;

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlServer)
            {
                transact = true;
            }

            return transact;
        }

        string GetServerConnectionString()
        {
            return DatabaseStatics.ServerConnection;
        }


        /// <summary>
        /// Based on the selected Sql Type, returns the required addition to the sql command,
        /// to create an Integer ,autoincrement Id column
        /// </summary>
        /// <returns></returns>
        /// 
        string IBHDatabaseSelector.GetPrimaryKeyCommand()
        {
            string PrimaryKeyCommand = "";
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    PrimaryKeyCommand = @"""Id"" INT IDENTITY(1,1) PRIMARY KEY NOT NULL ,";
                    break;
                case BlackHoleSqlTypes.MySql:
                    PrimaryKeyCommand = @"Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" SERIAL PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    PrimaryKeyCommand = @"Id INTEGER PRIMARY KEY AUTOINCREMENT ,";
                    break;
            }
            return PrimaryKeyCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string IBHDatabaseSelector.GetStringPrimaryKeyCommand()
        {
            string PrimaryKeyCommand = "";
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    PrimaryKeyCommand = @"""Id"" NVARCHAR(50) PRIMARY KEY NOT NULL ,";
                    break;
                case BlackHoleSqlTypes.MySql:
                    PrimaryKeyCommand = @"Id varchar(50) NOT NULL PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" varchar(50) NOT NULL PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    PrimaryKeyCommand = @"Id TEXT PRIMARY KEY ,";
                    break;
            }
            return PrimaryKeyCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string IBHDatabaseSelector.GetDatePrimaryKeyCommand()
        {
            string PrimaryKeyCommand = "";
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    PrimaryKeyCommand = @"""Id"" INT IDENTITY(1,1) PRIMARY KEY NOT NULL ,";
                    break;
                case BlackHoleSqlTypes.MySql:
                    PrimaryKeyCommand = @"Id TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" timestamp(6) default current_timestamp(6) PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    PrimaryKeyCommand = @"Id timestamp(6) DEFAULT CURRENT_TIMESTAMP(6) ,";
                    break;
            }
            return PrimaryKeyCommand;
        }

        /// <summary>
        /// Based on the selected Sql Type, returns the required addition to the sql command,
        /// to create a Uuid Id column
        /// </summary>
        /// <returns></returns>
        string IBHDatabaseSelector.GetGuidPrimaryKeyCommand()
        {
            string PrimaryKeyCommand = "";
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    PrimaryKeyCommand = @"""Id"" UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()) ,";
                    break;
                case BlackHoleSqlTypes.MySql:
                    PrimaryKeyCommand = @"Id varchar(50) NOT NULL PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" uuid DEFAULT gen_random_uuid() PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    PrimaryKeyCommand = @"Id TEXT PRIMARY KEY ,";
                    break;
            }
            return PrimaryKeyCommand;
        }

        bool IsMyShit()
        {
            bool myShit = false;
            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.MySql || DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlLite)
            {
                myShit = true;
            }
            return myShit;
        }


        /// <summary>
        /// Checks if the dabase type is MySql and returns boolean
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseSelector.IsMyShittyDb()
        {
            bool myShit = false;
            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.MySql)
            {
                myShit = true;
            }
            return myShit;
        }

        /// <summary>
        /// Checks if the dabase type is MySql or SqLite and returns boolean
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseSelector.GetMyShit()
        {
            return IsMyShit();
        }

        /// <summary>
        /// Returns the connection string for the sql host server
        /// </summary>
        /// <returns></returns>
        string IBHDatabaseSelector.GetServerConnection()
        {
            return GetServerConnectionString();
        }

        /// <summary>
        /// Translated the Enum of the Sql Type 
        /// and returns it as an integer, to be easier to save it in a config file
        /// </summary>
        /// <returns></returns>
        int IBHDatabaseSelector.GetSqlTypeId()
        {
            int sqlTypeId = 0;
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    sqlTypeId = 0;
                    break;
                case BlackHoleSqlTypes.MySql:
                    sqlTypeId = 1;
                    break;
                case BlackHoleSqlTypes.Postgres:
                    sqlTypeId = 2;
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    sqlTypeId = 3;
                    break;
            }
            return sqlTypeId;
        }


        /// <summary>
        /// Based on the selected Sql Type , returns an array of datatypes that correspond to
        /// C# datatypes in a specific order
        /// </summary>
        /// <returns></returns>
        string[] IBHDatabaseSelector.SqlDatatypesTranslation()
        {
            string[] SqlDatatypes = new string[10];
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    SqlDatatypes = new[] { "nvarchar", "int", "bigint", "decimal", "float", "float", "uniqueidentifier", "bit", "datetime", "varbinary" };
                    break;
                case BlackHoleSqlTypes.MySql:
                    SqlDatatypes = new[] { "varchar", "int", "bigint", "dec", "float", "double", "varchar(50)", "boolean", "datetime", "blob" };
                    break;
                case BlackHoleSqlTypes.Postgres:
                    SqlDatatypes = new[] { "varchar", "integer", "bigint", "numeric(15,2)", "numeric(21,5)", "numeric", "uuid", "boolean", "timestamp", "bytea" };
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    SqlDatatypes = new[] { "varchar", "integer", "bigint", "decimal(15,2)", "float", "numeric", "varchar", "boolean", "datetime", "blob" };
                    break;
                    //RAW(16) default SYS_GUID()
            }
            return SqlDatatypes;
        }

        /// <summary>
        /// Return the name of the database in string
        /// </summary>
        /// <returns></returns>
        string IBHDatabaseSelector.GetDatabaseName()
        {
            string databaseName = string.Empty;

            if (DatabaseStatics.DatabaseType != BlackHoleSqlTypes.SqlLite)
            {
                try
                {
                    string[] dbLinkSplit = DatabaseStatics.DatabaseName.Split("=");
                    databaseName = dbLinkSplit[1];
                }
                catch { databaseName = string.Empty; }
            }
            else
            {
                databaseName = DatabaseStatics.DatabaseName;
            }


            return databaseName;
        }

        /// <summary>
        /// Gives an Enum of the selected Sql Type
        /// </summary>
        /// <returns></returns>
        BlackHoleSqlTypes IBHDatabaseSelector.GetSqlType()
        {
            return DatabaseStatics.DatabaseType;
        }

        BlackHoleIdTypes IBHDatabaseSelector.GetIdType(Type type)
        {
            if (type == typeof(int))
            {
                return BlackHoleIdTypes.IntId;
            }

            if (type == typeof(Guid))
            {
                return BlackHoleIdTypes.GuidId;
            }

            return BlackHoleIdTypes.StringId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        bool IBHDatabaseSelector.RequiredIdGeneration(BlackHoleIdTypes dataType)
        {
            if (dataType == BlackHoleIdTypes.IntId)
            {
                return false;
            }

            if (dataType == BlackHoleIdTypes.StringId)
            {
                return true;
            }

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlLite || DatabaseStatics.DatabaseType == BlackHoleSqlTypes.MySql)
            {
                return true;
            }

            return false;
        }
    }
}