using BlackHole.Enums;
using BlackHole.Statics;

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
                        break;
                    case "varbinary":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "bit":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
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
                        break;
                    case "float":
                        scanResult.PropertyNameForColumn = "double";
                        break;
                    case "binary":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "int":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "nchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "money":
                        scanResult.PropertyNameForColumn = "decimal";
                        break;
                    case "ntext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "nvarchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "real":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "timestamp":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "smallint":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "smallmoney":
                        scanResult.PropertyNameForColumn = "decimal";
                        break;
                    case "variant":
                        scanResult.PropertyNameForColumn = "object?";
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
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
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "xml":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
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
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "bit":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "bool":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "smallint":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "int2":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "int4":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "integer":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "bigint":
                        scanResult.PropertyNameForColumn= "long";
                        break;
                    case "int8":
                        scanResult.PropertyNameForColumn = "long";
                        break;
                    case "real":
                        scanResult.PropertyNameForColumn = "float";
                        break;
                    case "float4":
                        scanResult.PropertyNameForColumn = "float";
                        break;
                    case "double":
                        scanResult.PropertyNameForColumn = "double";
                        break;
                    case "float8":
                        scanResult.PropertyNameForColumn = "double";
                        break;
                    case "numeric":
                        scanResult.PropertyNameForColumn = "decimal";
                        break;
                    case "money":
                        scanResult.PropertyNameForColumn = "decimal";
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "character":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "citext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "json":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "jsonb":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "xml":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
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
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
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
                        string dataType = GetNumericDataType(tableColumnInfo.NumPrecision);
                        if(dataType == string.Empty)
                        {
                            scanResult.UnidentifiedColumn = true;
                        }
                        scanResult.PropertyNameForColumn = dataType;
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
                        break;
                    case "varchar2":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
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
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "nchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "nvarchar2":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "clob":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "nclob":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "rowid":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "urowid":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
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

        private string GetNumericDataType(int NumPrecision)
        {
            if(NumPrecision == 1)
            {
                return "bool";
            }

            if (NumPrecision > 18)
            {
                return "float";
            }

            if (NumPrecision > 16)
            {
                return "double";
            }

            if (NumPrecision > 9)
            {
                return "decimal";
            }

            if (NumPrecision > 1)
            {
                return "int";
            }

            return string.Empty;
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
                        break;
                    case "float":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "int":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "longtext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "mediumint":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "mediumtext":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "smallint":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
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
                        scanResult.DefaultValue = " = string.Empty;"; 
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
                            scanResult.DefaultValue = " = string.Empty;";
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
                        break;
                    case "blob":
                        scanResult.PropertyNameForColumn = "byte[]?";
                        break;
                    case "datetime":
                        isDateTime = true;
                        break;
                    case "varchar":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "char";
                        break;
                    case "int2":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "bigint":
                        scanResult.PropertyNameForColumn = "long";
                        break;
                    case "decimal":
                        scanResult.PropertyNameForColumn = "decimal";
                        break;
                    case "real":
                        scanResult.PropertyNameForColumn = "double";
                        break;
                    case "float":
                        scanResult.PropertyNameForColumn = "float";
                        break;
                    case "numeric":
                        scanResult.PropertyNameForColumn = "double";
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
                        scanResult.DefaultValue = " = string.Empty;";
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
                        scanResult.DefaultValue = " = string.Empty;";
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
                        scanResult.DefaultValue = " = string.Empty;";
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
