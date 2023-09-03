﻿using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHDatabaseParser
    {
        private readonly IExecutionProvider connection;
        private readonly IBHDatabaseSelector _multiDatabaseSelector;
        internal BHSqlExportWriter sqlWriter { get; set; }
        internal List<string> IgnoredTables { get; set; } = new List<string>();
        internal List<string> RequiredChanges { get; set; } = new List<string>();
        internal List<string> MinorChanges { get; set; } = new List<string>();

        internal BHDatabaseParser()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
            sqlWriter = new BHSqlExportWriter("ParsingResult", "ParsingReport", "txt");
            sqlWriter.DeleteSqlFolder();

            MinorChanges.Add("-- Minor Changes that will be applied in the StartUp of the Application. Severity: Minor -- ");
            MinorChanges.Add($"");
            IgnoredTables.Add($"");
            IgnoredTables.Add($"--------- -------- ---------- --------- --------");
            IgnoredTables.Add($"-- Ignored Tables In parsing. Severity: High --");
            IgnoredTables.Add($"");
            RequiredChanges.Add($"");
            RequiredChanges.Add($"--------- -------- ---------- --------- --------");
            RequiredChanges.Add($"-- Required Changes In parsing. Severity: Medium --");
            RequiredChanges.Add($"");
        }

        internal int ParseDatabase()
        {
            int result = 0;
            CliLog("\t Please wait while reading the Database. This may take up to 2 minutes in large databases..");

            List<TableAspectsInfo> tableInfo = GetDatabaseInformation();
            BHParsingColumnScanner columnScanner = new BHParsingColumnScanner();

            DbParsingStates dbState = CheckCompatibility(tableInfo, columnScanner);
            SaveParsingReport();

            string folderPath = Path.Combine(DatabaseStatics.DataPath, "ParsingReport");

            switch (dbState)
            {
                case DbParsingStates.Proceed:
                    EntityCodeGenerator(tableInfo, columnScanner);
                    CliLog("");
                    CliLog("The database is compatible with the BlackHole Orm.");
                    CliLog("Parsing has been successfully completed.");
                    result = 0;
                    break;
                case DbParsingStates.MinorChanges:
                    EntityCodeGenerator(tableInfo, columnScanner);
                    CliLog("");
                    CliLog("Parsing has been successfully completed.");
                    CliLog("");
                    CliLog("Some minor changer are required to the Database in order to be compatible with BlackHole Orm");
                    CliLog("These changes will be done when your run your application.");
                    CliLog("");
                    CliLog("The detected required changes are stored in the parsing report at:");
                    CliLog($"\t {folderPath}");
                    result = 0;
                    break;
                case DbParsingStates.ChangesRequired:
                    CliLog("");
                    CliLog("The parsing didn't proceed, because some changes are required to the Database, in order to be compatible with BlackHole Orm.");
                    CliLog("");
                    CliLog("The required changes are stored in the parsing report at:");
                    CliLog($"\t {folderPath}");
                    CliLog("");
                    CliLog("If you want to force the parsing process before making the required changes, use the '-f' or '--force' argument with the 'parse' command.");
                    result = 407;
                    break;
                case DbParsingStates.ForceChanges:
                    EntityCodeGenerator(tableInfo, columnScanner);
                    CliLog("");
                    CliLog("The parsing has been forced. Please make the required changes before running your application.");
                    CliLog("");
                    CliLog("The required changes are stored in the parsing report at:");
                    CliLog($"\t {folderPath}");
                    CliLog("");
                    CliLog("Running the application before making the required changes to the Database, might cause 'Damage' or 'data loss' to the Database.");
                    result = 0;
                    break;
                case DbParsingStates.Incompatible:
                    CliLog("");
                    CliLog("This database is Incompatible with the BlackHole Orm.");
                    CliLog("");
                    CliLog("Find more details in the generated report at:");
                    CliLog($"\t {folderPath}");
                    CliLog("");
                    CliLog("If you want to force the parsing and get as many tables as possible, use the '-f' or '--force' argument with the 'parse' command.");
                    CliLog("The incompatible tables will be ignored.");
                    result = 510;
                    break;
            }

            CliLog("");
            CliLog("\t !!Important!!");
            CliLog("\t Make sure to keep a backup of your database before Running your application.");
            CliLog("\t BlackHole Orm and Mikarsoft are not responsible for damage or data loss in your Database.");
            CliLog("");
            return result;
        }

        internal void EntityCodeGenerator(List<TableAspectsInfo> parsingData, BHParsingColumnScanner columnScanner)
        {
            string scriptsPath = Path.Combine(CliCommand.ProjectPath, "BHEntities");
            string applicationName = Path.GetFileName(CliCommand.ProjectPath).Replace(".csproj","");

            CliLog($"Application Name: {applicationName}");

            if (!Directory.Exists(scriptsPath))
            {
                Directory.CreateDirectory(scriptsPath);
            }

            foreach (TableAspectsInfo tableAspectInf in parsingData.Where(x => x.GeneralError == false))
            {
                CliLog($"Creating Entity {tableAspectInf.TableName} in BHEntities Folder...");

                string EntityScript = $" using System;\n using BlackHole.Entities;\n\n namespace {applicationName}.BHEntities \n";
                string PropertiesScript = string.Empty;
                EntityScript += " { \n";
                EntityScript += $"\t public class {tableAspectInf.TableName} :";

                foreach(TableParsingInfo columnInfo in tableAspectInf.TableColumns.Where(x=>x.ColumnName.ToLower() != "inactive"))
                {
                    if (!columnInfo.PrimaryKey)
                    {
                        ColumnScanResult scanResult = columnScanner.ParseColumnToProperty(columnInfo);

                        if (!scanResult.UnidentifiedColumn)
                        {
                            PropertiesScript += GetBHAttribute(columnInfo, scanResult.PropertyNameForColumn);
                            PropertiesScript += $"\t\t public {scanResult.PropertyNameForColumn} {columnInfo.ColumnName}" + " {get; set;}" + $" {scanResult.DefaultValue} \n";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(columnInfo.ReferencedTable))
                        {
                            ColumnScanResult scanPkResult = columnScanner.ParsePrimaryKeyToProperty(columnInfo);
                            EntityScript += $" BlackHoleEntity<{scanPkResult.PropertyNameForColumn}> \n";
                            EntityScript += "\t { \n";
                        }
                    }
                }

                EntityScript += PropertiesScript;
                EntityScript += "\t } \n";
                EntityScript += " } \n";

                string pathFile = Path.Combine(scriptsPath, $"{tableAspectInf.TableName}.cs");

                try
                {
                    using (var tw = new StreamWriter(pathFile, true))
                    {
                        tw.Write(EntityScript);
                    }

                    CliLog($"Created Entity {tableAspectInf.TableName} in BHEntities Folder");
                }
                catch
                {
                    CliLog($"Failed to create Entity {tableAspectInf.TableName} in BHEntities Folder");
                }
            }
        }

        internal DbParsingStates CheckCompatibility(List<TableAspectsInfo> parsingData, BHParsingColumnScanner columnScanner)
        {
            DbParsingStates dbState = DbParsingStates.Proceed;
            string errorMessage = string.Empty;
            bool blockParsing = false;

            foreach (TableAspectsInfo tableAspectInf in parsingData)
            {
                List<TableParsingInfo> primaryKey = tableAspectInf.TableColumns.Where(x=>x.PrimaryKey && string.IsNullOrEmpty(x.ReferencedTable)).ToList();

                TableParsingInfo? inactiveColumn = tableAspectInf.TableColumns.FirstOrDefault(x => x.ColumnName == "Inactive");

                if (primaryKey.Count == 1 && inactiveColumn != null && primaryKey[0].ColumnName == "Id")
                {
                    ColumnScanResult scanPkResult = columnScanner.ParsePrimaryKeyToProperty(primaryKey[0]);

                    if (scanPkResult.UnidentifiedColumn)
                    {
                        errorMessage = $"Column {primaryKey[0].ColumnName} of the Table {tableAspectInf.TableName}, is Incompatible with BlackHoleEntity 's Supported Primary keys.";
                        CliLog(errorMessage);
                        CliLog($"Attempting to parse {tableAspectInf.TableName} Table as BHOpenEntity..");

                        ColumnScanResult scanOtherResult = columnScanner.ParseColumnToProperty(primaryKey[0]);

                        if (scanOtherResult.UnidentifiedColumn)
                        {
                            tableAspectInf.GeneralError = true;

                            if (CliCommand.ForceAction)
                            {
                                errorMessage = $"Table {tableAspectInf.TableName} is not Compatible with BHOpenEntity either and it will not be generated as Entity.";
                                CliLog(errorMessage);
                                dbState = DbParsingStates.ChangesRequired;
                            }
                            else
                            {
                                errorMessage = $"Incompatible Table {tableAspectInf.TableName}, has unidentified column {primaryKey[0].ColumnName}";
                                CliLog(errorMessage);
                                WriteIgnoredTable(tableAspectInf.TableName, errorMessage);
                                blockParsing = true;
                            }
                        }
                        else
                        {
                            tableAspectInf.UseOpenEntity = true;
                        }
                    }
                }
                else
                {
                    tableAspectInf.UseOpenEntity = true;
                }

                if (tableAspectInf.UseOpenEntity)
                {
                    foreach(TableParsingInfo columnInfo in primaryKey)
                    {
                        ColumnScanResult scanOpenPkResult = columnScanner.ParseColumnToProperty(columnInfo);

                        if (scanOpenPkResult.UnidentifiedColumn)
                        {
                            errorMessage = $"Column {columnInfo.ColumnName} of the Table {tableAspectInf.TableName}, is Incompatible with BHOpenEntity 's Supported types.";
                            tableAspectInf.GeneralError = true;

                            if (CliCommand.ForceAction)
                            {
                                errorMessage = $"A primary key of {tableAspectInf.TableName} Table, is not Compatible with BHOpenEntity either and it will not be generated as Entity.";
                                CliLog(errorMessage);
                                dbState = DbParsingStates.ChangesRequired;
                            }
                            else
                            {
                                errorMessage = $"Incompatible Table {tableAspectInf.TableName}, has unidentified column {columnInfo.ColumnName}";
                                CliLog(errorMessage);
                                WriteIgnoredTable(tableAspectInf.TableName, errorMessage);
                                blockParsing = true;
                            }
                        }
                        else
                        {
                            tableAspectInf.CheckPrimaryKeys = true;
                        }
                    }

                    if (tableAspectInf.CheckPrimaryKeys)
                    {
                        tableAspectInf.Configuration = columnScanner.CheckPrimaryKeySettings(primaryKey);
                    }
                }
            }

            if(dbState == DbParsingStates.ChangesRequired && CliCommand.ForceAction)
            {
                dbState = DbParsingStates.ForceChanges;
            }

            if (blockParsing)
            {
                dbState = DbParsingStates.Incompatible;
            }

            return dbState;
        }

        internal void WriteRequiredChange(string TableName, string message)
        {
            RequiredChanges.Add("");
            RequiredChanges.Add($"Change Required at: {TableName}");
            RequiredChanges.Add($"Change: {message}");
        }

        internal void WriteIgnoredTable(string TableName, string message)
        {
            IgnoredTables.Add("");
            IgnoredTables.Add($"Incompatible Table: {TableName}");
            IgnoredTables.Add($"Ignore Reason: {message}");
        }

        internal void WriteMinorChanges(string TableName, string message)
        {
            MinorChanges.Add("");
            MinorChanges.Add($"Minor Change Required at: {TableName}");
            MinorChanges.Add($"Change: {message}");
        }

        internal void SaveParsingReport()
        {
            sqlWriter.AddMultiple(MinorChanges);
            sqlWriter.AddMultiple(RequiredChanges);
            sqlWriter.AddMultiple(IgnoredTables);
            sqlWriter.CreateSqlFile();
        }

        private int GetSqLiteLength(string dataType)
        {
            string[] sizeCheck = dataType.Split("(");

            if (sizeCheck.Length > 1)
            {
                string lengthNumber = sizeCheck[1].Replace(" ", "").Replace(")", "");

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

        internal string GetBHAttribute(TableParsingInfo columnInfo, string dotNetType)
        {
            string VarcharSize = string.Empty;

            if(dotNetType == "string")
            {
                int characterSize = columnInfo.MaxLength;

                if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlServer)
                {
                    characterSize = columnInfo.MaxLength / 2;
                }

                if(DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlLite)
                {
                    characterSize = GetSqLiteLength(columnInfo.DataType);
                }

                if(characterSize == 0)
                {
                    characterSize = 4000;
                }

                if(characterSize != 255)
                {
                    VarcharSize = $"\n\t\t [VarCharSize({characterSize})]";
                }
            }

            if (!string.IsNullOrEmpty(columnInfo.ReferencedTable))
            {
                if (columnInfo.Nullable)
                {
                    return $"{VarcharSize}\n\t\t [ForeignKey(typeof({columnInfo.ReferencedTable}))]\n";
                }
                else
                {
                    return $"{VarcharSize}\n\t\t [ForeignKey(typeof({columnInfo.ReferencedTable}),false)]\n";
                }
            }

            if (!columnInfo.Nullable)
            {
                return $"{VarcharSize}\n\t\t [NotNullable]\n";
            }

            if(VarcharSize != string.Empty)
            {
                return $"{VarcharSize}\n";
            }

            return string.Empty;
        }

        internal List<TableAspectsInfo> GetDatabaseInformation()
        {
            string parseCommand = string.Empty;
            string schemaCheck = GetSchemaCheckCommand();
            List<TableAspectsInfo> tableAspects = new List<TableAspectsInfo>();
            List<TableParsingInfo> parsingData = new List<TableParsingInfo>();

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    parseCommand = @"SELECT distinct tb.name as TableName, c.name as ColumnName,PT.COLUMN_NAME as ReferencedColumn, t.name as DataType, c.max_length as MaxLength,
                        c.precision as NumPrecision , c.scale  as NumScale, c.is_nullable as Nullable, ISNULL(i.is_primary_key, 0) as PrimaryKey,
	                    RC.DELETE_RULE as DeleteRule, TC.TABLE_NAME as ReferencedTable, K.CONSTRAINT_NAME as ConstraintName, CD.COLUMN_DEFAULT as DefaultValue,
						c.is_identity as IsIdentity FROM sys.columns c
                        inner join sys.types t ON c.user_type_id = t.user_type_id
                        inner join sys.tables tb  on tb.object_id = c.object_id
                        left outer join sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                        left outer join sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                        left outer join INFORMATION_SCHEMA.KEY_COLUMN_USAGE K on K.COLUMN_NAME = c.name and K.TABLE_NAME = tb.name
                        left join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC on RC.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                        left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON TC.CONSTRAINT_NAME= RC.UNIQUE_CONSTRAINT_NAME
						left outer join INFORMATION_SCHEMA.COLUMNS CD ON C.name = CD.COLUMN_NAME AND CD.COLUMN_DEFAULT IS NOT NULL
						LEFT OUTER JOIN (SELECT i1.TABLE_NAME,i2.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
						INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME WHERE
						i1.CONSTRAINT_TYPE = 'PRIMARY KEY') PT ON PT.TABLE_NAME = TC.TABLE_NAME " +
                        $" {schemaCheck}";
                    parsingData = connection.Query<TableParsingInfo>(parseCommand, null);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    parseCommand = @"select distinct K.table_name as TableName, K.column_name as ColumnName, K.column_default as DefaultValue, K.udt_name as DataType,
                        K.character_maximum_length as MaxLength, K.numeric_precision as NumPrecision, K.numeric_scale as NumScale,
                        case when K.is_nullable='YES' then true else false end as Nullable, 
                        case when T.constraint_type ='PRIMARY KEY' then true else false end as PrimaryKey,
                        case when T.constraint_type = 'FOREIGN KEY' then CU.table_name else '' end as ReferencedTable,
						case when (T.constraint_type = 'FOREIGN KEY' and K.is_nullable = 'YES') then 'SET NULL'
							 when (T.constraint_type = 'FOREIGN KEY' and K.is_nullable = 'NO') then	'CASCADE'
							 else '' end as DeleteRule, T.CONSTRAINT_NAME as ConstraintName, 
						case when T.constraint_type = 'FOREIGN KEY' then CU.column_name else '' end as ReferencedColumn
						from INFORMATION_SCHEMA.COLUMNS K 
                        left join INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME)
                        left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS T on T.CONSTRAINT_NAME = C.CONSTRAINT_NAME 
                        left join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CU on CU.CONSTRAINT_NAME = T.CONSTRAINT_NAME" +
                        $" {schemaCheck}";
                    parsingData = connection.Query<TableParsingInfo>(parseCommand, null);
                    break;
                case BlackHoleSqlTypes.MySql:
                    parseCommand = @"select distinct K.TABLE_NAME as TableName, K.COLUMN_NAME as ColumnName, K.column_default as DefaultValue,K.EXTRA, K.COLUMN_COMMENT, K.DATA_TYPE as DataType, 
                        K.CHARACTER_MAXIMUM_LENGTH as MaxLength, K.NUMERIC_PRECISION as NumPrecision, K.NUMERIC_SCALE as NumScale , 
                        case when K.IS_NULLABLE = 'YES' THEN true else false end as Nullable,
                        case when K.COLUMN_KEY ='PRI' then true else false end as PrimaryKey,
                        case when (K.COLUMN_KEY ='MUL' and  K.IS_NULLABLE = 'YES') then 'SET NULL'
	                         when (K.COLUMN_KEY ='MUL' and K.IS_NULLABLE = 'NO') then 'CASCADE'
                             else '' end as DeleteRule,
                        C.REFERENCED_TABLE_NAME as ReferencedTable, C.REFERENCED_COLUMN_NAME as ReferencedColumn,
                        C.CONSTRAINT_NAME as ConstraintName from INFORMATION_SCHEMA.COLUMNS K 
		                left outer join information_schema.key_column_usage C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME) " +
                        $" where K.TABLE_SCHEMA ='{schemaCheck}'";
                    parsingData = connection.Query<TableParsingInfo>(parseCommand, null);
                    break;
                case BlackHoleSqlTypes.Oracle:
                    parseCommand = @"select distinct col.table_name as TableName, col.column_name as ColumnName, col.data_type as DataType, col.data_length as MaxLength,
                        col.data_precision as NumPrecision, col.data_scale as NumScale,
                        case when col.nullable = 'Y' then 1 else 0 end as Nullable,
                        case when c.constraint_type = 'P' then 1 else 0 end as PrimaryKey,
                        c.delete_rule as DeleteRule, c.constraint_name as ConstraintName,
                        case when c.constraint_type = 'R' then c_pk.table_name else '' end as ReferencedTable,
                        ccu.column_name as ReferencedColumn,
                        case when col.default_length is null then ''
                           else extractvalue ( dbms_xmlgen.getxmltype
                            ( 'select data_default from user_tab_columns where table_name = ''' || col.table_name || ''' and column_name = ''' || col.column_name || '''' )
                            , '//text()' ) end as DefaultValue
                        from sys.all_tab_columns col
                        left join sys.all_cons_columns a on (a.column_name = col.column_name and a.table_name = col.table_name and a.owner = col.owner and a.position is not null)
                        left join all_constraints c ON a.owner = c.owner AND a.constraint_name = c.constraint_name
                        left JOIN all_constraints c_pk ON c.r_owner = c_pk.owner AND c.r_constraint_name = c_pk.constraint_name
                        left outer join USER_CONS_COLUMNS ccu on ccu.CONSTRAINT_NAME = c.r_constraint_name And c.constraint_type='R'" +
                        $" where col.owner ='{schemaCheck}'";
                    parsingData = connection.Query<TableParsingInfo>(parseCommand, null);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    parsingData = SqLiteParsing();
                    break;
            }

            List<string> TableNames = parsingData.Select(parsingData => parsingData.TableName).Distinct().ToList();

            foreach (string tableName in TableNames)
            {
                List<TableParsingInfo> tableInfo = parsingData.Where(x => x.TableName == tableName).ToList();

                TableAspectsInfo tableAspectInf = new TableAspectsInfo
                {
                    TableName = tableName,
                    TableColumns = tableInfo
                };

                tableAspects.Add(tableAspectInf);
            }

            return tableAspects;
        }

        internal List<TableParsingInfo> SqLiteParsing()
        {
            List<TableParsingInfo> parsingLiteData = new List<TableParsingInfo>();

            List<string> tableNames = connection.Query<string>("SELECT name FROM sqlite_master  where type = 'table' and name != 'sqlite_sequence';", null);

            foreach(string tableName in tableNames)
            {
                List<SqLiteTableInfo> tableInfo = connection.Query<SqLiteTableInfo>($"PRAGMA table_info({tableName});", null);
                List<SqLiteForeignKeySchema> foreignKeys = connection.Query<SqLiteForeignKeySchema>($"PRAGMA foreign_key_list({tableName});", null);
                LiteAutoIncrementInfo? isAutoIncrement = connection.QueryFirst<LiteAutoIncrementInfo>($"SELECT * FROM sqlite_sequence WHERE name='{tableName}'", null);

                foreach(SqLiteTableInfo info in tableInfo)
                {
                    TableParsingInfo parsingLine = new TableParsingInfo
                    {
                        TableName = tableName,
                        ColumnName = info.name,
                        DataType = info.type,
                        Nullable = !info.notnull,
                        PrimaryKey = info.pk > 0,
                        DefaultValue = info.dflt_value
                    };

                    if (info.pk == 1 && isAutoIncrement != null && info.type.ToLower().Contains("integer"))
                    {
                        parsingLine.Extra = "auto_increment";
                    }

                    SqLiteForeignKeySchema? foreignKey = foreignKeys.Where(x => x.from == parsingLine.ColumnName).FirstOrDefault();

                    if(foreignKey != null)
                    {
                        parsingLine.DeleteRule = foreignKey.on_delete;
                        parsingLine.ReferencedTable = foreignKey.table;
                    }

                    parsingLiteData.Add(parsingLine);
                }
            }

            return parsingLiteData;
        }

        internal string GetSchemaCheckCommand()
        {
            string schemaCheck = string.Empty;

            if(DatabaseStatics.DatabaseSchema != string.Empty)
            {
                switch (DatabaseStatics.DatabaseType)
                {
                    case BlackHoleSqlTypes.SqlServer:
                        schemaCheck = $"WHERE SCHEMA_NAME(tb.schema_id) = '{DatabaseStatics.DatabaseSchema}'";
                        break;
                    case BlackHoleSqlTypes.Postgres:
                        schemaCheck = $"where K.table_schema='{DatabaseStatics.DatabaseSchema}'";
                        break;
                    case BlackHoleSqlTypes.MySql:
                        schemaCheck = _multiDatabaseSelector.GetDatabaseName();
                        break;
                    case BlackHoleSqlTypes.Oracle:
                        schemaCheck = _multiDatabaseSelector.GetDatabaseName();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (DatabaseStatics.DatabaseType)
                {
                    case BlackHoleSqlTypes.SqlServer:
                        schemaCheck = $"WHERE SCHEMA_NAME(tb.schema_id) = 'dbo'";
                        break;
                    case BlackHoleSqlTypes.Postgres:
                        schemaCheck = $"where K.table_schema = 'public'";
                        break;
                    case BlackHoleSqlTypes.MySql:
                        schemaCheck = _multiDatabaseSelector.GetDatabaseName();
                        break;
                    case BlackHoleSqlTypes.Oracle:
                        schemaCheck = _multiDatabaseSelector.GetDatabaseName();
                        break;
                    default:
                        break;
                }
            }

            return schemaCheck;
        }

        internal void CliLog(string logText)
        {
            if (CliCommand.CliExecution)
            {
                Console.WriteLine($"_bhLog_{logText}");
            }
        }
    }
}
