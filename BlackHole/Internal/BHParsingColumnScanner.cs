﻿using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHParsingColumnScanner
    {
        private readonly IExecutionProvider _connection;

        internal BHParsingColumnScanner(IExecutionProvider connection)
        {
            _connection = connection;
        }

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
            ColumnScanResult scanResult = new ColumnScanResult();
            
            return scanResult;
        }

        internal ColumnScanResult GetSqlServerColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new ColumnScanResult();

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
                switch (tableColumnInfo.DataType.ToLower())
                {
                    case "bigint":
                        scanResult.PropertyNameForColumn = "long";
                        break;
                    case "varbinary":
                        scanResult.PropertyNameForColumn = "byte[]";
                        break;
                    case "bit":
                        scanResult.PropertyNameForColumn = "bool";
                        break;
                    case "char":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "date":
                        scanResult.PropertyNameForColumn = "DateTime";
                        break;
                    case "datetime":
                        scanResult.PropertyNameForColumn = "DateTime";
                        break;
                    case "datetime2":
                        scanResult.PropertyNameForColumn = "DateTime";
                        break;
                    case "decimal":
                        scanResult.PropertyNameForColumn = "decimal";
                        break;
                    case "float":
                        scanResult.PropertyNameForColumn = "double";
                        break;
                    case "binary":
                        scanResult.PropertyNameForColumn = "byte[]";
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
                        scanResult.PropertyNameForColumn = "byte[]";
                        break;
                    case "smallint":
                        scanResult.PropertyNameForColumn = "short";
                        break;
                    case "smallmoney":
                        scanResult.PropertyNameForColumn = "decimal";
                        break;
                    case "variant":
                        scanResult.PropertyNameForColumn = "object";
                        break;
                    case "text":
                        scanResult.PropertyNameForColumn = "string";
                        scanResult.DefaultValue = " = string.Empty;";
                        break;
                    case "time":
                        scanResult.PropertyNameForColumn = "TimeOnly";
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
                        scanResult.PropertyNameForColumn = "XmlNode";
                        break;
                    default:
                        scanResult.UnidentifiedColumn = true;
                        break;
                }
            }
            return scanResult;
        }

        internal ColumnScanResult GetNpgSqlColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new ColumnScanResult();
            return scanResult;
        }

        internal ColumnScanResult GetOracleColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new ColumnScanResult();
            return scanResult;
        }

        internal ColumnScanResult GetMySqlColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new ColumnScanResult();
            return scanResult;
        }

        internal ColumnScanResult GetSqLiteColumn(TableParsingInfo tableColumnInfo, bool isPrimaryKey)
        {
            ColumnScanResult scanResult = new ColumnScanResult();
            return scanResult;
        }
    }
}
