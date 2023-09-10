using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.Statics;
using System.Globalization;

namespace BlackHole.Internal
{
    internal class BHDatabaseParser
    {
        private readonly IExecutionProvider connection;
        private readonly IBHDatabaseSelector _multiDatabaseSelector;
        internal BHSqlExportWriter sqlWriter { get; set; }
        internal List<string> IgnoredTables { get; set; } = new List<string>();
        internal List<string> WarningsList { get; set; } = new List<string>();
        internal bool majorErrors { get; set; } = false;
        internal BHDatabaseParser()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
            sqlWriter = new BHSqlExportWriter("ParsingResult", "ParsingReport", "txt");
            sqlWriter.DeleteSqlFolder();
            WarningsList.Add("");
            WarningsList.Add($"--------- -------- ---------- --------- --------");
            WarningsList.Add("-- Warnings. Need to be checked before starting the application. -- ");
            WarningsList.Add($"");
            IgnoredTables.Add($"");
            IgnoredTables.Add($"--------- -------- ---------- --------- --------");
            IgnoredTables.Add($"-- Errors. Important errors that can cause problems in the application. --");
            IgnoredTables.Add($"");
        }

        internal int ParseDatabase()
        {
            int result = 0;
            CliLog("\t Please wait while reading the Database. This may take up to 2 minutes in large databases..");

            if (!_multiDatabaseSelector.SetDbDateFormat(connection))
            {
                CliLog("Error 404. Database was not found. Please check the connection string in your project");
                return 404;
            }

            List<TableAspectsInfo> tableInfo = GetDatabaseInformation();
            BHParsingColumnScanner columnScanner = new BHParsingColumnScanner();

            DbParsingStates dbState = CheckCompatibility(tableInfo, columnScanner);

            string folderPath = Path.Combine(DatabaseStatics.DataPath, "ParsingReport");
            string ParsingReportResult = "";
            switch (dbState)
            {
                case DbParsingStates.Proceed:
                    EntityCodeGenerator(tableInfo, columnScanner);
                    CliLog("");
                    CliLog("The database is fully compatible with the BlackHole Orm.");
                    CliLog("Parsing has been successfully completed. Check 'parsing report' for any minor warnings.");
                    CliLog("");
                    CliLog("The 'parsing report' is stored at:");
                    CliLog($"\t {folderPath}");
                    ParsingReportResult = "---- Parsing Result: Successful Parsing of the Database ----";
                    result = 0;
                    break;
                case DbParsingStates.ChangesRequired:
                    EntityCodeGenerator(tableInfo, columnScanner);
                    CliLog("");
                    CliLog("Parsing has been completed with warnings.");
                    CliLog("Some tables had unidentified columns and they were not parsed. Please check the 'parsing report' and make those Entities manually.");
                    CliLog("");
                    CliLog("The 'parsing report' is stored at:");
                    CliLog($"\t {folderPath}");
                    ParsingReportResult = "---- Parsing Result:  Parsing was completed with Errors ----";
                    result = 0;
                    break;
                case DbParsingStates.Incompatible:
                    CliLog("");
                    CliLog("Parsing has failed.");
                    CliLog("Some tables had unidentified columns. The process has exited before parsing. Please check the 'parsing report' for details.");
                    CliLog("");
                    CliLog("The 'parsing report' is stored at:");
                    CliLog($"\t {folderPath}");
                    CliLog("");
                    CliLog("If you want to force the parsing and get as many tables as possible, use the '-f' or '--force' argument with the 'parse' command.");
                    CliLog("The incompatible tables will be ignored.");
                    ParsingReportResult = "---- Parsing Result:  Parsing of the Database Failed ----";
                    result = 510;
                    break;
            }

            SaveParsingReport(ParsingReportResult);

            if (dbState != DbParsingStates.Incompatible)
            {
                CliLog("");
                CliLog("\t !!Important!!");
                CliLog("\t Keep a backup of your database before Running your application.");
                CliLog("\t Use the 'BlockAutomaticUpdate()' setting in the configuration, until you make sure that the database was parsed correctly.");
                CliLog("\t BlackHole Orm and Mikarsoft are not responsible for damage or data loss in your Database.");
                CliLog("");
            }

            return result;
        }

        internal void EntityCodeGenerator(List<TableAspectsInfo> parsingData, BHParsingColumnScanner columnScanner)
        {
            string scriptsPath = Path.Combine(CliCommand.ProjectPath, "BHEntities");
            string applicationName = Path.GetFileName(CliCommand.ProjectPath).Replace(".csproj","");
            CliLog(" Checking Tables has been completed");
            CliLog("------------------------ --------------------- ---------------------");
            CliLog("");
            CliLog($"Application Name: {applicationName}");
            CliLog("");

            if (!Directory.Exists(scriptsPath))
            {
                Directory.CreateDirectory(scriptsPath);
            }

            foreach (TableAspectsInfo tableAspectInf in parsingData.Where(x => x.GeneralError == false && x.UseOpenEntity == false))
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
                        else
                        {
                            string errorMessage = $"Column '{columnInfo.ColumnName}' of Table '{tableAspectInf.TableName}', is Incompatible with BlackHole's supported types and it will be ignored.";
                            CliLog(errorMessage);
                            WriteWarningsList(tableAspectInf.TableName, errorMessage);
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

                try
                {
                    string pathFile = Path.Combine(scriptsPath, $"{tableAspectInf.TableName}.cs");

                    if (File.Exists(pathFile))
                    {
                        File.Delete(pathFile);
                    }

                    using (var tw = new StreamWriter(pathFile, true))
                    {
                        tw.Write(EntityScript);
                    }

                    CliLog($"Successfully Created BlackHoleEntity '{tableAspectInf.TableName}' in BHEntities Folder");
                }
                catch(Exception ex)
                {
                    CliLog($"Failed to create Entity {tableAspectInf.TableName} in BHEntities Folder");
                    CliLog(ex.Message);
                }
                CliLog("");
            }

            foreach (TableAspectsInfo tableAspectInf in parsingData.Where(x => x.GeneralError == false && x.UseOpenEntity))
            {
                CliLog($"Creating Entity {tableAspectInf.TableName} in BHEntities Folder...");
                string ClassName = tableAspectInf.TableName.Replace("-", "_");
                string EntityScript = $" using System;\n using BlackHole.Entities;\n\n namespace {applicationName}.BHEntities \n";
                string PropertiesScript = string.Empty;
                string PKOptionsScript = $"\n\n\t\t public PKSettings<{ClassName}> PrimaryKeyOptions(PKOptionsBuilder<{ClassName}> builder)\n";
                PKOptionsScript += "\t\t {\n";
                PKOptionsScript += "\t\t\t return builder";
                EntityScript += " { \n";
                EntityScript += $"\t public class {ClassName} : BHOpenEntity<{ClassName}> \n";
                EntityScript += "\t {\n";
                string MainPK = string.Empty;
                string OtherPKs = string.Empty;
                bool noPrimaryKey = true;

                foreach (TableParsingInfo columnInfo in tableAspectInf.TableColumns)
                {
                    ColumnScanResult scanResult = columnScanner.ParseColumnToProperty(columnInfo);

                    if (!scanResult.UnidentifiedColumn)
                    {
                        PropertiesScript += GetBHAttribute(columnInfo, scanResult.PropertyNameForColumn);
                        PropertiesScript += $"\t\t public {scanResult.PropertyNameForColumn} {columnInfo.ColumnName}" + " {get; set;}" + $" {scanResult.DefaultValue} \n";
                    }
                    else
                    {
                        string errorMessage = $"Column '{columnInfo.ColumnName}' of Table '{tableAspectInf.TableName}', is Incompatible with BlackHole's supported types and it will be ignored.";
                        CliLog(errorMessage);
                        WriteWarningsList(tableAspectInf.TableName, errorMessage);
                    }

                    if (columnInfo.PrimaryKey && tableAspectInf.Configuration != null)
                    {
                        noPrimaryKey = false;
                        if(columnInfo.ColumnName == tableAspectInf.Configuration.MainPrimaryKey)
                        {
                            if (tableAspectInf.Configuration.HasAutoIncrement)
                            {
                                MainPK = $".SetPrimaryKey(x => x.{columnInfo.ColumnName}, true)";
                            }
                            else
                            {
                                MainPK = $".SetPrimaryKey(x => x.{columnInfo.ColumnName})";
                            }
                        }
                        else
                        {
                            OtherPKs += $".CompositeKey(x => x.{columnInfo.ColumnName})";
                        }
                    }
                }

                if (noPrimaryKey)
                {
                    PKOptionsScript += ".NoPrimaryKey();";
                    WriteWarningsList(tableAspectInf.TableName, $"No Primary keys were found for Table '{tableAspectInf.TableName}'. \n\t  If this is incorrect, please modify the Entity manually before running your application.");
                }
                else
                {
                    PKOptionsScript += $"{MainPK}{OtherPKs};";
                }

                EntityScript += PropertiesScript;
                EntityScript += PKOptionsScript;
                EntityScript += "\n\t\t }\n";
                EntityScript += "\t } \n";
                EntityScript += " } \n";

                try
                {
                    string pathFile = Path.Combine(scriptsPath, $"{ClassName}.cs");

                    if (File.Exists(pathFile))
                    {
                        File.Delete(pathFile);
                    }

                    using (var tw = new StreamWriter(pathFile, true))
                    {
                        tw.Write(EntityScript);
                    }

                    CliLog($"Successfully Created BHOpenEntity '{tableAspectInf.TableName}' in BHEntities Folder");
                }
                catch(Exception ex)
                {
                    CliLog($"Failed to create Entity '{tableAspectInf.TableName}' in BHEntities Folder");
                    CliLog(ex.Message);
                }
                CliLog("");
            }
        }

        internal DbParsingStates CheckCompatibility(List<TableAspectsInfo> parsingData, BHParsingColumnScanner columnScanner)
        {
            DbParsingStates dbState = DbParsingStates.Proceed;
            string errorMessage = string.Empty;
            bool blockParsing = false;

            foreach (TableAspectsInfo tableAspectInf in parsingData)
            {
                CliLog($"Checking Table '{tableAspectInf.TableName}'..");
                CliLog("");
                List<TableParsingInfo> primaryKey = tableAspectInf.TableColumns.Where(x=>x.PrimaryKey && string.IsNullOrEmpty(x.ReferencedTable)).ToList();

                TableParsingInfo? inactiveColumn = tableAspectInf.TableColumns.FirstOrDefault(x => x.ColumnName == "Inactive");

                if (primaryKey.Count == 1 && inactiveColumn != null && primaryKey[0].ColumnName == "Id")
                {
                    CliLog("Trying to parse table as BlackHoleEntity..");
                    CliLog("");
                    ColumnScanResult scanPkResult = columnScanner.ParsePrimaryKeyToProperty(primaryKey[0]);

                    if (scanPkResult.UnidentifiedColumn)
                    {
                        CliLog($"Primary Key Column '{primaryKey[0].ColumnName}' of the Table '{tableAspectInf.TableName}', is Incompatible with BlackHoleEntity 's Supported Primary keys.");
                        CliLog($"Checking if '{tableAspectInf.TableName}' Table is compatible with BHOpenEntity..");
                        CliLog("");
                        ColumnScanResult scanOtherResult = columnScanner.ParseColumnToProperty(primaryKey[0]);

                        if (scanOtherResult.UnidentifiedColumn)
                        {
                            tableAspectInf.GeneralError = true;
                            errorMessage = $"Table '{tableAspectInf.TableName}' has incompatible Primary Key column type at '{primaryKey[0].ColumnName}' column.";
                            WriteIgnoredTable(tableAspectInf.TableName, errorMessage);
                            majorErrors = true;

                            if (CliCommand.ForceAction)
                            {
                                CliLog($"{errorMessage} It will not be generated as Entity.");
                                CliLog("");
                                dbState = DbParsingStates.ChangesRequired;
                            }
                            else
                            {
                                CliLog($"{errorMessage} Parsing will be aborted.");
                                CliLog("");
                                blockParsing = true;
                            }
                        }
                        else
                        {
                            tableAspectInf.UseOpenEntity = true;
                        }
                    }
                    else
                    {
                        CliLog($"Table '{tableAspectInf.TableName}' is compatible with BlackHoleEntity.");
                        CliLog("");
                    }
                }
                else
                {
                    tableAspectInf.UseOpenEntity = true;
                }

                if (tableAspectInf.UseOpenEntity)
                {
                    CliLog($"Trying to parse '{tableAspectInf.TableName}' table as BHOpenEntity..");

                    foreach (TableParsingInfo columnInfo in primaryKey)
                    {
                        ColumnScanResult scanOpenPkResult = columnScanner.ParseColumnToProperty(columnInfo);

                        if (scanOpenPkResult.UnidentifiedColumn)
                        {
                            errorMessage = $"Primary Key Column '{columnInfo.ColumnName}' of the Table '{tableAspectInf.TableName}', is Incompatible with BlackHole's Supported types.";
                            WriteIgnoredTable(tableAspectInf.TableName, errorMessage);
                            tableAspectInf.GeneralError = true;
                            majorErrors = true;

                            if (CliCommand.ForceAction)
                            {
                                CliLog($"{errorMessage} It will not be generated as Entity.");
                                dbState = DbParsingStates.ChangesRequired;
                            }
                            else
                            {
                                CliLog($"{errorMessage} Parsing will be aborted.");
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
                        if(tableAspectInf.Configuration != null && tableAspectInf.Configuration.WarningMessage != string.Empty)
                        {
                            CliLog(tableAspectInf.Configuration.WarningMessage);
                            WriteWarningsList(tableAspectInf.TableName,tableAspectInf.Configuration.WarningMessage);
                        }
                    }

                    if (!tableAspectInf.GeneralError)
                    {
                        CliLog($"Table '{tableAspectInf.TableName}' is compatible with BHOpenEntity.");
                        CliLog("");
                    }
                }
            }

            if (blockParsing)
            {
                dbState = DbParsingStates.Incompatible;
            }

            return dbState;
        }

        internal void WriteIgnoredTable(string TableName, string message)
        {
            IgnoredTables.Add("");
            IgnoredTables.Add($"\t - Incompatible Table: {TableName}");
            IgnoredTables.Add($"\t   Ignore Reason: {message}");
        }

        internal void WriteWarningsList(string TableName, string message)
        {
            WarningsList.Add("");
            WarningsList.Add($"\t - Warning at Table: {TableName}");
            WarningsList.Add($"\t   Warning Information: {message}");
        }

        internal void SaveParsingReport(string finalStatus)
        {
            sqlWriter.AddSqlCommand(finalStatus);
            sqlWriter.AddMultiple(WarningsList);

            if (!majorErrors)
            {
                IgnoredTables.Add("\t\t\t - No Errors -");
            }

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
            string DefaultVal = DefaultValueCheck(columnInfo);

            if (dotNetType == "string")
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
                string referencedCol = $@"""{columnInfo.ReferencedColumn}""";
                if (columnInfo.Nullable)
                {
                    return $"{VarcharSize}\n\t\t [ForeignKey(typeof({columnInfo.ReferencedTable}),{referencedCol})]\n";
                }
                else
                {
                    return $"{VarcharSize}\n\t\t [ForeignKey(typeof({columnInfo.ReferencedTable}),{referencedCol},false)]\n";
                }
            }

            if (!columnInfo.Nullable)
            {
                if(DefaultVal != string.Empty)
                {
                    return $"{VarcharSize}\n\t\t [NotNullable{DefaultVal}]\n";
                }

                return $"{VarcharSize}\n\t\t [NotNullable]\n";
            }

            if (DefaultVal != string.Empty)
            {
                return $"{VarcharSize}\n\t\t [DefaultValue{DefaultVal}]\n";
            }

            if (VarcharSize != string.Empty)
            {
                return $"{VarcharSize}\n";
            }

            return string.Empty;
        }

        internal string DefaultValueCheck(TableParsingInfo columnInfo)
        {
            if (!columnInfo.PrimaryKey && !string.IsNullOrEmpty(columnInfo.DefaultValue))
            {
                string[] testValue = columnInfo.DefaultValue.Replace("(", "").Replace(")", "").Split("'");

                if(testValue.Length > 2)
                {
                    string mainValue = testValue[1];

                    if(DateTime.TryParseExact(mainValue, DatabaseStatics.DbDateFormat, CultureInfo.InvariantCulture,DateTimeStyles.None, out DateTime parseDt))
                    {
                        return $"({parseDt.Year},{parseDt.Month},{parseDt.Day})";
                    }

                    return $@"(""{mainValue}"")";
                }

                if(testValue.Length > 0)
                {
                    string mainValue = testValue[0];

                    if(Double.TryParse(mainValue, out double result))
                    {
                        return $"({result})";
                    }
                }
            }
            return string.Empty;
        }

        internal List<TableAspectsInfo> GetDatabaseInformation()
        {
            string parseCommand = string.Empty;
            string schemaCheck = GetSchemaCheckCommand();
            List<TableAspectsInfo> tableAspects = new();
            List<TableParsingInfo> parsingData = new();
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
                    if (info.pk == 1 && isAutoIncrement != null && (info.type.ToLower().Contains("integer") || info.type.ToLower().Contains("bigint")))
                    {
                        parsingLine.Extra = "auto_increment";
                    }

                    if(info.type.ToLower().Contains("bigint") && info.dflt_value.ToLower().Contains("last_insert_rowid()"))
                    {
                        parsingLine.Extra = "auto_increment";
                        parsingLine.PrimaryKey = true;
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
