﻿using BlackHole.Enums;
using BlackHole.Interfaces;
using BlackHole.Statics;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data;

namespace BlackHole.Data
{
    internal class BHDatabaseSelector : IBHDatabaseSelector
    {
        IDbConnection IBHDatabaseSelector.GetConnection()
        {
            string _connectionString = DatabaseStatics.ConnectionString;
            IDbConnection _Sconnection = new SqlConnection();

            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    _Sconnection = new SqlConnection(_connectionString);
                    break;
                case BHSqlTypes.MySql:
                    _Sconnection = new MySqlConnection(_connectionString);
                    break;
                case BHSqlTypes.Postgres:
                    _Sconnection = new NpgsqlConnection(_connectionString);
                    break;
                case BHSqlTypes.SqlLite:
                    _Sconnection = new SqliteConnection(_connectionString);
                    break;
            }

            return _Sconnection;
        }

        IDbConnection IBHDatabaseSelector.CreateConnection(string connectionString)
        {
            IDbConnection _Sconnection = new SqlConnection();

            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    _Sconnection = new SqlConnection(connectionString);
                    break;
                case BHSqlTypes.MySql:
                    _Sconnection = new MySqlConnection(connectionString);
                    break;
                case BHSqlTypes.Postgres:
                    _Sconnection = new NpgsqlConnection(connectionString);
                    break;
                case BHSqlTypes.SqlLite:
                    _Sconnection = new SqliteConnection(connectionString);
                    break;
            }

            return _Sconnection;
        }


        string[] IBHDatabaseSelector.IdOutput(string tableName, string columnName, bool g)
        {
            string[] InsertOutputs = new string[2];
            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    InsertOutputs[0] = $"output inserted.{columnName}";
                    break;
                case BHSqlTypes.MySql:
                    if (g)
                    {
                        InsertOutputs[1] = $";SELECT CAST(LAST_INSERT_ID() AS VARCHAR);";
                    }
                    else
                    {
                        InsertOutputs[1] = $";SELECT LAST_INSERT_ID();";
                    }
                    break;
                case BHSqlTypes.Postgres:
                    InsertOutputs[1] = $"returning {tableName}.{columnName};";
                    break;
                case BHSqlTypes.SqlLite:
                    InsertOutputs[1] = $"returning {columnName}";
                    break;
            }

            return InsertOutputs;
        }

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
                        case BHSqlTypes.MsSql:
                            _Sconnection = new SqlConnection(serverConnectionString);
                            break;
                        case BHSqlTypes.MySql:
                            _Sconnection = new MySqlConnection(serverConnectionString);
                            break;
                        case BHSqlTypes.Postgres:
                            _Sconnection = new NpgsqlConnection(serverConnectionString);
                            break;
                        case BHSqlTypes.SqlLite:
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

        bool IBHDatabaseSelector.IsLite()
        {
            return CheckSqlLite();
        }

        bool CheckSqlLite()
        {
            bool lite = false;

            if (DatabaseStatics.DatabaseType == BHSqlTypes.SqlLite)
            {
                lite = true;
            }

            return lite;
        }

        bool IBHDatabaseSelector.IsTransact()
        {
            bool transact = false;

            if(DatabaseStatics.DatabaseType == BHSqlTypes.MsSql)
            {
                transact = true;
            }

            return transact;
        }

        string GetServerConnectionString()
        {
            return DatabaseStatics.ServerConnection;
        }


        string IBHDatabaseSelector.GetPrimaryKeyCommand()
        {
            string PrimaryKeyCommand = "";
            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    PrimaryKeyCommand = @"""Id"" INT IDENTITY(1,1) PRIMARY KEY NOT NULL ,";
                    break;
                case BHSqlTypes.MySql:
                    PrimaryKeyCommand = @"Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY ,";
                    break;
                case BHSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" SERIAL PRIMARY KEY ,";
                    break;
                case BHSqlTypes.SqlLite:
                    PrimaryKeyCommand = @"Id INTEGER PRIMARY KEY AUTOINCREMENT ,";
                    break;
            }
            return PrimaryKeyCommand;
        }

        string IBHDatabaseSelector.GetGuidPrimaryKeyCommand()
        {
            string PrimaryKeyCommand = "";
            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    PrimaryKeyCommand = @"""Id"" UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()) ,";
                    break;
                case BHSqlTypes.MySql:
                    PrimaryKeyCommand = @"Id varchar(50) NOT NULL PRIMARY KEY ,";
                    break;
                case BHSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" uuid DEFAULT gen_random_uuid() PRIMARY KEY ,";
                    break;
                case BHSqlTypes.SqlLite:
                    PrimaryKeyCommand = @"Id TEXT PRIMARY KEY ,";
                    break;
            }
            return PrimaryKeyCommand;
        }

        bool IsMyShit()
        {
            bool myShit = false;
            if(DatabaseStatics.DatabaseType == BHSqlTypes.MySql || DatabaseStatics.DatabaseType == BHSqlTypes.SqlLite)
            {
                myShit = true;
            }
            return myShit;
        }

        bool IBHDatabaseSelector.IsMyShittyDb()
        {
            bool myShit = false;
            if (DatabaseStatics.DatabaseType == BHSqlTypes.MySql)
            {
                myShit = true;
            }
            return myShit;
        }

        bool IBHDatabaseSelector.GetMyShit()
        {
            return IsMyShit();
        }

        string IBHDatabaseSelector.GetServerConnection()
        {
            return GetServerConnectionString();
        }

        int IBHDatabaseSelector.GetSqlTypeId()
        {
            int sqlTypeId = 0;
            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    sqlTypeId = 0;
                    break;
                case BHSqlTypes.MySql:
                    sqlTypeId = 1;
                    break;
                case BHSqlTypes.Postgres:
                    sqlTypeId = 2;
                    break;
                case BHSqlTypes.SqlLite:
                    sqlTypeId = 3;
                    break;
            }
            return sqlTypeId;
        }

        string[] IBHDatabaseSelector.SqlDatatypesTranslation()
        {
            string[] SqlDatatypes = new string[10];
            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.MsSql:
                    SqlDatatypes = new[] {"nvarchar", "int", "bigint", "decimal", "float", "float", "uniqueidentifier", "bit", "datetime", "varbinary"};
                    break;
                case BHSqlTypes.MySql:
                    SqlDatatypes = new[] { "varchar", "int", "bigint", "dec", "float", "double", "varchar(50)", "boolean", "datetime", "blob" };
                    break;
                case BHSqlTypes.Postgres:
                    SqlDatatypes = new[] { "varchar", "integer", "bigint", "numeric(15,2)", "numeric(21,5)", "numeric", "uuid", "boolean", "timestamp", "bytea" };
                    break;
                case BHSqlTypes.SqlLite:
                    SqlDatatypes = new[] { "varchar", "integer", "bigint", "decimal(15,2)", "float", "numeric", "varchar", "boolean", "datetime", "blob" };
                    break;
            }
            return SqlDatatypes;
        }

        string IBHDatabaseSelector.GetDatabaseName()
        {
            string databaseName = string.Empty;

            if(DatabaseStatics.DatabaseType != BHSqlTypes.SqlLite)
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

        BHSqlTypes IBHDatabaseSelector.GetSqlType()
        {
            return DatabaseStatics.DatabaseType;
        }
    }
}
