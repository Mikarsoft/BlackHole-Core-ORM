using BlackHole.Engine;
using BlackHole.DataProviders;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHDatabaseSelector
    {
        internal IDataProvider GetExecutionProvider(int connectionIndex)
        {        
            return WormHoleData.DbTypes[connectionIndex] switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerDataProvider(WormHoleData.ConnectionStrings[connectionIndex], WormHoleData.IsQuotedDb[connectionIndex]),
                BlackHoleSqlTypes.MySql => new MySqlDataProvider(WormHoleData.ConnectionStrings[connectionIndex]),
                BlackHoleSqlTypes.Postgres => new PostgresDataProvider(WormHoleData.ConnectionStrings[connectionIndex]),
                BlackHoleSqlTypes.SqlLite => new SqLiteDataProvider(WormHoleData.ConnectionStrings[connectionIndex], WormHoleData.IsQuotedDb[connectionIndex]),
                _ => new OracleDataProvider(WormHoleData.ConnectionStrings[connectionIndex]),
            };
        }

        internal bool CheckIfForcedUpdate()
        {
            return BHStaticSettings.AutoUpdate && (BHStaticSettings.IsDevMove || CliCommand.ForceAction);
        }

        internal bool IsLite(int connectionIndex)
        {
            bool lite = false;

            if (WormHoleData.DbTypes[connectionIndex] == BlackHoleSqlTypes.SqlLite)
            {
                lite = true;
            }

            return lite;
        }

        internal string GetPrimaryKeyCommand(int connectionIndex)
        {
            return BHStaticSettings.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $"{GetQuotes("Id",connectionIndex)} INT IDENTITY(1,1) PRIMARY KEY NOT NULL ,",
                BlackHoleSqlTypes.MySql => $"{GetQuotes("Id", connectionIndex)} int AUTO_INCREMENT primary key NOT NULL ,",
                BlackHoleSqlTypes.Postgres => $"{GetQuotes("Id", connectionIndex)} SERIAL PRIMARY KEY ,",
                BlackHoleSqlTypes.SqlLite => $"{GetQuotes("Id", connectionIndex)} INTEGER PRIMARY KEY AUTOINCREMENT ,",
                _ => $"{GetQuotes("Id", connectionIndex)} NUMBER(8,0) GENERATED ALWAYS AS IDENTITY PRIMARY KEY ,",
            };
        }

        internal string GetStringPrimaryKeyCommand(int connectionIndex)
        {
            return BHStaticSettings.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $"{GetQuotes("Id", connectionIndex)} NVARCHAR(50) PRIMARY KEY NOT NULL ,",
                BlackHoleSqlTypes.MySql => $"{GetQuotes("Id", connectionIndex)} varchar(50) NOT NULL PRIMARY KEY ,",
                BlackHoleSqlTypes.Postgres => $"{GetQuotes("Id", connectionIndex)} varchar(50) NOT NULL PRIMARY KEY ,",
                BlackHoleSqlTypes.SqlLite => $"{GetQuotes("Id", connectionIndex)} varchar(50) PRIMARY KEY ,",
                _ => $"{GetQuotes("Id", connectionIndex)} varchar2(50) NOT NULL PRIMARY KEY ,",
            };
        }

        internal string GetGuidPrimaryKeyCommand(int connectionIndex)
        {
            return BHStaticSettings.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $"{GetQuotes("Id", connectionIndex)} UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()) ,",
                BlackHoleSqlTypes.MySql => $"{GetQuotes("Id", connectionIndex)} varchar(36) NOT NULL PRIMARY KEY ,",
                BlackHoleSqlTypes.Postgres => $"{GetQuotes("Id", connectionIndex)} uuid DEFAULT gen_random_uuid() PRIMARY KEY ,",
                BlackHoleSqlTypes.SqlLite => $"{GetQuotes("Id", connectionIndex)} varchar(36) PRIMARY KEY ,",
                _ => $"{GetQuotes("Id", connectionIndex)} varchar2(36) NOT NULL PRIMARY KEY ,",
            };
        }

        internal string GetCompositePrimaryKeyCommand(Type propType, string columName, int connectionIndex)
        {
            if(propType == typeof(Guid))
            {
                return BHStaticSettings.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => $"{GetQuotes(columName, connectionIndex)} UNIQUEIDENTIFIER DEFAULT (NEWID()) ,",
                    BlackHoleSqlTypes.Postgres => $"{GetQuotes(columName, connectionIndex)} uuid DEFAULT gen_random_uuid() ,",
                    _ => string.Empty,
                };
            }

            if(propType == typeof(long))
            {
                return BHStaticSettings.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => $"{GetQuotes(columName, connectionIndex)} BIGINT IDENTITY(1,1) NOT NULL ,",
                    BlackHoleSqlTypes.MySql => $"{GetQuotes(columName, connectionIndex)} bigint AUTO_INCREMENT NOT NULL ,",
                    BlackHoleSqlTypes.Postgres => $"{GetQuotes(columName, connectionIndex)} BIGSERIAL ,",
                    BlackHoleSqlTypes.SqlLite => $"{GetQuotes(columName, connectionIndex)} bigint default (last_insert_rowid() + 1) NOT NULL ,",
                    _ => $"{GetQuotes(columName, connectionIndex)} NUMBER(16,0) GENERATED ALWAYS AS IDENTITY ,",
                };
            }

            return BHStaticSettings.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $"{GetQuotes(columName, connectionIndex)} INT IDENTITY(1,1) NOT NULL ,",
                BlackHoleSqlTypes.MySql => $"{GetQuotes(columName, connectionIndex)} int AUTO_INCREMENT NOT NULL ,",
                BlackHoleSqlTypes.Postgres => $"{GetQuotes(columName, connectionIndex)} SERIAL ,",
                BlackHoleSqlTypes.SqlLite => $"{GetQuotes(columName, connectionIndex)} INTEGER AUTOINCREMENT ,",
                _ => $"{GetQuotes(columName, connectionIndex)} NUMBER(8,0) GENERATED ALWAYS AS IDENTITY ,",
            };
        }

        internal bool SkipQuotesOnDb(int connectionIndex)
        {
            return SkipQuotes(connectionIndex);
        }

        internal string GetServerConnection(int connectionIndex)
        {
            return WormHoleData.ServerConnections[connectionIndex];
        }

        internal int GetSqlTypeId(int connectionIndex)
        {
            return WormHoleData.DbTypes[connectionIndex] switch
            {
                BlackHoleSqlTypes.SqlServer => 0,
                BlackHoleSqlTypes.MySql => 1,
                BlackHoleSqlTypes.Postgres => 2,
                BlackHoleSqlTypes.SqlLite => 3,
                _=> 4
            };
        }

        internal bool GetOpenPKConstraint(int connectionIndex)
        {
            return WormHoleData.DbTypes[connectionIndex] switch
            {
                BlackHoleSqlTypes.SqlServer => true,
                BlackHoleSqlTypes.MySql => false,
                BlackHoleSqlTypes.Postgres => false,
                BlackHoleSqlTypes.SqlLite => false,
                _ => true,
            };
        }

        internal bool SetDbDateFormat(IDataProvider _executionProvider, int connectionIndex)
        {
            string getDateCommand = WormHoleData.DbTypes[connectionIndex] switch
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
                WormHoleData.DbDateFormats[connectionIndex] = AnalyzeDateFormat(dateFormat);
            }
            return true;
        }

        private string AnalyzeDateFormat(string dateFormat)
        {
            if(BHStaticSettings.DatabaseType == BlackHoleSqlTypes.Oracle)
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

        internal string[] SqlDatatypesTranslation(int connectionIndex)
        {
            string[] SqlDatatypes = new string[12];
            switch (WormHoleData.DbTypes[connectionIndex])
            {
                case BlackHoleSqlTypes.SqlServer:
                    SqlDatatypes = new[] { "nvarchar", "char", "smallint", "int", "bigint", "decimal", "smallmoney", "float", "uniqueidentifier", "bit", "datetime", "datetimeoffset", "time", "varbinary" };
                    break;
                case BlackHoleSqlTypes.MySql:
                    SqlDatatypes = new[] { "varchar", "char","smallint", "int", "bigint", "decimal", "float", "double", "varchar(36)", "bit", "datetime", "varbinary" };
                    break;
                case BlackHoleSqlTypes.Postgres:
                    SqlDatatypes = new[] { "varchar", "char", "int2", "int4", "int8", "numeric", "float4", "float8", "uuid", "bool", "timestamp", "timestamptz", "time", "bytea" };
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

        internal string TableSchemaCheck(int connectionIndex)
        {
            if (WormHoleData.DbDefaultSchemas[connectionIndex] != string.Empty)
            {
                return $"and table_schema = '{WormHoleData.DbDefaultSchemas[connectionIndex]}'";
            }

            return string.Empty;
        }

        internal string GetDatabaseSchema(int connectionIndex)
        {
            if (WormHoleData.DbDefaultSchemas[connectionIndex] != string.Empty)
            {
                return $"{WormHoleData.DbDefaultSchemas[connectionIndex]}.";
            }
            return string.Empty;
        }

        internal string GetDatabaseSchemaFk()
        {
            if (BHStaticSettings.DatabaseSchema != string.Empty)
            {
                return $"{BHStaticSettings.DatabaseSchema}";
            }
            return string.Empty;
        }

        internal string GetDatabaseName(int connectionIndex)
        {
            if (WormHoleData.DbTypes[connectionIndex] != BlackHoleSqlTypes.SqlLite)
            {
                try
                {
                    string[] dbLinkSplit = WormHoleData.DatabaseNames[connectionIndex].Split("=");
                    return dbLinkSplit[1];
                }
                catch { return string.Empty; }
            }
            else
            {
                return WormHoleData.DatabaseNames[connectionIndex];
            }
        }

        private bool SkipQuotes(int connectionIndex)
        {
            return WormHoleData.DbTypes[connectionIndex] switch
            {
                BlackHoleSqlTypes.SqlServer => BHStaticSettings.IsQuotedDatabase ? false : true,
                BlackHoleSqlTypes.MySql => true,
                BlackHoleSqlTypes.Postgres => false,
                BlackHoleSqlTypes.SqlLite => BHStaticSettings.IsQuotedDatabase ? false : true,
                _ => false,
            };
        }

        private string GetQuotes(string propName, int connectionIndex)
        {
            if (!SkipQuotes(connectionIndex))
            {
                return $@"""{propName}""";
            }
            return propName;
        }

        internal string GetOwnerName(int connectionIndex)
        {
            BlackHoleSqlTypes dbType = WormHoleData.DbTypes[connectionIndex];

            if (dbType != BlackHoleSqlTypes.SqlLite && dbType != BlackHoleSqlTypes.Oracle)
            {
                try
                {
                    if(WormHoleData.OwnerNames[connectionIndex] != string.Empty)
                    {
                        string[] dbLinkSplit = WormHoleData.OwnerNames[connectionIndex].Split("=");
                        return dbLinkSplit[1];
                    }
                }
                catch { return string.Empty; }
            }
            return string.Empty;
        }

        internal bool IsUsingOracleProduct(int connectionIndex)
        {
            if (WormHoleData.DbTypes[connectionIndex] == BlackHoleSqlTypes.Oracle || WormHoleData.DbTypes[connectionIndex] == BlackHoleSqlTypes.MySql)
            {
                return true;
            }
            return false;
        }

        internal string[] GetSafeTransactionTry(int connectionIndex)
        {
            return WormHoleData.DbTypes[connectionIndex] switch
            {
                BlackHoleSqlTypes.SqlServer => new string[] { "BEGIN TRANSACTION BEGIN TRY ", " COMMIT END TRY BEGIN CATCH ROLLBACK; THROW; END CATCH" },
                _ => new string[] { "do language plpgsql $$ begin ", " exception when others then raise notice 'Transaction rolled back'; " +
                "raise EXCEPTION '% %', SQLERRM, SQLSTATE; end; $$"}
            };
        }

        internal string GetColumnModifyCommand(int connectionIndex)
        {
            if (IsUsingOracleProduct(connectionIndex))
            {
                return "MODIFY";
            }

            return "ALTER COLUMN";
        }
    }
}
