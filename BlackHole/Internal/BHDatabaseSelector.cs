using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.ExecutionProviders;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHDatabaseSelector : IBHDatabaseSelector
    {

        IExecutionProvider IBHDatabaseSelector.GetExecutionProvider(string connectionString)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerExecutionProvider(connectionString, DatabaseStatics.IsQuotedDatabase),
                BlackHoleSqlTypes.MySql => new MySqlExecutionProvider(connectionString),
                BlackHoleSqlTypes.Postgres => new PostgresExecutionProvider(connectionString),
                BlackHoleSqlTypes.SqlLite => new SqLiteExecutionProvider(connectionString, DatabaseStatics.IsQuotedDatabase),
                _ => new OracleExecutionProvider(connectionString),
            };
        }

        bool IBHDatabaseSelector.IsLite()
        {
            bool lite = false;

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlLite)
            {
                lite = true;
            }

            return lite;
        }

        string IBHDatabaseSelector.GetPrimaryKeyCommand()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $"{GetMyShit("Id")} INT IDENTITY(1,1) PRIMARY KEY NOT NULL ,",
                BlackHoleSqlTypes.MySql => $"{GetMyShit("Id")} int AUTO_INCREMENT primary key NOT NULL ,",
                BlackHoleSqlTypes.Postgres => $"{GetMyShit("Id")} SERIAL PRIMARY KEY ,",
                BlackHoleSqlTypes.SqlLite => $"{GetMyShit("Id")} INTEGER PRIMARY KEY AUTOINCREMENT ,",
                _ => $"{GetMyShit("Id")} NUMBER(8,0) GENERATED ALWAYS AS IDENTITY PRIMARY KEY ,",
            };
        }

        string IBHDatabaseSelector.GetStringPrimaryKeyCommand()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $"{GetMyShit("Id")} NVARCHAR(50) PRIMARY KEY NOT NULL ,",
                BlackHoleSqlTypes.MySql => $"{GetMyShit("Id")} varchar(50) NOT NULL PRIMARY KEY ,",
                BlackHoleSqlTypes.Postgres => $"{GetMyShit("Id")} varchar(50) NOT NULL PRIMARY KEY ,",
                BlackHoleSqlTypes.SqlLite => $"{GetMyShit("Id")} varchar(50) PRIMARY KEY ,",
                _ => $"{GetMyShit("Id")} varchar2(50) NOT NULL PRIMARY KEY ,",
            };
        }

        string IBHDatabaseSelector.GetGuidPrimaryKeyCommand()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $"{GetMyShit("Id")} UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()) ,",
                BlackHoleSqlTypes.MySql => $"{GetMyShit("Id")} varchar(36) NOT NULL PRIMARY KEY ,",
                BlackHoleSqlTypes.Postgres => $"{GetMyShit("Id")} uuid DEFAULT gen_random_uuid() PRIMARY KEY ,",
                BlackHoleSqlTypes.SqlLite => $"{GetMyShit("Id")} varchar(36) PRIMARY KEY ,",
                _ => $"{GetMyShit("Id")} varchar2(36) NOT NULL PRIMARY KEY ,",
            };
        }

        string IBHDatabaseSelector.GetCompositePrimaryKeyCommand(Type propType, string columName)
        {
            if(propType == typeof(Guid))
            {
                return DatabaseStatics.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => $"{GetMyShit(columName)} UNIQUEIDENTIFIER DEFAULT (NEWID()) ,",
                    BlackHoleSqlTypes.Postgres => $"{GetMyShit(columName)} uuid DEFAULT gen_random_uuid() ,",
                    _ => string.Empty,
                };
            }

            if(propType == typeof(long))
            {
                return DatabaseStatics.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => $"{GetMyShit(columName)} BIGINT IDENTITY(1,1) NOT NULL ,",
                    BlackHoleSqlTypes.MySql => $"{GetMyShit(columName)} bigint AUTO_INCREMENT NOT NULL ,",
                    BlackHoleSqlTypes.Postgres => $"{GetMyShit(columName)} BIGSERIAL ,",
                    BlackHoleSqlTypes.SqlLite => $"{GetMyShit(columName)} bigint default (last_insert_rowid() + 1) NOT NULL ,",
                    _ => $"{GetMyShit(columName)} NUMBER(16,0) GENERATED ALWAYS AS IDENTITY ,",
                };
            }

            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $"{GetMyShit(columName)} INT IDENTITY(1,1) NOT NULL ,",
                BlackHoleSqlTypes.MySql => $"{GetMyShit(columName)} int AUTO_INCREMENT NOT NULL ,",
                BlackHoleSqlTypes.Postgres => $"{GetMyShit(columName)} SERIAL ,",
                BlackHoleSqlTypes.SqlLite => $"{GetMyShit(columName)} INTEGER default (last_insert_rowid() + 1) NOT NULL ,",
                _ => $"{GetMyShit(columName)} NUMBER(8,0) GENERATED ALWAYS AS IDENTITY ,",
            };
        }

        bool IBHDatabaseSelector.GetMyShit()
        {
            return SkipQuotes();
        }

        string IBHDatabaseSelector.GetServerConnection()
        {
            return DatabaseStatics.ServerConnection;
        }

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

        bool IBHDatabaseSelector.GetOpenPKConstraint()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => true,
                BlackHoleSqlTypes.MySql => false,
                BlackHoleSqlTypes.Postgres => false,
                BlackHoleSqlTypes.SqlLite => false,
                _ => true,
            };
        }

        bool IBHDatabaseSelector.SetDbDateFormat(IExecutionProvider _executionProvider)
        {
            string getDateCommand = DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => "SELECT r.date_format FROM master.sys.dm_exec_requests r WHERE r.session_id = @@SPID",
                BlackHoleSqlTypes.MySql => "system",
                BlackHoleSqlTypes.Postgres => "show datestyle",
                BlackHoleSqlTypes.SqlLite => "system",
                _ => "SELECT value FROM V$NLS_PARAMETERS WHERE parameter = 'NLS_DATE_FORMAT'",
            };
            if (getDateCommand == "system")
            {
                return true;
            }
            else
            {
                string? dateFormat = _executionProvider.ExecuteScalar<string>(getDateCommand, null);
                if (string.IsNullOrEmpty(dateFormat))
                {
                    return false;
                }
                DatabaseStatics.DbDateFormat = AnalyzeDateFormat(dateFormat);
            }
            return true;
        }

        private string AnalyzeDateFormat(string dateFormat)
        {
            if(DatabaseStatics.DatabaseType == BlackHoleSqlTypes.Oracle)
            {
                return dateFormat.ToLower().Replace("/", "-").Replace("mm", "MM").Replace("rr", "yyyy");
            }
            string[] parts = dateFormat.Split(",");
            string cleanDateFormat = string.Empty;
            if(parts.Length > 1)
            {
                cleanDateFormat = parts[1].Trim();
            }
            else
            {
                cleanDateFormat = parts[0].Trim();
            }
            string result = string.Empty;
            foreach(char letter in cleanDateFormat.ToLower())
            {
                result += letter switch
                {
                    'm' => "-MM",
                    'd' => "-dd",
                    _ => "-yyyy"
                };
            }
            return result.Remove(0,1);
        }

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
                    SqlDatatypes = new[] { "varchar", "char", "int2", "integer", "bigint", "decimal", "float", "numeric", "varchar(36)", "boolean", "datetime", "blob" };
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
                return $"and table_schema = '{DatabaseStatics.DatabaseSchema}'";
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

        string IBHDatabaseSelector.GetDatabaseName()
        {
            if (DatabaseStatics.DatabaseType != BlackHoleSqlTypes.SqlLite)
            {
                try
                {
                    string[] dbLinkSplit = DatabaseStatics.DatabaseName.Split("=");
                    return dbLinkSplit[1];
                }
                catch { return string.Empty; }
            }
            else
            {
                return DatabaseStatics.DatabaseName;
            }
        }

        private bool SkipQuotes()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => DatabaseStatics.IsQuotedDatabase ? false : true,
                BlackHoleSqlTypes.MySql => true,
                BlackHoleSqlTypes.Postgres => false,
                BlackHoleSqlTypes.SqlLite => DatabaseStatics.IsQuotedDatabase ? false : true,
                _ => false,
            };
        }

        private string GetMyShit(string propName)
        {
            if (!SkipQuotes())
            {
                return $@"""{propName}""";
            }
            return propName;
        }

        string IBHDatabaseSelector.GetOwnerName()
        {
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
                catch { return string.Empty; }
            }
            return string.Empty;
        }
    }
}
