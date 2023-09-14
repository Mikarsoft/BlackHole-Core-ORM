using System.Data;
using System.Reflection;
using System.Text;
using BlackHole.CoreSupport;
using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHTableBuilder
    {
        private readonly IBHDatabaseSelector _multiDatabaseSelector;
        private readonly IExecutionProvider connection;
        private BHSqlExportWriter SqlWriter { get; set; }
        private List<TableParsingInfo> DbConstraints { get; set; } = new();

        private readonly string[] SqlDatatypes;
        private readonly bool IsMyShit;
        private readonly bool IsLite;
        private readonly bool IsOpenPKConstraint;
        private string TableSchemaCheck { get; set; }
        private string TableSchema { get; set; }
        private string TableSchemaFk { get; set; }
        public BlackHoleTransaction transaction = new(); 
        internal BHDatabaseInfoReader dbInfoReader { get; set; }

        internal BHTableBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
            dbInfoReader = new BHDatabaseInfoReader(connection, _multiDatabaseSelector);
            DbConstraints = dbInfoReader.GetDatabaseParsingInfo();
            SqlDatatypes = _multiDatabaseSelector.SqlDatatypesTranslation();
            TableSchemaCheck = _multiDatabaseSelector.TableSchemaCheck();
            TableSchema = _multiDatabaseSelector.GetDatabaseSchema();
            TableSchemaFk = _multiDatabaseSelector.GetDatabaseSchemaFk();
            IsMyShit = _multiDatabaseSelector.GetMyShit();
            IsLite = _multiDatabaseSelector.IsLite();
            IsOpenPKConstraint = _multiDatabaseSelector.GetOpenPKConstraint();
            SqlWriter = new BHSqlExportWriter("2_TablesSql","SqlFiles","sql");
        }

        internal void BuildMultipleTables(List<Type> TableTypes, List<Type> OpenTableTypes)
        {
            if(DatabaseStatics.AutoUpdate)
            {
                DatabaseStatics.InitializeData = true;
                bool[] Builded = new bool[TableTypes.Count];
                bool[] OpenBuilded = new bool[OpenTableTypes.Count];

                for (int i = 0; i < Builded.Length; i++)
                {
                    Builded[i] = CreateTable(TableTypes[i]);
                }

                for (int i = 0; i < OpenBuilded.Length; i++)
                {
                    OpenBuilded[i] = CreateOpenTable(OpenTableTypes[i]);
                }

                CliConsoleLogs("");

                for (int j = 0; j < Builded.Length; j++)
                {
                    if (Builded[j])
                    {
                        AsignForeignKeys(TableTypes[j]);
                    }
                    else
                    {
                        UpdateSchema(TableTypes[j]);
                    }
                }

                for (int j = 0; j < OpenBuilded.Length; j++)
                {
                    if (OpenBuilded[j])
                    {
                        AsignOpenForeignKeys(OpenTableTypes[j]);
                    }
                    else
                    {
                        UpdateOpenSchema(OpenTableTypes[j]);
                    }
                }

                if (!transaction.Commit())
                {
                    transaction.Dispose();
                    CliConsoleLogs("Errors were detected on the Entities. BlackHole will not update the database.Check the BlackHole Logs for more information. Transaction failed.");
                    Thread.Sleep(2000);
                    throw new Exception("Something went wrong with the Update of the Database. Please check the BlackHole logs to detect and fix the problem.");
                }

                if (CliCommand.ExportSql)
                {
                    SqlWriter.CreateSqlFile();
                }
            }
            transaction.Dispose();
            DatabaseStatics.AutoUpdate = false;
        }

        internal void CleanupConstraints()
        {
            DbConstraints.Clear();
        }

        bool CreateOpenTable(Type TableType)
        {
            if (!CheckTable(TableType.Name))
            {
                PKInfo pkInformation = ReadOpenEntityPrimaryKeys(TableType);
                List<string> pkSettings = pkInformation.PKPropertyNames;
                string Pkoption = OpenPrimaryKey(pkSettings, TableType.Name);

                PropertyInfo[] Properties = TableType.GetProperties();
                StringBuilder tableCreator = new();

                tableCreator.Append($"CREATE TABLE {TableSchema}{MyShit(TableType.Name)} (");

                foreach (PropertyInfo Property in Properties)
                {
                    object[] attributes = Property.GetCustomAttributes(true);

                    if (pkInformation.HasAutoIncrement)
                    {
                        if (Property.Name != pkInformation.MainPrimaryKey)
                        {
                            tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                            tableCreator.Append(GetSqlColumn(attributes, true, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name,TableType.Name));
                        }
                        else
                        {
                            tableCreator.Append(_multiDatabaseSelector.GetCompositePrimaryKeyCommand(Property.PropertyType, pkInformation.MainPrimaryKey));
                        }
                    }
                    else
                    {
                        tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                        tableCreator.Append(GetSqlColumn(attributes, true, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name, TableType.Name));
                    }
                }

                string creationCommand = tableCreator.ToString();
                creationCommand = $"{creationCommand[..^2]}{Pkoption})";
                CliConsoleLogs($"{creationCommand};");
                return connection.JustExecute(creationCommand, null, transaction);
            }

            DatabaseStatics.InitializeData = false;
            return false;
        }

        bool CreateTable(Type TableType)
        {
            if (!CheckTable(TableType.Name))
            {
                PropertyInfo[] Properties = TableType.GetProperties();
                StringBuilder tableCreator = new();
                tableCreator.Append($"CREATE TABLE {TableSchema}{MyShit(TableType.Name)} (");

                tableCreator.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), "Inactive"));
                tableCreator.Append(GetSqlColumn(new object[] {new DefaultValue(0)}, true, typeof(int), false, "Inactive", TableType.Name));

                foreach (PropertyInfo Property in Properties)
                {
                    object[] attributes = Property.GetCustomAttributes(true);

                    if (Property.Name != "Id")
                    {
                        tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));

                        tableCreator.Append(GetSqlColumn(attributes, true, Property.PropertyType, false,Property.Name,TableType.Name));
                    }
                    else
                    {
                        if (TableType.BaseType == typeof(BlackHoleEntity<int>))
                        {
                            tableCreator.Append(_multiDatabaseSelector.GetPrimaryKeyCommand());
                        }

                        if (TableType.BaseType == typeof(BlackHoleEntity<Guid>))
                        {
                            tableCreator.Append(_multiDatabaseSelector.GetGuidPrimaryKeyCommand());
                        }

                        if (TableType.BaseType == typeof(BlackHoleEntity<string>))
                        {
                            tableCreator.Append(_multiDatabaseSelector.GetStringPrimaryKeyCommand());
                        }
                    }
                }

                string creationCommand = tableCreator.ToString();
                creationCommand = $"{creationCommand[..^2]})";
                CliConsoleLogs($"{creationCommand};");
                return connection.JustExecute(creationCommand , null, transaction);
            }

            DatabaseStatics.InitializeData = false;
            return false;
        }

        bool CheckTable(string Tablename)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlLite => connection.ExecuteScalar<string>($@"SELECT name FROM sqlite_master WHERE type='table' AND name='" + Tablename + "'", null, transaction) == Tablename,
                BlackHoleSqlTypes.Oracle => connection.ExecuteScalar<string>($"SELECT table_name FROM all_tables WHERE owner ='{_multiDatabaseSelector.GetDatabaseName()}' and TABLE_NAME = '{Tablename}'", null, transaction) == Tablename,
                _ => connection.ExecuteScalar<int>($"select case when exists((select * from information_schema.tables where table_name = '" + Tablename + $"' {TableSchemaCheck})) then 1 else 0 end", null, transaction) == 1,
            };
        }

        PKInfo ReadOpenEntityPrimaryKeys(Type openEntity)
        {
            List<string> pkNames = new();
            string mainPkCol = string.Empty;
            bool hasAutoIncrement = false;

            var pkOptionsBuilderType = typeof(PKOptionsBuilder<>).MakeGenericType(openEntity);
            object? pkOptionsBuilderObj = Activator.CreateInstance(pkOptionsBuilderType, new object[] { });

            ConstructorInfo? openEntityConstructor = openEntity.GetConstructor(Type.EmptyTypes);
            object? openEntityObj = openEntityConstructor?.Invoke(new object[] { });

            MethodInfo? pkOptionsMethod = openEntity?.GetMethod("PrimaryKeyOptions");
            object? pkSettingsObj = pkOptionsMethod?.Invoke(openEntityObj, new object?[] { pkOptionsBuilderObj });

            if(pkSettingsObj != null)
            {
                if(pkSettingsObj.GetType().GetProperty("PKPropertyNames")?.GetValue(pkSettingsObj, null) is List<string> pkSettings)
                {
                    pkNames = pkSettings;
                }

                if (pkSettingsObj.GetType().GetProperty("MainPrimaryKey")?.GetValue(pkSettingsObj, null) is string mainPK)
                {
                    mainPkCol = mainPK;
                }

                if (pkSettingsObj.GetType().GetProperty("HasAutoIncrement")?.GetValue(pkSettingsObj, null) is bool autoInc)
                {
                    hasAutoIncrement = autoInc;
                }
            }

            return new PKInfo
            {
                PKPropertyNames = pkNames,
                HasAutoIncrement = hasAutoIncrement,
                MainPrimaryKey= mainPkCol
            };
        }

        string OpenPrimaryKey(List<string> pkSettings, string TableName)
        {
            if (pkSettings.Any())
            {
                string pkConstraint = string.Empty;
                if (IsOpenPKConstraint)
                {
                    pkConstraint = $" CONSTRAINT PK_{TableName}";
                }
                StringBuilder primaryKeyCreator = new();
                primaryKeyCreator.Append($",{pkConstraint} Primary Key(");
                foreach (string key in pkSettings)
                {
                    primaryKeyCreator.Append($"{MyShit(key)},");
                }
                return $"{primaryKeyCreator.ToString()[..^1]})";
            }
            return string.Empty;
        }

        void AsignForeignKeys(Type TableType)
        {
            if (IsLite)
            {
                ForeignKeyLiteAsignment(TableType, true);
            }
            else
            {
                ForeignKeyAsignment(TableType);
            }
        }

        void AsignOpenForeignKeys(Type TableType)
        {
            if (IsLite)
            {
                ForeignKeyLiteOpenAsignment(TableType, true);
            }
            else
            {
                ForeignKeyAsignment(TableType);
            }
        }

        void UpdateSchema(Type TableType)
        {
            if (IsLite)
            {
                UpdateLiteTableSchema(TableType);
            }
            else
            {
                UpdateTableSchema(TableType);
            }
        }

        void UpdateOpenSchema(Type TableType)
        {
            if (IsLite)
            {
                UpdateLiteOpenTableSchema(TableType);
            }
            else
            {
                UpdateOpenTableSchema(TableType);
            }
        }

        void UpdateLiteTableSchema(Type TableType)
        {
            List<string> ColumnNames = new();
            List<string> NewColumnNames = new()
            {
                "Inactive"
            };

            foreach (PropertyInfo Property in TableType.GetProperties())
            {
                NewColumnNames.Add(Property.Name);
            }

            foreach (SqLiteTableInfo column in connection.Query<SqLiteTableInfo>($"PRAGMA table_info({MyShit(TableType.Name)}); ", null, transaction))
            {
                ColumnNames.Add(column.name);
            }

            IEnumerable<string> CommonList = ColumnNames.Intersect(NewColumnNames);

            if (CommonList.Count() != ColumnNames.Count)
            {
                ForeignKeyLiteAsignment(TableType, false);
            }
        }

        void UpdateLiteOpenTableSchema(Type TableType)
        {
            List<string> ColumnNames = new();
            List<string> NewColumnNames = new();

            foreach (PropertyInfo Property in TableType.GetProperties())
            {
                NewColumnNames.Add(Property.Name);
            }

            foreach (SqLiteTableInfo column in connection.Query<SqLiteTableInfo>($"PRAGMA table_info({MyShit(TableType.Name)}); ", null))
            {
                ColumnNames.Add(column.name);
            }

            IEnumerable<string> CommonList = ColumnNames.Intersect(NewColumnNames);

            if (CommonList.Count() != ColumnNames.Count)
            {
                ForeignKeyLiteOpenAsignment(TableType, false);
            }
        }

        void ForeignKeyLiteAsignment(Type TableType, bool firstTime)
        {
            string Tablename = MyShit(TableType.Name);
            string OldTablename = MyShit($"{TableType.Name}_Old");

            List<string> ColumnNames = new();
            List<string> NewColumnNames = new()
            {
                "Inactive"
            };

            List<SqLiteForeignKeySchema> SchemaInfo = connection.Query<SqLiteForeignKeySchema>($"PRAGMA foreign_key_list({Tablename});", null, transaction);
            List<SqLiteTableInfo> ColumnsInfo = connection.Query<SqLiteTableInfo>($"PRAGMA table_info({Tablename});", null, transaction);

            foreach (SqLiteTableInfo column in ColumnsInfo)
            {
                ColumnNames.Add(column.name);
            }

            StringBuilder alterTable = new();
            StringBuilder foreignKeys = new();
            StringBuilder closingCommand = new();

            alterTable.Append($"PRAGMA foreign_keys = off; ALTER TABLE {Tablename} RENAME TO {OldTablename}; CREATE TABLE {Tablename} (");
            alterTable.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), "Inactive"));
            alterTable.Append(GetSqlColumn(new object[] { new DefaultValue(0) }, true, typeof(int), false, "Inactive",TableType.Name));

            foreach (PropertyInfo Property in TableType.GetProperties())
            {
                object[] attributes = Property.GetCustomAttributes(true);
                NewColumnNames.Add(Property.Name);

                if (Property.Name != "Id")
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));

                    alterTable.Append(GetSqlColumn(attributes, firstTime, Property.PropertyType, false, Property.Name,TableType.Name));
                }
                else
                {
                    if (TableType.BaseType == typeof(BlackHoleEntity<int>))
                    {
                        alterTable.Append(_multiDatabaseSelector.GetPrimaryKeyCommand());
                    }

                    if (TableType.BaseType == typeof(BlackHoleEntity<Guid>))
                    {
                        alterTable.Append(_multiDatabaseSelector.GetGuidPrimaryKeyCommand());
                    }

                    if (TableType.BaseType == typeof(BlackHoleEntity<string>))
                    {
                        alterTable.Append(_multiDatabaseSelector.GetStringPrimaryKeyCommand());
                    }
                }

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.SingleOrDefault(x => x.GetType() == typeof(ForeignKey));

                    if (FK_attribute != null)
                    {
                        var tName = FK_attribute.GetType().GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FK_attribute.GetType().GetProperty("Column")?.GetValue(FK_attribute, null);
                        var cascadeInfo = FK_attribute.GetType().GetProperty("CascadeInfo")?.GetValue(FK_attribute, null);
                        foreignKeys.Append(LiteConstraint(TableType.Name, Property.Name, tName, tColumn, cascadeInfo));
                    }
                }
            }

            if (!DatabaseStatics.IsDevMove && !CliCommand.ForceAction)
            {
                List<string> ColumnsToAdd = ColumnNames.Except(NewColumnNames).ToList();

                foreach(string ColumnName in ColumnsToAdd)
                {
                    SqLiteTableInfo? columnInfo = ColumnsInfo.Where(x => x.name == ColumnName).FirstOrDefault();
                    SqLiteForeignKeySchema? fkC = SchemaInfo.Where(x => x.from == ColumnName).FirstOrDefault();

                    if(columnInfo != null)
                    {
                        NewColumnNames.Add(ColumnName);

                        alterTable.Append($"{columnInfo.name} {columnInfo.type} NULL, ");

                        if(fkC != null)
                        {
                            foreignKeys.Append($"CONSTRAINT fk_{Tablename}_{fkC.table} FOREIGN KEY ({fkC.from}) REFERENCES {fkC.table}({fkC.to}) on delete {fkC.on_delete}, ");
                        }
                    }
                }
            }

            string FkCommand = $"{alterTable}{foreignKeys}";

            if (FkCommand.Length > 1)
            {
                FkCommand = $"{FkCommand[..(FkCommand.Length - 2)]}); ";
            }

            alterTable.Clear(); foreignKeys.Clear();

            closingCommand.Append(FkCommand);
            closingCommand.Append($"{TransferOldTableData(ColumnNames, NewColumnNames, Tablename, OldTablename)} DROP TABLE {OldTablename};");
            closingCommand.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; ALTER TABLE {OldTablename} RENAME TO {Tablename};");
            closingCommand.Append($"PRAGMA foreign_keys = on; DROP INDEX IF EXISTS {OldTablename}");
            connection.JustExecute(closingCommand.ToString(), null, transaction);
            CliConsoleLogs($"{closingCommand}");
            closingCommand.Clear();
        }


        void ForeignKeyLiteOpenAsignment(Type TableType, bool firstTime)
        {
            string Tablename = MyShit(TableType.Name);
            string OldTablename = MyShit($"{TableType.Name}_Old");
            PKInfo pkInformation = ReadOpenEntityPrimaryKeys(TableType);
            List<string> pkSettings = pkInformation.PKPropertyNames;
            string Pkoption = OpenPrimaryKey(pkSettings, TableType.Name);

            List<string> ColumnNames = new();
            List<string> NewColumnNames = new();

            List<SqLiteForeignKeySchema> SchemaInfo = connection.Query<SqLiteForeignKeySchema>($"PRAGMA foreign_key_list({Tablename});", null, transaction);
            List<SqLiteTableInfo> ColumnsInfo = connection.Query<SqLiteTableInfo>($"PRAGMA table_info({Tablename});", null, transaction);

            foreach (SqLiteTableInfo column in ColumnsInfo)
            {
                ColumnNames.Add(column.name);
            }

            StringBuilder alterTable = new();
            StringBuilder foreignKeys = new();
            StringBuilder closingCommand = new();

            alterTable.Append($"PRAGMA foreign_keys=off; ALTER TABLE {Tablename} RENAME TO {OldTablename}; CREATE TABLE {Tablename} (");

            foreach (PropertyInfo Property in TableType.GetProperties())
            {
                object[] attributes = Property.GetCustomAttributes(true);
                NewColumnNames.Add(Property.Name);

                if (pkInformation.HasAutoIncrement)
                {
                    if(Property.Name != pkInformation.MainPrimaryKey)
                    {
                        alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                        alterTable.Append(GetSqlColumn(attributes, firstTime, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name,TableType.Name));
                    }
                    else
                    {
                        alterTable.Append(_multiDatabaseSelector.GetCompositePrimaryKeyCommand(Property.PropertyType, pkInformation.MainPrimaryKey));
                    }
                }
                else
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                    alterTable.Append(GetSqlColumn(attributes, firstTime, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name,TableType.Name));
                }

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.SingleOrDefault(x => x.GetType() == typeof(ForeignKey));

                    if (FK_attribute != null)
                    {
                        var tName = FK_attribute.GetType().GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FK_attribute.GetType().GetProperty("Column")?.GetValue(FK_attribute, null);
                        var cascadeInfo = FK_attribute.GetType().GetProperty("CascadeInfo")?.GetValue(FK_attribute, null);
                        foreignKeys.Append(LiteConstraint(TableType.Name, Property.Name, tName, tColumn, cascadeInfo));
                    }
                }
            }

            if (!DatabaseStatics.IsDevMove && !CliCommand.ForceAction)
            {
                List<string> ColumnsToAdd = ColumnNames.Except(NewColumnNames).ToList();

                foreach (string ColumnName in ColumnsToAdd)
                {
                    SqLiteTableInfo? columnInfo = ColumnsInfo.Where(x => x.name == ColumnName).FirstOrDefault();
                    SqLiteForeignKeySchema? fkC = SchemaInfo.Where(x => x.from == ColumnName).FirstOrDefault();

                    if (columnInfo != null)
                    {
                        NewColumnNames.Add(ColumnName);

                        alterTable.Append($"{columnInfo.name} {columnInfo.type} NULL, ");

                        if (fkC != null)
                        {
                            foreignKeys.Append($"CONSTRAINT fk_{TableType.Name}_{fkC.table} FOREIGN KEY ({fkC.from}) REFERENCES {fkC.table}({fkC.to}) on delete {fkC.on_delete}, ");
                        }
                    }
                }
            }

            if(Pkoption != string.Empty)
            {
                Pkoption = $"{Pkoption.Remove(0, 1)}, ";
            }

            string FkCommand = $"{alterTable}{Pkoption}{foreignKeys}";

            if (FkCommand.Length > 1)
            {
                FkCommand = $"{FkCommand[..(FkCommand.Length - 2)]}); ";
            }

            alterTable.Clear(); foreignKeys.Clear();

            closingCommand.Append(FkCommand);
            closingCommand.Append($"{TransferOldTableData(ColumnNames, NewColumnNames, Tablename, OldTablename)} DROP TABLE {OldTablename};");
            closingCommand.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; ALTER TABLE {OldTablename} RENAME TO {Tablename};");
            closingCommand.Append($"PRAGMA foreign_keys = on; DROP INDEX IF EXISTS {OldTablename}");
            string test = closingCommand.ToString();
            connection.JustExecute(closingCommand.ToString(), null, transaction);
            CliConsoleLogs($"{closingCommand}");
            closingCommand.Clear();
        }

        void ForeignKeyAsignment(Type TableType)
        {
            string Tablename = TableType.Name;
            PropertyInfo[] Properties = TableType.GetProperties();
            string alterTable = $" ALTER TABLE {TableSchema}{MyShit(Tablename)}";

            foreach (PropertyInfo Property in Properties)
            {
                object[] attributes = Property.GetCustomAttributes(true);

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.SingleOrDefault(x => x.GetType() == typeof(ForeignKey));

                    if (FK_attribute != null)
                    {
                        var tName = FK_attribute.GetType().GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FK_attribute.GetType().GetProperty("Column")?.GetValue(FK_attribute, null);
                        var cascadeInfo = FK_attribute.GetType().GetProperty("CascadeInfo")?.GetValue(FK_attribute, null);
                        string alterColumn = MyShitConstraint(alterTable, Tablename, Property.Name, tName, tColumn, cascadeInfo);
                        connection.JustExecute(alterColumn, null, transaction);
                        CliConsoleLogs($"{alterColumn};");
                    }
                }
            }
        }

        void UpdateTableSchema(Type TableType)
        {
            string getColumns = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableType.Name}' {TableSchemaCheck}";

            if(DatabaseStatics.DatabaseType == BlackHoleSqlTypes.Oracle)
            {
                string owner = _multiDatabaseSelector.GetDatabaseName();
                getColumns = $"SELECT Column_name From all_tab_cols where owner = '{owner}' and TABLE_NAME = '{TableType.Name}'";
            }

            PropertyInfo[] Properties = TableType.GetProperties();
            string Tablename = MyShit(TableType.Name);

            List<string> NewColumnNames = new()
            {
                "Inactive"
            };

            foreach (PropertyInfo Property in Properties)
            {
                NewColumnNames.Add(Property.Name);
            }

            List<string> ColumnNames = connection.Query<string>(getColumns, null, transaction);
            List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
            List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();
            DropColumns(ColumnsToDrop, TableType.Name);

            StringBuilder columnCreator = new();

            foreach (string ColumnName in ColumnsToAdd)
            {
                columnCreator.Append($"ALTER TABLE {TableSchema}{Tablename} ADD ");
                PropertyInfo? Property = Properties.Where(x => x.Name == ColumnName).FirstOrDefault();

                if (Property != null)
                {
                    object[]? attributes = Property.GetCustomAttributes(true);
                    columnCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, ColumnName));
                    columnCreator.Append(AddColumnConstaints(attributes, TableType.Name, Property.Name, Property.PropertyType));

                    foreach(string commandText in columnCreator.ToString().Split("##"))
                    {
                        if (!string.IsNullOrEmpty(commandText))
                        {
                            connection.JustExecute(commandText, null, transaction);
                            CliConsoleLogs($"{commandText};");
                        }
                    }
                }

                if (ColumnName == "Inactive")
                {
                    columnCreator.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), ColumnName));
                    connection.JustExecute(columnCreator.ToString(), null, transaction);
                    CliConsoleLogs($"{columnCreator};");
                }

                columnCreator.Clear();
            }
        }

        void UpdateOpenTableSchema(Type TableType)
        {
            PKInfo pkInformation = ReadOpenEntityPrimaryKeys(TableType);
            List<TableParsingInfo> ExistingPkInfo = DbConstraints.Where(x => x.TableName == TableType.Name && x.PrimaryKey).ToList();
            bool canUpdatePKs = CompairPrimaryKeys(ExistingPkInfo,pkInformation.PKPropertyNames, TableType.Name);

            string Tablename = MyShit(TableType.Name);

            string getColumns = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableType.Name}' {TableSchemaCheck}";

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.Oracle)
            {
                string owner = _multiDatabaseSelector.GetDatabaseName();
                getColumns = $"SELECT Column_name From all_tab_cols where owner = '{owner}' and TABLE_NAME = '{TableType.Name}'";
            }

            PropertyInfo[] Properties = TableType.GetProperties();

            List<string> NewColumnNames = new();

            foreach (PropertyInfo Property in Properties)
            {
                NewColumnNames.Add(Property.Name);
            }

            List<string> ColumnNames = connection.Query<string>(getColumns, null,transaction);

            bool PKsDropped = false;

            if (canUpdatePKs)
            {
                PKsDropped = connection.JustExecute($"ALTER TABLE {TableSchema}{Tablename} DROP CONSTRAINT PK_{TableType.Name} ", null, transaction);
            }

            List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
            List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();
            DropColumns(ColumnsToDrop, TableType.Name);

            StringBuilder columnCreator = new();

            foreach (string ColumnName in ColumnsToAdd)
            {
                columnCreator.Append($"ALTER TABLE {TableSchema}{Tablename} ADD ");
                PropertyInfo? Property = Properties.Where(x => x.Name == ColumnName).FirstOrDefault();

                if (Property != null)
                {
                    object[]? attributes = Property.GetCustomAttributes(true);
                    columnCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, ColumnName));
                    columnCreator.Append(AddColumnConstaints(attributes, TableType.Name, Property.Name, Property.PropertyType));

                    foreach (string commandText in columnCreator.ToString().Split("##"))
                    {
                        if (!string.IsNullOrEmpty(commandText))
                        {
                            connection.JustExecute(commandText, null, transaction);
                            CliConsoleLogs($"{commandText};");
                        }
                    }
                }

                columnCreator.Clear();
            }

            if (pkInformation.PKPropertyNames.Any() && PKsDropped)
            {
                string primaryKeys = string.Empty;
                foreach(string pkName in pkInformation.PKPropertyNames)
                {
                    primaryKeys += $",{MyShit(pkName)}";
                }
                connection.JustExecute($"ALTER TABLE {TableSchema}{Tablename} ADD CONSTRAINT PK_{TableType.Name} PRIMARY KEY ({primaryKeys.Remove(0, 1)})", null, transaction);
            }
        }

        private bool CompairPrimaryKeys(List<TableParsingInfo> existingPKs, List<string> newPKs, string TableName)
        {
            List<string> primaryKeys = new List<string>();
            foreach (TableParsingInfo pkInDb in existingPKs)
            {
                primaryKeys.Add(pkInDb.ColumnName);
            }

            List<string> PKsToAdd = newPKs.Except(primaryKeys).ToList();
            List<string> PKsToDrop = primaryKeys.Except(newPKs).ToList();

            bool result = PKsToAdd.Any() || PKsToDrop.Any();

            if (DatabaseStatics.IsDevMove || CliCommand.ForceAction)
            {
                return result;
            }

            if (result)
            {
                transaction.DoNotCommit();
                transaction.Dispose();
                string errorMessage = $"Error at Table '{TableName}' on Primary Keys Configuration. You CAN ONLY change the PRIMARY KEYS of a Table in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'";
                CliConsoleLogs(errorMessage);
                throw new Exception(errorMessage);
            }

            return false;
        }

        private bool NullabilityUpdateCheck(PropertyInfo newColumn,TableParsingInfo existingColumn, string TableName)
        {
            bool isNullable = existingColumn.Nullable;
            object? defaultVal = null;
            object[] bhAttributes = newColumn.GetCustomAttributes(true);
            bool isColumnDateTime = false;

            object? fkAttribute = bhAttributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));
            if(fkAttribute != null)
            {
                object? nullability = typeof(ForeignKey).GetProperty("Nullability")?.GetValue(fkAttribute, null);
                if(nullability is bool nullableCol)
                {
                    isNullable = nullableCol;
                }
            }

            object? notNullableAtr = bhAttributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
            if (notNullableAtr != null)
            {
                object? nullabilityAtr = typeof(NotNullable).GetProperty("Nullability")?.GetValue(notNullableAtr, null);
                defaultVal = typeof(NotNullable).GetProperty("ValueDefault")?.GetValue(notNullableAtr, null);
                object? isDatetimeVal = typeof(NotNullable).GetProperty("IsDatetimeValue")?.GetValue(notNullableAtr, null);

                if (nullabilityAtr is bool nullableColAtr)
                {
                    isNullable = nullableColAtr;
                }

                if(isDatetimeVal is bool isdateTimeType)
                {
                    isColumnDateTime = isdateTimeType;
                }
            }

            if (newColumn.PropertyType.Name.Contains("Nullable"))
            {
                if (newColumn.PropertyType.GenericTypeArguments != null && newColumn.PropertyType.GenericTypeArguments.Length > 0)
                {
                    isNullable = true;
                }
            }

            if (isNullable != existingColumn.Nullable)
            {
                if (DatabaseStatics.IsDevMove || CliCommand.ForceAction)
                {
                    if(existingColumn.Nullable)
                    {
                        SetColumnToNotNull(newColumn.PropertyType, defaultVal, isColumnDateTime, newColumn.Name, TableName, existingColumn);
                    }
                    else
                    {
                        SetColumnToNull(TableName, newColumn.Name, existingColumn);
                    }
                }
                else
                {
                    transaction.DoNotCommit();
                    transaction.Dispose();
                    string errorMessage = $"Error at Table '{TableName}' and Column '{newColumn.Name}' on Nullability Change. You CAN ONLY Change Nullability of a Column in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'";
                    CliConsoleLogs(errorMessage);
                    throw new Exception(errorMessage);
                }
            }

            return false;
        }

        private void SetColumnToNotNull(Type PropType,object? defaultVal, bool isDatetime, string PropName, string TableName, TableParsingInfo ColumnInfo)
        {
            string defaultValCommand = DefaultValueCheck(PropType, defaultVal, isDatetime, PropName,TableName, true);

            if (string.IsNullOrEmpty(defaultValCommand))
            {
                transaction.DoNotCommit();
                transaction.Dispose();
                string errorMessage = $"Error at Table '{TableName}' and Column '{PropName}'. A DEFAULT VALUE is REQUIRED to change a 'NULL' Column to 'NOT NULL' on an existing Table. Use the '[NotNullable(value)]' Attribute Overload";
                CliConsoleLogs(errorMessage);
                throw new Exception(errorMessage);
            }
            connection.JustExecute($"Update {TableSchema}{MyShit(TableName)} set {MyShit(PropName)} = {defaultValCommand.Trim()} where {MyShit(PropName)} is null", null, transaction);
            connection.JustExecute($"ALTER TABLE {TableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} default {defaultValCommand.Trim()} NOT NULL", null, transaction);
        }

        private void SetColumnToNull(string TableName, string PropName, TableParsingInfo ColumnInfo)
        { 
            connection.JustExecute($"ALTER TABLE {TableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} NULL", null, transaction);
        }

        private string GetSqlDataType(TableParsingInfo columnInfo)
        {
            string DataType = columnInfo.DataType;

            if(DatabaseStatics.DatabaseType == BlackHoleSqlTypes.Oracle)
            {
                if (DataType.ToLower().Contains("varchar"))
                {
                    return DataType + $"({columnInfo.MaxLength})";
                }

                if (DataType.ToLower() == "number")
                {
                    return DataType + $"({columnInfo.NumPrecision,0})";
                }
            }
            else
            {
                if (columnInfo.MaxLength > 0)
                {
                    DataType += $"({columnInfo.MaxLength})";
                }
            }

            return DataType;
        }

        private void DropColumns(List<string> ColumnsToDrop, string TableName)
        {
            string lowerTbName = TableName.ToLower();
            if (DatabaseStatics.IsDevMove || CliCommand.ForceAction)
            {
                foreach (string ColumnName in ColumnsToDrop)
                {
                    List<TableParsingInfo> referencedAt = DbConstraints.Where(x=> 
                    x.ReferencedTable.ToLower() == lowerTbName 
                    && x.ReferencedColumn.ToLower() == ColumnName.ToLower()).ToList();

                    foreach(TableParsingInfo referenced in referencedAt)
                    {
                        string dropConstraint = $"ALTER TABLE {TableSchema}{MyShit(referenced.TableName)} DROP CONSTRAINT {referenced.ConstraintName}";
                        connection.JustExecute(dropConstraint, null, transaction);
                        CliConsoleLogs($"{dropConstraint};");
                    }

                    string dropCommand = $"ALTER TABLE {TableSchema}{MyShit(TableName)} DROP COLUMN {MyShit(ColumnName)}";
                    connection.JustExecute(dropCommand, null, transaction);
                    CliConsoleLogs($"{dropCommand};");
                }
            }
            else
            {
                if (ColumnsToDrop.Any())
                {
                    transaction.DoNotCommit();
                    transaction.Dispose();
                    string errorMessage = $"Error at Table '{TableName}' on Dropping Columns. You CAN ONLY Drop Columns of a Table in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'";
                    CliConsoleLogs(errorMessage);
                    throw new Exception(errorMessage);
                }
            }
        }

        string AddColumnConstaints(object[] attributes, string Tablename, string PropName, Type PropType)
        {
            string constraintsCommand = "NULL ##";
            string alterTable = $"ALTER TABLE {TableSchema}{MyShit(Tablename)}";

            Type fkAttributeType = typeof(ForeignKey);
            object? fkAttribute = attributes.FirstOrDefault(x => x.GetType() == fkAttributeType);

            if(fkAttribute != null)
            {
                var tName = fkAttributeType.GetProperty("TableName")?.GetValue(fkAttribute, null);
                var tColumn = fkAttributeType.GetProperty("Column")?.GetValue(fkAttribute, null);
                var tNullable = fkAttributeType.GetProperty("IsNullable")?.GetValue(fkAttribute, null);

                var cascadeInfo = "on delete cascade";

                if (tNullable != null && tNullable.ToString() == "NULL")
                {
                    cascadeInfo = "on delete set null";
                }

                return $"{constraintsCommand} {MyShitConstraint(alterTable, Tablename, PropName, tName, tColumn, cascadeInfo)}";
            }

            object? nnAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));

            if(nnAttribute != null)
            {
                Type nnAttributeType = typeof(NotNullable);

                var defaultValNotnull = nnAttributeType.GetProperty("ValueDefault")?.GetValue(nnAttribute, null);
                var isDatetimeVal = nnAttributeType.GetProperty("IsDatetimeValue")?.GetValue(nnAttribute, null);
                bool usingDateTime = false;
                if(isDatetimeVal is bool)
                {
                    usingDateTime = (bool)isDatetimeVal;
                }
                return $"{DefaultValueCheck(PropType, defaultValNotnull, usingDateTime, PropName, Tablename)} NOT NULL ";
            }

            object? dvAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(DefaultValue));

            if(dvAttribute != null)
            {
                Type dvAttributeType = typeof(DefaultValue);

                var defaultVal = dvAttributeType.GetProperty("ValueDefault")?.GetValue(dvAttribute, null);
                var isDatetimeValue = dvAttributeType.GetProperty("IsDatetimeValue")?.GetValue(dvAttribute, null);
                bool useDateTimeVal = false;
                if (isDatetimeValue is bool)
                {
                    useDateTimeVal = (bool)isDatetimeValue;
                }
                return $"{DefaultValueCheck(PropType, defaultVal, useDateTimeVal,PropName,Tablename)} NULL ";
            }

            return constraintsCommand;
        }

        string GetSqlColumn(object[] attributes, bool firstTime, Type PropertyType, bool isOpenPk, string PropName, string TableName)
        {
            bool mandatoryNull = false;
            string nullPhase = "NULL, ";

            if (PropertyType.Name.Contains("Nullable"))
            {
                if (PropertyType.GenericTypeArguments != null && PropertyType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            Type Fk_Type = typeof(ForeignKey);
            object? fkAttribute = attributes.FirstOrDefault(x => x.GetType() == Fk_Type);

            if (fkAttribute != null)
            {
                var tNull = Fk_Type.GetProperty("IsNullable")?.GetValue(fkAttribute, null);
                nullPhase = firstTime ? $"{tNull}, " : "NULL, ";

                if (mandatoryNull)
                {
                    nullPhase = "NULL, ";
                }

                if (isOpenPk)
                {
                    nullPhase = "NOT NULL, ";
                }

                return nullPhase;
            }

            object? nnAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));

            if (nnAttribute != null)
            {
                Type nnAttributeType = typeof(NotNullable);

                var defaultValNotnull = nnAttributeType.GetProperty("ValueDefault")?.GetValue(nnAttribute, null);
                nullPhase = firstTime ? "NOT NULL, " : "NULL, ";

                if (mandatoryNull)
                {
                    nullPhase = "NULL, ";
                }

                if (isOpenPk)
                {
                    nullPhase = "NOT NULL, ";
                }

                var isDatetimeVal = nnAttributeType.GetProperty("IsDatetimeValue")?.GetValue(nnAttribute, null);
                bool useDateTime = false;
                if (isDatetimeVal is bool)
                {
                    useDateTime = (bool)isDatetimeVal;
                }

                return $"{DefaultValueCheck(PropertyType, defaultValNotnull,useDateTime, PropName, TableName)} {nullPhase}";
            }

            object? dvAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(DefaultValue));

            if (dvAttribute != null)
            {
                Type dvAttributeType = typeof(DefaultValue);

                var defaultVal = dvAttributeType.GetProperty("ValueDefault")?.GetValue(dvAttribute, null);
                var isDatetimeValue = dvAttributeType.GetProperty("IsDatetimeValue")?.GetValue(dvAttribute, null);
                bool useDateTimeVal = false;
                if (isDatetimeValue is bool)
                {
                    useDateTimeVal = (bool)isDatetimeValue;
                }

                if (isOpenPk)
                {
                    nullPhase = "NOT NULL, ";
                }

                return $"{DefaultValueCheck(PropertyType, defaultVal,useDateTimeVal, PropName, TableName)} {nullPhase}";
            }

            if (mandatoryNull)
            {
                return "NULL, ";
            }

            return ", ";
        }

        string MyShit(string propName)
        {
            if (!IsMyShit)
            {
                return $@"""{propName}""";
            }

            return propName;
        }

        string MyShitConstraint(string alterTable, string Tablename, string propName, object? tName, object? tColumn, object? cascadeInfo)
        {
            string constraint = $"ADD CONSTRAINT fk_{Tablename}_{tName}{TableSchemaFk}";

            if (!IsMyShit)
            {
                return $@"{alterTable} {constraint} FOREIGN KEY (""{propName}"") REFERENCES {TableSchema}""{tName}""(""{tColumn}"") {cascadeInfo}";
            }

            return $"{alterTable} {constraint} FOREIGN KEY ({propName}) REFERENCES {TableSchema}{tName}({tColumn}) {cascadeInfo}";
        }

        string LiteConstraint(string Tablename, string propName, object? tName, object? tColumn, object? cascadeInfo)
        {
            if (!IsMyShit)
            {
                return $@"CONSTRAINT fk_{Tablename}_{tName} FOREIGN KEY (""{propName}"") REFERENCES ""{tName}""(""{tColumn}"") {cascadeInfo}, ";
            }

            return $"CONSTRAINT fk_{Tablename}_{tName} FOREIGN KEY ({propName}) REFERENCES {tName}({tColumn}) {cascadeInfo}, ";
        }

        string TransferOldTableData(List<string> oldColumns, List<string> newColumns, string newTablename, string oldTablename)
        {
            string result = "";
            IEnumerable<string> CommonList = oldColumns.Intersect(newColumns);

            if (CommonList.Any())
            {
                result = $"INSERT INTO {newTablename} (";
                string selection = ") SELECT ";
                string selectionClosing = "";

                foreach (string column in CommonList)
                {
                    result += $"{MyShit(column)},";
                    selectionClosing += $"{MyShit(column)},";
                }
                result = $"{result[..^1]}{selection}{selectionClosing[..^1]} FROM {oldTablename};";
            }

            return result;
        }

        string DefaultValueCheck(Type PropertyType, object? defaultValue, bool useDateTime,string PropName, string TableName, bool JustValue = false)
        {
            if(defaultValue != null)
            {
                string defaultCase = "default";
                if (JustValue)
                {
                    defaultCase = string.Empty;
                }
                if (IsNumericType(defaultValue))
                {
                    if (IsIntegerType(PropertyType))
                    {
                        string? obj = defaultValue.ToString();

                        if (!string.IsNullOrEmpty(obj))
                        {
                            string[] numberParts = obj.Split(".");
                            return $" {defaultCase} {numberParts[0]} ";
                        }
                    }

                    if (IsNumericType(PropertyType))
                    {
                        return $" {defaultCase} {defaultValue} ";
                    }
                }
                else
                {
                    bool isDateTimeProp = IsDateTimeType(PropertyType);

                    if (useDateTime)
                    {
                        if (isDateTimeProp)
                        {
                            return $" {defaultCase} '{defaultValue}' ";
                        }
                    }
                    else
                    {
                        if (!IsNumericType(PropertyType) && !isDateTimeProp)
                        {
                            return $" {defaultCase} '{defaultValue}' ";
                        }
                    }
                }

                transaction.DoNotCommit();
                transaction.Dispose();
                string errorMessage = $"Error at Table '{TableName}' and Column '{PropName}'. The DEFAULT VALUE Does NOT match with the property Type.";
                CliConsoleLogs(errorMessage);
                throw new Exception(errorMessage);
            }

            return string.Empty;
        }

        private bool IsDateTimeType(Type propertyType)
        {
            Type propType = propertyType;

            if (propertyType.Name.Contains("Nullable"))
            {
                if (propertyType.GenericTypeArguments != null && propertyType.GenericTypeArguments.Length > 0)
                {
                    propType = propertyType.GenericTypeArguments[0];
                }
            }

            if(Type.GetTypeCode(propType) == TypeCode.DateTime)
            {
                return true;
            }

            return false;
        }

        public bool IsNumericType(object obj)
        {
            return Type.GetTypeCode(obj.GetType()) switch
            {
                TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                _ => false,
            };
        }

        public bool IsIntegerType(Type obj)
        {
            Type propType = obj;

            if (obj.Name.Contains("Nullable"))
            {
                if (obj.GenericTypeArguments != null && obj.GenericTypeArguments.Length > 0)
                {
                    propType = obj.GenericTypeArguments[0];
                }
            }

            return Type.GetTypeCode(propType) == TypeCode.Int32;
        }

        public bool IsNumericType(Type obj)
        {
            Type propType = obj;

            if (obj.Name.Contains("Nullable"))
            {
                if (obj.GenericTypeArguments != null && obj.GenericTypeArguments.Length > 0)
                {
                    propType = obj.GenericTypeArguments[0];
                }
            }

            return Type.GetTypeCode(propType) switch
            {
                TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                _ => false,
            };
        }

        void CliConsoleLogs(string logCommand)
        {
            if (CliCommand.CliExecution)
            {
                Console.WriteLine("_bhLog_");
                Console.Write($"_bhLog_{logCommand}");
                Console.WriteLine("_bhLog_");
            }

            if (CliCommand.ExportSql)
            {
                SqlWriter.AddSqlCommand(logCommand);
            }
        }

        void CliConsoleLogsNoSpace(string logCommand)
        {
            if (CliCommand.CliExecution)
            {
                Console.WriteLine($"_bhLog_{logCommand}");
            }
        }

        internal void UpdateWithoutForceWarning()
        {
            if (CliCommand.CliExecution)
            {
                Console.WriteLine("_bhLog_ \t Update finished");
                Console.WriteLine("_bhLog_");
                if (!DatabaseStatics.IsDevMove && !CliCommand.ForceAction)
                {
                    CliConsoleLogsNoSpace("Warning:");
                    CliConsoleLogsNoSpace("BlackHole is not in Dev Mode. Columns of deleted properties will change to nullable instead of dropping.");
                    CliConsoleLogsNoSpace("If you want to drop some columns, run the update command using the '-f' or '--force' argument");
                    CliConsoleLogsNoSpace("");
                    CliConsoleLogsNoSpace("Example : bhl update -f");
                    CliConsoleLogsNoSpace("");
                }
            }
        }

        string GetDatatypeCommand(Type PropertyType, object[] attributes, string Propertyname)
        {
            string propTypeName = PropertyType.Name;
            string dataCommand = "";

            if (propTypeName.Contains("Nullable"))
            {
                if(PropertyType.GenericTypeArguments != null && PropertyType.GenericTypeArguments.Length > 0)
                {
                    propTypeName = PropertyType.GenericTypeArguments[0].Name;
                }
            }

            switch (propTypeName)
            {
                case "String":
                    object? CharLength = attributes.FirstOrDefault(x => x.GetType() == typeof(VarCharSize));
                    object? ForeignKeyAtt = attributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));

                    if (CharLength != null)
                    {
                        var Lngth = CharLength.GetType().GetProperty("Charlength")?.GetValue(CharLength, null);
                        dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[0]}({Lngth?.ToString()}) ";
                    }
                    else
                    {
                        dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[0]}(255) ";
                    }

                    if(ForeignKeyAtt != null)
                    {
                        dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[0]}(50) ";
                    }

                    break;
                case "Char":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[1]} ";
                    break;
                case "Int16":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[2]} ";
                    break;
                case "Int32":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[3]} ";
                    break;
                case "Int64":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[4]} ";
                    break;
                case "Decimal":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[5]} ";
                    break;
                case "Single":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[6]} ";
                    break;
                case "Double":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[7]} ";
                    break;
                case "Guid":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[8]} ";
                    break;
                case "Boolean":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[9]} ";
                    break;
                case "DateTime":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[10]} ";
                    break;
                case "Byte[]":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[11]} ";
                    break;
                default:
                    throw (new Exception($"Unsupported property type {PropertyType.FullName}"));
            }

            return dataCommand;
        }
    }
}
