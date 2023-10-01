using BlackHole.Enums;
using BlackHole.Statics;
using System.Globalization;

namespace BlackHole.Internal
{
    internal class BHParsingColumnScanner
    {
        internal ColumnScanResult ParseColumnToProperty(TableParsingInfo tableColumnInfo)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => GetSqlServerColumn(tableColumnInfo, false),
                BlackHoleSqlTypes.MySql => GetMySqlColumn(tableColumnInfo, false),
                BlackHoleSqlTypes.Postgres => GetNpgSqlColumn(tableColumnInfo, false),
                BlackHoleSqlTypes.Oracle => GetOracleColumn(tableColumnInfo, false),
                _ => GetSqLiteColumn(tableColumnInfo, false),
            };
        }

        internal ColumnScanResult ParsePrimaryKeyToProperty(TableParsingInfo tableColumnInfo)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => GetSqlServerColumn(tableColumnInfo, true),
                BlackHoleSqlTypes.MySql => GetMySqlColumn(tableColumnInfo, true),
                BlackHoleSqlTypes.Postgres => GetNpgSqlColumn(tableColumnInfo, true),
                BlackHoleSqlTypes.Oracle => GetOracleColumn(tableColumnInfo, true),
                _ => GetSqLiteColumn(tableColumnInfo, true),
            };
        }

        internal PKConfiguration? CheckPrimaryKeySettings(List<TableParsingInfo> tableColumnInfo)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => CheckServerPK(tableColumnInfo),
                BlackHoleSqlTypes.MySql => CheckMySqlPK(tableColumnInfo),
                BlackHoleSqlTypes.Postgres => CheckNpgPK(tableColumnInfo),
                BlackHoleSqlTypes.Oracle => CheckOraclePK(tableColumnInfo),
                _ => CheckLitePK(tableColumnInfo),
            };
        }

        internal PKConfiguration? CheckNpgPK(List<TableParsingInfo> tableColumnInfo)
        {
            List<TableParsingInfo> autoIncrementUuid = tableColumnInfo.Where(x => x.DefaultValue.ToLower().Contains("gen_random_uuid()")).ToList();
            List<TableParsingInfo> autoIncrementInteger = tableColumnInfo.Where(x => x.DefaultValue.ToLower().Contains("nextval(")).ToList();
            string WarningMessage = string.Empty;
            string MainPrimaryKey = string.Empty;

            if (autoIncrementUuid.Any() && autoIncrementInteger.Any())
            {
                WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementUuid[0].TableName}'. Please Fix your database before running this project.";
            }

            if (autoIncrementInteger.Any())
            {
                if (autoIncrementInteger.Count > 1)
                {
                    WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementInteger[0].TableName}'. Please Fix your database before running this project.";
                }

                return new PKConfiguration
                {
                    MainPrimaryKey = autoIncrementInteger[0].ColumnName,
                    HasAutoIncrement = true,
                    WarningMessage = WarningMessage,
                    PKCount = tableColumnInfo.Count
                };
            }

            if (autoIncrementUuid.Any())
            {
                if (autoIncrementUuid.Count > 1)
                {
                    WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementUuid[0].TableName}'. Please Fix your database before running this project.";
                }

                return new PKConfiguration
                {
                    MainPrimaryKey = autoIncrementUuid[0].ColumnName,
                    HasAutoIncrement = true,
                    WarningMessage = WarningMessage,
                    PKCount = tableColumnInfo.Count
                };
            }

            if (tableColumnInfo.Any())
            {
                MainPrimaryKey = tableColumnInfo[0].ColumnName;
            }

            return new PKConfiguration
            {
                MainPrimaryKey = MainPrimaryKey,
                HasAutoIncrement = false,
                WarningMessage = WarningMessage,
                PKCount = tableColumnInfo.Count
            };
        }

        internal PKConfiguration? CheckServerPK(List<TableParsingInfo> tableColumnInfo)
        {
            List<TableParsingInfo> autoIncrementUuid = tableColumnInfo.Where(x => (x.DefaultValue.ToLower().Contains("newid()")
            || x.DefaultValue.ToLower().Contains("newsequentialid()")) && x.DataType.ToLower() == "uniqueidentifier").ToList();
            List<TableParsingInfo> autoIncrementInteger = tableColumnInfo.Where(x => x.IsIdentity).ToList();
            string WarningMessage = string.Empty;
            string MainPrimaryKey = string.Empty;

            if (autoIncrementUuid.Any() && autoIncrementInteger.Any())
            {
                WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementUuid[0].TableName}'. Please Fix your database before running this project.";
            }

            if (autoIncrementInteger.Any())
            {
                if (autoIncrementInteger.Count > 1)
                {
                    WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementInteger[0].TableName}'. Please Fix your database before running this project.";
                }

                return new PKConfiguration
                {
                    MainPrimaryKey = autoIncrementInteger[0].ColumnName,
                    HasAutoIncrement = true,
                    WarningMessage = WarningMessage,
                    PKCount = tableColumnInfo.Count
                };
            }

            if (autoIncrementUuid.Any())
            {
                if (autoIncrementUuid.Count > 1)
                {
                    WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementUuid[0].TableName}'. Please Fix your database before running this project.";
                }

                return new PKConfiguration
                {
                    MainPrimaryKey = autoIncrementUuid[0].ColumnName,
                    HasAutoIncrement = true,
                    WarningMessage = WarningMessage,
                    PKCount = tableColumnInfo.Count
                };
            }

            if (tableColumnInfo.Any())
            {
                MainPrimaryKey = tableColumnInfo[0].ColumnName;
            }

            return new PKConfiguration
            {
                MainPrimaryKey = MainPrimaryKey,
                HasAutoIncrement = false,
                WarningMessage = WarningMessage,
                PKCount = tableColumnInfo.Count
            };
        }
        internal PKConfiguration? CheckOraclePK(List<TableParsingInfo> tableColumnInfo)
        {
            List<TableParsingInfo> autoIncrementColumn = tableColumnInfo.Where(x => x.DefaultValue.ToLower().Contains(".nextval")).ToList();
            string WarningMessage = string.Empty;
            string MainPrimaryKey = string.Empty;

            if (autoIncrementColumn.Any())
            {
                if (autoIncrementColumn.Count > 1)
                {
                    WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementColumn[0].TableName}'. Please Fix your database before running this project.";
                }

                return new PKConfiguration
                {
                    MainPrimaryKey = autoIncrementColumn[0].ColumnName,
                    HasAutoIncrement = true,
                    WarningMessage = WarningMessage,
                    PKCount = tableColumnInfo.Count
                };
            }

            if (tableColumnInfo.Any())
            {
                MainPrimaryKey = tableColumnInfo[0].ColumnName;
            }

            return new PKConfiguration
            {
                MainPrimaryKey = MainPrimaryKey,
                HasAutoIncrement = false,
                WarningMessage = WarningMessage,
                PKCount = tableColumnInfo.Count
            };
        }
        internal PKConfiguration CheckMySqlPK(List<TableParsingInfo> tableColumnInfo)
        {
            List<TableParsingInfo> autoIncrementColumn = tableColumnInfo.Where(x => x.Extra.ToLower().Contains("auto_increment")).ToList();
            string WarningMessage = string.Empty;
            string MainPrimaryKey = string.Empty;

            if (autoIncrementColumn.Any())
            {
                if(autoIncrementColumn.Count > 1)
                {
                    WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementColumn[0].TableName}'. Please Fix your database before running this project.";
                }

                return new PKConfiguration
                {
                    MainPrimaryKey = autoIncrementColumn[0].ColumnName,
                    HasAutoIncrement = true,
                    WarningMessage = WarningMessage,
                    PKCount = tableColumnInfo.Count
                };
            }

            if (tableColumnInfo.Any())
            {
                MainPrimaryKey = tableColumnInfo[0].ColumnName;
            }

            return new PKConfiguration
            {
                MainPrimaryKey = MainPrimaryKey,
                HasAutoIncrement = false,
                WarningMessage = WarningMessage,
                PKCount = tableColumnInfo.Count
            };
        }

        internal PKConfiguration CheckLitePK(List<TableParsingInfo> tableColumnInfo)
        {
            List<TableParsingInfo> autoIncrementColumn = tableColumnInfo.Where(x => x.Extra.ToLower().Contains("auto_increment")).ToList();

            string WarningMessage = string.Empty;
            string MainPrimaryKey = string.Empty;

            if (autoIncrementColumn.Any())
            {
                if (autoIncrementColumn.Count > 1)
                {
                    WarningMessage = $"More than one Primary Keys with auto increment, were found in Table '{autoIncrementColumn[0].TableName}'. Please Fix your database before running this project.";
                }

                return new PKConfiguration
                {
                    MainPrimaryKey = autoIncrementColumn[0].ColumnName,
                    HasAutoIncrement = true,
                    WarningMessage = WarningMessage,
                    PKCount = tableColumnInfo.Count
                };
            }

            if (tableColumnInfo.Any())
            {
                MainPrimaryKey = tableColumnInfo[0].ColumnName;
            }

            return new PKConfiguration
            {
                MainPrimaryKey = MainPrimaryKey,
                HasAutoIncrement = false,
                WarningMessage = WarningMessage,
                PKCount = tableColumnInfo.Count
            };
        }

        internal ColumnScanResult GetSqlServerColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new()
            {
                UnidentifiedColumn = false
            };

            if (isPrimaryKey)
            {
                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "int":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "nvarchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "uniqueidentifier":
                        scanResult.PropertyNameForColumn = "Guid";
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }
            }
            else
            {
                bool isDateTime = false;

                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "bigint":
                        scanResult.PropertyNameForColumn = "long";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "varbinary":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "bit":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "date":
                        isDateTime = true;
                        break;
                    case "datetime":
                        isDateTime = true;
                        break;
                    case "datetime2":
                        isDateTime = true;
                        break;
                    case "decimal":
                        scanResult.PropertyNameForColumn = "decimal";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, true, false);
                        break;
                    case "float":
                        scanResult.PropertyNameForColumn = "double";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "binary":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "int":
                        scanResult.PropertyNameForColumn = "int";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "nchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "money":
                        scanResult.PropertyNameForColumn = "decimal";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, true, false);
                        break;
                    case "ntext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "nvarchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "real":
                        scanResult.PropertyNameForColumn = "short";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "timestamp":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "smallint":
                        scanResult.PropertyNameForColumn = "short";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "smallmoney":
                        scanResult.PropertyNameForColumn = "decimal";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, true, false);
                        break;
                    case "variant":
                        scanResult.PropertyNameForColumn = "object?";
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "time":
                        scanResult.PropertyNameForColumn = "TimeSpan";
                        break;
                    case "tinyint":
                        scanResult.PropertyNameForColumn = "byte";
                        break;
                    case "uniqueidentifier":
                        scanResult.PropertyNameForColumn = "Guid";
                        break;
                    case "varchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "xml":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }
                
                if (isDateTime)
                {
                    if (tableColumnInfo.Nullable)
                    {
                        scanResult.PropertyNameForColumn = "DateTime?";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "DateTime";
                    }
                    scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, true, false, false);
                }
            }
            return scanResult;
        }

        internal ColumnScanResult GetNpgSqlColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new()
            {
                UnidentifiedColumn = false
            };

            if (isPrimaryKey)
            {
                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "int4":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "serial":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "integer":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "varchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "uuid":
                        scanResult.PropertyNameForColumn = "Guid";
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }
            }
            else
            {
                bool isDateTime = false;

                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "boolean":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "varchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "bit":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "bool":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "smallint":
                        scanResult.PropertyNameForColumn = "short";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "int2":
                        scanResult.PropertyNameForColumn = "short";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "int4":
                        scanResult.PropertyNameForColumn = "int";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "integer":
                        scanResult.PropertyNameForColumn = "int";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "bigint":
                        scanResult.PropertyNameForColumn= "long";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "int8":
                        scanResult.PropertyNameForColumn = "long";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "real":
                        scanResult.PropertyNameForColumn = "float";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, true);
                        break;
                    case "float4":
                        scanResult.PropertyNameForColumn = "float";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, true);
                        break;
                    case "double":
                        scanResult.PropertyNameForColumn = "double";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "float8":
                        scanResult.PropertyNameForColumn = "double";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "numeric":
                        scanResult.PropertyNameForColumn = "decimal";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, true, false);
                        break;
                    case "money":
                        scanResult.PropertyNameForColumn = "decimal";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, true, false);
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "character":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "citext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "json":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "jsonb":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "xml":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "uuid":
                        scanResult.PropertyNameForColumn = "Guid";
                        break;
                    case "bytea":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "timestamp":
                        isDateTime = true;
                        break;
                    case "date":
                        isDateTime = true;
                        break;
                    case "time":
                        scanResult.PropertyNameForColumn = "TimeSpan";
                        break;
                    case "interval":
                        scanResult.PropertyNameForColumn = "TimeSpan";
                        break;
                    case "name":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }

                if (isDateTime)
                {
                    if (tableColumnInfo.Nullable)
                    {
                        scanResult.PropertyNameForColumn = "DateTime?";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "DateTime";
                    }
                    scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, true, false, false);
                }
            }

            return scanResult;
        }

        internal ColumnScanResult GetOracleColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new()
            {
                UnidentifiedColumn = false
            };

            if (isPrimaryKey)
            {
                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "number":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "varchar2":
                        if (tableColumnInfo.MaxLength == 36)
                        {
                            scanResult.PropertyNameForColumn = "Guid";
                        }
                        else
                        {
                            scanResult.PropertyNameForColumn = "string";
                            scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        }
                        break;
                    case "nvarchar2":
                        if (tableColumnInfo.MaxLength == 36)
                        {
                            scanResult.PropertyNameForColumn = "Guid";
                        }
                        else
                        {
                            scanResult.PropertyNameForColumn = "string";
                            scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        }
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }
            }
            else
            {
                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "number":
                        scanResult = GetNumericDataType(tableColumnInfo);
                        if(scanResult.PropertyNameForColumn == string.Empty)
                        {
                            scanResult.UnidentifiedColumn = true;
                        }
                        break;
                    case "date":
                        if (tableColumnInfo.Nullable)
                        {
                            scanResult.PropertyNameForColumn = "DateTime?";
                        }
                        else
                        {
                            scanResult.PropertyNameForColumn = "DateTime";
                        }
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, true, false, false);
                        break;
                    case "varchar2":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "blob":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "raw6":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "raw5":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "long":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "nchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "nvarchar2":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "clob":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "nclob":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "rowid":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "urowid":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }

                if (tableColumnInfo.DataType.ToLower().Contains("timestamp"))
                {
                    if (tableColumnInfo.Nullable)
                    {
                        scanResult.PropertyNameForColumn = "DateTime?";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "DateTime";
                    }
                    scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, true, false, false);
                    scanResult.UnidentifiedColumn = false;
                }

                if(scanResult.UnidentifiedColumn && tableColumnInfo.DataType.ToLower().Contains("raw"))
                {
                    if(tableColumnInfo.MaxLength == 16)
                    {
                        scanResult.PropertyNameForColumn = "Guid";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "byte[]?";
                    }

                    scanResult.UnidentifiedColumn = false;
                }
            }

            return scanResult;
        }

        internal string DefaultValueCheck(TableParsingInfo columnInfo, bool isString, bool isDate, bool isDecimal, bool isFloat)
        {
            if (!columnInfo.PrimaryKey && !string.IsNullOrEmpty(columnInfo.DefaultValue))
            {
                string[] testValue = columnInfo.DefaultValue.Replace("(", "").Replace(")", "").Split("'");

                if (testValue.Length > 2)
                {
                    string mainValue = testValue[1];

                    if (isDate)
                    {
                        if (DateTime.TryParseExact(mainValue, DatabaseStatics.DbDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDt))
                        {
                            return $" = new DateTime({parseDt.Year},{parseDt.Month},{parseDt.Day});";
                        }
                    }

                    if (isString)
                    {
                        return $@" = ""{mainValue}"";";
                    }
                }

                if (testValue.Length > 0)
                {
                    string mainValue = testValue[0];

                    if (Double.TryParse(mainValue, out double result))
                    {
                        string numberType = string.Empty;
                        if (isDecimal)
                        {
                            numberType = "m";
                        }

                        if (isFloat)
                        {
                            numberType = "f";
                        }

                        return $" = {result}{numberType};";
                    }
                }
            }

            if (isString)
            {
                return " = string.Empty;";
            }

            return string.Empty;
        }

        private ColumnScanResult GetNumericDataType(TableParsingInfo columnInfo)
        {
            ColumnScanResult result = new();
            if(columnInfo.NumPrecision == 1)
            {
                result.PropertyNameForColumn = "bool";
                return result;
            }

            if (columnInfo.NumPrecision > 18)
            {
                result.PropertyNameForColumn = "float";
                result.DefaultValue = DefaultValueCheck(columnInfo, false, false, false, true);
                return result;
            }

            if (columnInfo.NumPrecision > 16)
            {
                result.PropertyNameForColumn = "double";
                result.DefaultValue = DefaultValueCheck(columnInfo, false, false, false, false);
                return result;
            }

            if (columnInfo.NumPrecision > 13)
            {
                result.PropertyNameForColumn = "long";
                result.DefaultValue = DefaultValueCheck(columnInfo, false, false, false, false);
                return result;
            }

            if (columnInfo.NumPrecision > 9)
            {
                result.PropertyNameForColumn = "decimal";
                result.DefaultValue = DefaultValueCheck(columnInfo, false, false, true, false);
                return result;
            }

            if (columnInfo.NumPrecision > 4)
            {
                result.PropertyNameForColumn = "int";
                result.DefaultValue = DefaultValueCheck(columnInfo, false, false, false, false);
                return result;
            }

            if (columnInfo.NumPrecision > 1)
            {
                result.PropertyNameForColumn = "short";
                result.DefaultValue = DefaultValueCheck(columnInfo, false, false, false, false);
                return result;
            }

            result.PropertyNameForColumn = string.Empty;
            return result;
        }

        internal ColumnScanResult GetMySqlColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new()
            {
                UnidentifiedColumn = false
            };

            if (isPrimaryKey)
            {
                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "int":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "varchar":
                        if (tableColumnInfo.MaxLength == 36)
                        {
                            scanResult.PropertyNameForColumn = "Guid";
                        }
                        else
                        {
                            scanResult.PropertyNameForColumn = "string";
                            scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        }
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }
            }
            else
            {
                bool isDateTime = false;

                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "bigint":
                        scanResult.PropertyNameForColumn = "long";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "bit":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "char";
                        break;
                    case "date":
                        isDateTime = true;
                        break;
                    case "datetime":
                        isDateTime = true;
                        break;
                    case "decimal":
                        scanResult.PropertyNameForColumn = "decimal";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, true, false);
                        break;
                    case "float":
                        scanResult.PropertyNameForColumn = "short";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "int":
                        scanResult.PropertyNameForColumn = "int";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "longtext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "mediumint":
                        scanResult.PropertyNameForColumn = "int";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "mediumtext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "smallint":
                        scanResult.PropertyNameForColumn = "short";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "time":
                        isDateTime = true;
                        break;
                    case "timestamp":
                        isDateTime = true;
                        break;
                    case "tinyint":
                        scanResult.PropertyNameForColumn = "byte";
                        break;
                    case "tinytext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "varbinary":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "varchar":
                        if(tableColumnInfo.MaxLength == 36)
                        {
                            scanResult.PropertyNameForColumn = "Guid";
                        }
                        else
                        {
                            scanResult.PropertyNameForColumn = "string";
                            scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        }
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }

                if (isDateTime)
                {
                    if (tableColumnInfo.Nullable)
                    {
                        scanResult.PropertyNameForColumn = "DateTime?";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "DateTime";
                    }
                    scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, true, false, false);
                }
            }

            return scanResult;
        }

        internal ColumnScanResult GetSqLiteColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new()
            {
                UnidentifiedColumn = false
            };

            if (isPrimaryKey)
            {
                scanResult.UnidentifiedColumn = true;

                if (scanResult.UnidentifiedColumn && tableColumnInfo.DataType.ToLower().Contains("text"))
                {
                    int textLength = GetSqLiteLength(tableColumnInfo.DataType);

                    if (textLength == 36)
                    {
                        scanResult.PropertyNameForColumn = "Guid";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                    }

                    scanResult.UnidentifiedColumn = false;
                }

                if (scanResult.UnidentifiedColumn && tableColumnInfo.DataType.ToLower().Contains("varchar"))
                {
                    int textLength = GetSqLiteLength(tableColumnInfo.DataType);

                    if (textLength == 36)
                    {
                        scanResult.PropertyNameForColumn = "Guid";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                    }

                    scanResult.UnidentifiedColumn = false;
                }

                if (scanResult.UnidentifiedColumn && tableColumnInfo.DataType.ToLower().Contains("char"))
                {
                    int textLength = GetSqLiteLength(tableColumnInfo.DataType);

                    if (textLength == 36)
                    {
                        scanResult.PropertyNameForColumn = "Guid";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                    }

                    scanResult.UnidentifiedColumn = false;
                }

                if(scanResult.UnidentifiedColumn && tableColumnInfo.DataType.ToLower().Contains("integer"))
                {
                    scanResult.PropertyNameForColumn = "int";
                    scanResult.UnidentifiedColumn = false;
                }
            }
            else
            {
                bool isDateTime = false;

                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "boolean":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "integer":
                        scanResult.PropertyNameForColumn = "int";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "blob":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "datetime":
                        isDateTime = true;
                        break;
                    case "varchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "char";
                        break;
                    case "int2":
                        scanResult.PropertyNameForColumn = "short";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "bigint":
                        scanResult.PropertyNameForColumn = "long";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "decimal":
                        scanResult.PropertyNameForColumn = "decimal";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, true, false);
                        break;
                    case "real":
                        scanResult.PropertyNameForColumn = "double";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    case "float":
                        scanResult.PropertyNameForColumn = "float";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, true);
                        break;
                    case "numeric":
                        scanResult.PropertyNameForColumn = "double";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, false, false, false);
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }

                if (scanResult.UnidentifiedColumn && tableColumnInfo.DataType.ToLower().Contains("text"))
                {
                    int textLength = GetSqLiteLength(tableColumnInfo.DataType);

                    if(textLength == 36)
                    {
                        scanResult.PropertyNameForColumn = "Guid";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                    }

                    scanResult.UnidentifiedColumn = false;
                }

                if (scanResult.UnidentifiedColumn && tableColumnInfo.DataType.ToLower().Contains("varchar"))
                {
                    int textLength = GetSqLiteLength(tableColumnInfo.DataType);

                    if (textLength == 36)
                    {
                        scanResult.PropertyNameForColumn = "Guid";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                    }

                    scanResult.UnidentifiedColumn = false;
                }

                if (scanResult.UnidentifiedColumn && tableColumnInfo.DataType.ToLower().Contains("char"))
                {
                    int textLength = GetSqLiteLength(tableColumnInfo.DataType);

                    if (textLength == 36)
                    {
                        scanResult.PropertyNameForColumn = "Guid";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, true, false, false, false);
                    }

                    scanResult.UnidentifiedColumn = false;
                }

                if (isDateTime)
                {
                    if (tableColumnInfo.Nullable)
                    {
                        scanResult.PropertyNameForColumn = "DateTime?";
                    }
                    else
                    {
                        scanResult.PropertyNameForColumn = "DateTime";
                    }
                    scanResult.DefaultValue = DefaultValueCheck(tableColumnInfo, false, true, false, false);
                }
            }
            return scanResult;
        }

        private int GetSqLiteLength(string dataType)
        {
            string[] sizeCheck = dataType.Split("(");

            if(sizeCheck.Length > 1)
            {
                string lengthNumber = sizeCheck[1].Replace(" ","").Replace(")","");

                try
                {
                    return Int32.Parse(lengthNumber);
                }
                catch
                {
                    return 0;
                }
            }

            return 0;
        }
    }
}
