using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.ExecutionProviders;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHDatabaseSelector : IBHDatabaseSelector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IExecutionProvider IBHDatabaseSelector.GetExecutionProvider(string connectionString)
        {
            IExecutionProvider _Sconnection = new OracleExecutionProvider(connectionString);

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    _Sconnection = new SqlServerExecutionProvider(connectionString);
                    break;
                case BlackHoleSqlTypes.MySql:
                    _Sconnection = new MySqlExecutionProvider(connectionString);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    _Sconnection = new PostgresExecutionProvider(connectionString);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    _Sconnection = new SqLiteExecutionProvider(connectionString);
                    break;
                case BlackHoleSqlTypes.Oracle:
                    _Sconnection = new OracleExecutionProvider(connectionString);
                    break;
            }

            return _Sconnection;
        }

        /// <summary>
        /// Checks if the selected Sql Type is SQLite and returns boolean
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseSelector.IsLite()
        {
            bool lite = false;

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlLite)
            {
                lite = true;
            }

            return lite;
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
                    PrimaryKeyCommand = "Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL ,";
                    break;
                case BlackHoleSqlTypes.MySql:
                    PrimaryKeyCommand = "Id int AUTO_INCREMENT primary key NOT NULL ,";
                    break;
                case BlackHoleSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" SERIAL PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    PrimaryKeyCommand = "Id INTEGER PRIMARY KEY AUTOINCREMENT ,";
                    break;
                case BlackHoleSqlTypes.Oracle:
                    PrimaryKeyCommand = @"""Id"" NUMBER(8,0) GENERATED ALWAYS AS IDENTITY PRIMARY KEY ,";
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
                    PrimaryKeyCommand = "Id NVARCHAR(50) PRIMARY KEY NOT NULL ,";
                    break;
                case BlackHoleSqlTypes.MySql:
                    PrimaryKeyCommand = "Id varchar(50) NOT NULL PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" varchar(50) NOT NULL PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    PrimaryKeyCommand = "Id TEXT PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.Oracle:
                    PrimaryKeyCommand = @"""Id"" varchar2(50) NOT NULL PRIMARY KEY ,";
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
                    PrimaryKeyCommand = "Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()) ,";
                    break;
                case BlackHoleSqlTypes.MySql:
                    PrimaryKeyCommand = "Id varchar(36) NOT NULL PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.Postgres:
                    PrimaryKeyCommand = @"""Id"" uuid DEFAULT gen_random_uuid() PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    PrimaryKeyCommand = "Id TEXT PRIMARY KEY ,";
                    break;
                case BlackHoleSqlTypes.Oracle:
                    PrimaryKeyCommand = @"""Id"" varchar2(36) NOT NULL PRIMARY KEY ,";
                    break;
            }
            return PrimaryKeyCommand;
        }

        /// <summary>
        /// Checks if the dabase type is MySql or SqLite and returns boolean
        /// </summary>
        /// <returns></returns>
        bool IBHDatabaseSelector.GetMyShit()
        {
            bool myShit = false;
            if (DatabaseStatics.DatabaseType != BlackHoleSqlTypes.Postgres && DatabaseStatics.DatabaseType != BlackHoleSqlTypes.Oracle)
            {
                myShit = true;
            }
            return myShit;
        }

        /// <summary>
        /// Returns the connection string for the sql host server
        /// </summary>
        /// <returns></returns>
        string IBHDatabaseSelector.GetServerConnection()
        {
            return DatabaseStatics.ServerConnection;
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
                case BlackHoleSqlTypes.Oracle:
                    sqlTypeId = 4;
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
            string[] SqlDatatypes = new string[12];
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    SqlDatatypes = new[] { "nvarchar", "char", "smallint", "int", "bigint", "decimal", "smallmoney", "float", "uniqueidentifier", "bit", "datetime", "varbinary" };
                    break;
                case BlackHoleSqlTypes.MySql:
                    SqlDatatypes = new[] { "varchar", "char","smallint", "int", "bigint", "decimal", "float", "double", "varchar(36)", "bit", "datetime", "varbinary" };
                    break;
                case BlackHoleSqlTypes.Postgres:
                    SqlDatatypes = new[] { "varchar", "char", "int2", "int4", "int8", "numeric", "float4", "float8", "uuid", "bool", "timestamp", "bytea" };
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    SqlDatatypes = new[] { "varchar", "char", "int2", "integer", "bigint", "decimal", "float", "numeric", "text", "boolean", "datetime", "blob" };
                    break;
                case BlackHoleSqlTypes.Oracle:
                    SqlDatatypes = new[] { "varchar2", "Char", "Number(4,0)", "Number(8,0)", "Number(16,0)", "Number(19,0)", "Number(18,0)", "Number", "varchar2(36)", "Number(1,0)", "Timestamp", "Blob" };
                    break;
            }
            return SqlDatatypes;
        }

        string IBHDatabaseSelector.TableSchemaCheck()
        {
            if (DatabaseStatics.DatabaseSchema != string.Empty)
            {
                return $"and table_schema = '{DatabaseStatics.DatabaseSchema}";
            }

            return string.Empty;
        }

        string IBHDatabaseSelector.GetDatabaseSchema()
        {
            if (DatabaseStatics.DatabaseSchema != string.Empty)
            {
                return $"{DatabaseStatics.DatabaseSchema}.";
            }
            return string.Empty;
        }

        string IBHDatabaseSelector.GetDatabaseSchemaFk()
        {
            if (DatabaseStatics.DatabaseSchema != string.Empty)
            {
                return $"{DatabaseStatics.DatabaseSchema}";
            }
            return string.Empty;
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

        string IBHDatabaseSelector.GetOwnerName()
        {
            string databaseName = string.Empty;

            if (DatabaseStatics.DatabaseType != BlackHoleSqlTypes.SqlLite && DatabaseStatics.DatabaseType != BlackHoleSqlTypes.Oracle)
            {
                try
                {
                    if(DatabaseStatics.OwnerName != string.Empty)
                    {
                        string[] dbLinkSplit = DatabaseStatics.OwnerName.Split("=");
                        return dbLinkSplit[1];
                    }
                }
                catch { databaseName = string.Empty; }
            }

            return string.Empty;
        }
    }
}
