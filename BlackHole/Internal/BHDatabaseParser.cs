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
        internal BHDatabaseInfoReader dbInfoReader { get; set; }
        internal BHDatabaseParser()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
            sqlWriter = new BHSqlExportWriter("ParsingResult", "ParsingReport", "txt");
            dbInfoReader = new BHDatabaseInfoReader(connection, _multiDatabaseSelector);
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

            List<TableAspectsInfo> tableInfo = dbInfoReader.GetDatabaseInformation(0);
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

                foreach(TableParsingInfo columnInfo in tableAspectInf.TableColumns.Where(x=>x.ColumnName.ToLower() != "inactive").DistinctBy(x=> x.ColumnName))
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
                string PKOptionsScript = $"\n\n\t\t public override EntitySettings<{ClassName}> EntityOptions(EntityOptionsBuilder<{ClassName}> builder)\n";
                PKOptionsScript += "\t\t {\n";
                PKOptionsScript += "\t\t\t return builder";
                EntityScript += " { \n";
                EntityScript += $"\t public class {ClassName} : BHOpenEntity<{ClassName}> \n";
                EntityScript += "\t {\n";
                string MainPK = string.Empty;
                string OtherPKs = string.Empty;
                bool noPrimaryKey = true;

                foreach (TableParsingInfo columnInfo in tableAspectInf.TableColumns.DistinctBy(x => x.ColumnName))
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

                if(characterSize != 255 || !string.IsNullOrEmpty(columnInfo.ReferencedTable))
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
                return $"{VarcharSize}\n\t\t [NotNullable]\n";
            }

            if (VarcharSize != string.Empty)
            {
                return $"{VarcharSize}\n";
            }

            return string.Empty;
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
