using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHParsingColumnScanner
    {
        internal ColumnScanResult ParseColumnToProperty(TableParsingInfo tableColumnInfo)
        {
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    return GetSqlServerColumn(tableColumnInfo, false);
                case BlackHoleSqlTypes.MySql:
                    return GetMySqlColumn(tableColumnInfo, false);
                case BlackHoleSqlTypes.Postgres:
                    return GetNpgSqlColumn(tableColumnInfo, false);
                case BlackHoleSqlTypes.Oracle:
                    return GetOracleColumn(tableColumnInfo, false);
                default:
                    return GetSqLiteColumn(tableColumnInfo, false);
            }
        }

        internal ColumnScanResult ParsePrimaryKeyToProperty(TableParsingInfo tableColumnInfo)
        {
            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    return GetSqlServerColumn(tableColumnInfo, true);
                case BlackHoleSqlTypes.MySql:
                    return GetMySqlColumn(tableColumnInfo, true);
                case BlackHoleSqlTypes.Postgres:
                    return GetNpgSqlColumn(tableColumnInfo, true);
                case BlackHoleSqlTypes.Oracle:
                    return GetOracleColumn(tableColumnInfo, true);
                default:
                    return GetSqLiteColumn(tableColumnInfo, true);
            }
        }

        internal ColumnScanResult GetSqlServerColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new ColumnScanResult();
            scanResult.UnidentifiedColumn = false;

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
            ColumnScanResult scanResult = new ColumnScanResult();
            scanResult.UnidentifiedColumn = false;

            if (isPrimaryKey)
            {
                switch (tableColumnInfo.DataType.ToLower())
                {
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
                    case "smallint":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "integer":
                        scanResult.PropertyNameForColumn = "int";
                        break;
                    case "bigint":
                        scanResult.PropertyNameForColumn= "long";
                        break;
                    case "real":
                        scanResult.PropertyNameForColumn = "float";
                        break;
                    case "double":
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
                    case "bit":
                        scanResult.PropertyNameForColumn = "bool";
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
            ColumnScanResult scanResult = new ColumnScanResult();
            scanResult.UnidentifiedColumn = false;

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
            ColumnScanResult scanResult = new ColumnScanResult();
            scanResult.UnidentifiedColumn = false;

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
            ColumnScanResult scanResult = new ColumnScanResult();
            return scanResult;
        }
    }
}
