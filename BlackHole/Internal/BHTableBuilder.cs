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
        internal BHDatabaseInfoReader? dbInfoReader { get; set; }
        internal bool IsForcedUpdate { get; } 

        internal BHTableBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
            IsForcedUpdate = DatabaseStatics.AutoUpdate && (DatabaseStatics.IsDevMove || CliCommand.ForceAction);

            if (IsForcedUpdate)
            {
                dbInfoReader = new BHDatabaseInfoReader(connection, _multiDatabaseSelector);
                DbConstraints = dbInfoReader.GetDatabaseParsingInfo();
            }

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
                    Thread.Sleep(2000);
                    throw ProtectDbAndThrow("Something went wrong with the Update of the Database. Please check the BlackHole logs to detect and fix the problem.");
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
                            tableCreator.Append(GetSqlColumn(attributes, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name,TableType.Name));
                        }
                        else
                        {
                            tableCreator.Append(_multiDatabaseSelector.GetCompositePrimaryKeyCommand(Property.PropertyType, pkInformation.MainPrimaryKey));
                        }
                    }
                    else
                    {
                        tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                        tableCreator.Append(GetSqlColumn(attributes, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name, TableType.Name));
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
                tableCreator.Append(" NULL, ");

                foreach (PropertyInfo Property in Properties)
                {
                    object[] attributes = Property.GetCustomAttributes(true);

                    if (Property.Name != "Id")
                    {
                        tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));

                        tableCreator.Append(GetSqlColumn(attributes, Property.PropertyType, false,Property.Name,TableType.Name));
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

            var pkOptionsBuilderType = typeof(EntityOptionsBuilder<>).MakeGenericType(openEntity);
            object? pkOptionsBuilderObj = Activator.CreateInstance(pkOptionsBuilderType, new object[] { });

            ConstructorInfo? openEntityConstructor = openEntity.GetConstructor(Type.EmptyTypes);
            object? openEntityObj = openEntityConstructor?.Invoke(new object[] { });

            MethodInfo? pkOptionsMethod = openEntity?.GetMethod("EntityOptions");
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

            PropertyInfo[] Properties = TableType.GetProperties();
            foreach (PropertyInfo Property in Properties)
            {
                NewColumnNames.Add(Property.Name);
            }

            List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();
            if(ColumnsToDrop.Any() && !IsForcedUpdate)
            {
                throw ProtectDbAndThrow($"Error at Table '{TableType.Name}' on Dropping Columns. You CAN ONLY Drop Columns of a Table in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'");
            }
            List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
            List<string> CommonColumns = ColumnNames.Intersect(NewColumnNames).ToList();

            StringBuilder alterTable = new();
            StringBuilder foreignKeys = new();
            StringBuilder closingCommand = new();

            alterTable.Append($"PRAGMA foreign_keys = off; ALTER TABLE {Tablename} RENAME TO {OldTablename}; CREATE TABLE {Tablename} (");
            alterTable.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), "Inactive"));
            alterTable.Append(" NULL, ");
            bool missingInactiveColumn = false;

            foreach (string AddColumn in CommonColumns)
            {
                PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
                object[] attributes = Property.GetCustomAttributes(true);

                if (Property.Name != "Id")
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                    SqLiteTableInfo existingCol = ColumnsInfo.First(x=> x.name == AddColumn);
                    alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, false, Property.Name, TableType.Name, existingCol.notnull));
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

            foreach (string AddColumn in ColumnsToAdd)
            {
                if(AddColumn != "Inactive")
                {
                    PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
                    object[] attributes = Property.GetCustomAttributes(true);
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                    alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, false, Property.Name, TableType.Name, false));

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
                else
                {
                    missingInactiveColumn = true;
                }
            }

            string FkCommand = $"{alterTable}{foreignKeys}";

            if (FkCommand.Length > 1)
            {
                FkCommand = $"{FkCommand[..(FkCommand.Length - 2)]}); ";
            }

            alterTable.Clear(); foreignKeys.Clear();

            closingCommand.Append(FkCommand);
            closingCommand.Append($"{TransferOldTableData(CommonColumns, Tablename, OldTablename)} DROP TABLE {OldTablename};");
            closingCommand.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; ALTER TABLE {OldTablename} RENAME TO {Tablename};");
            closingCommand.Append($"PRAGMA foreign_keys = on; DROP INDEX IF EXISTS {OldTablename}");
            connection.JustExecute(closingCommand.ToString(), null, transaction);
            CliConsoleLogs($"{closingCommand}");
            closingCommand.Clear();

            if (missingInactiveColumn)
            {
                string updateInactiveCol = $"Update Table {TableSchema}{Tablename} set {MyShit("Inactive")} = 0 where {MyShit("Inactive")} is null";
                connection.JustExecute(updateInactiveCol, null, transaction);
                CliConsoleLogs($"{updateInactiveCol};");
            }
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

            PropertyInfo[] Properties = TableType.GetProperties();
            foreach (PropertyInfo Property in Properties)
            {
                NewColumnNames.Add(Property.Name);
            }

            List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();
            if (ColumnsToDrop.Any() && !IsForcedUpdate)
            {
                throw ProtectDbAndThrow($"Error at Table '{TableType.Name}' on Dropping Columns. You CAN ONLY Drop Columns of a Table in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'");
            }
            List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
            List<string> CommonColumns = ColumnNames.Intersect(NewColumnNames).ToList();

            StringBuilder alterTable = new();
            StringBuilder foreignKeys = new();
            StringBuilder closingCommand = new();

            alterTable.Append($"PRAGMA foreign_keys=off; ALTER TABLE {Tablename} RENAME TO {OldTablename}; CREATE TABLE {Tablename} (");

            foreach (string AddColumn in CommonColumns)
            {
                PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
                object[] attributes = Property.GetCustomAttributes(true);
                SqLiteTableInfo existingCol = ColumnsInfo.First(x => x.name == AddColumn);

                if (pkInformation.HasAutoIncrement)
                {
                    if (Property.Name != pkInformation.MainPrimaryKey)
                    {
                        alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                        alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name, TableType.Name, existingCol.notnull));
                    }
                    else
                    {
                        alterTable.Append(_multiDatabaseSelector.GetCompositePrimaryKeyCommand(Property.PropertyType, pkInformation.MainPrimaryKey));
                    }
                }
                else
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                    alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name, TableType.Name, existingCol.notnull));
                }

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));

                    if (FK_attribute != null)
                    {
                        var tName = FK_attribute.GetType().GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FK_attribute.GetType().GetProperty("Column")?.GetValue(FK_attribute, null);
                        var cascadeInfo = FK_attribute.GetType().GetProperty("CascadeInfo")?.GetValue(FK_attribute, null);
                        foreignKeys.Append(LiteConstraint(TableType.Name, Property.Name, tName, tColumn, cascadeInfo));
                    }
                }
            }

            foreach (string AddColumn in ColumnsToAdd)
            {
                PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
                object[] attributes = Property.GetCustomAttributes(true);

                if (pkInformation.HasAutoIncrement)
                {
                    if(Property.Name != pkInformation.MainPrimaryKey)
                    {
                        alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                        alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name,TableType.Name, false));
                    }
                    else
                    {
                        alterTable.Append(_multiDatabaseSelector.GetCompositePrimaryKeyCommand(Property.PropertyType, pkInformation.MainPrimaryKey));
                    }
                }
                else
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                    alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name,TableType.Name, false));
                }

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));

                    if (FK_attribute != null)
                    {
                        var tName = FK_attribute.GetType().GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FK_attribute.GetType().GetProperty("Column")?.GetValue(FK_attribute, null);
                        var cascadeInfo = FK_attribute.GetType().GetProperty("CascadeInfo")?.GetValue(FK_attribute, null);
                        foreignKeys.Append(LiteConstraint(TableType.Name, Property.Name, tName, tColumn, cascadeInfo));
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
            closingCommand.Append($"{TransferOldTableData(CommonColumns, Tablename, OldTablename)} DROP TABLE {OldTablename};");
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

            if (IsForcedUpdate)
            {
                List<string> CommonColumns = NewColumnNames.Intersect(ColumnNames).ToList();
                UpdateTableColumnsNullability(CommonColumns, Properties, TableType.Name);
            }

            List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
            List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();
            DropColumns(ColumnsToDrop, TableType.Name);

            bool inactiveColMissing = false;
            StringBuilder columnCreator = new();

            foreach (string ColumnName in ColumnsToAdd)
            {
                columnCreator.Append($"ALTER TABLE {TableSchema}{Tablename} ADD ");

                if (ColumnName == "Inactive")
                {
                    inactiveColMissing = true;
                    columnCreator.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), ColumnName));
                    connection.JustExecute(columnCreator.ToString() + "NULL ", null, transaction);
                    CliConsoleLogs($"{columnCreator} NULL;");
                }
                else
                {
                    PropertyInfo Property = Properties.First(x => x.Name == ColumnName);

                    object[]? attributes = Property.GetCustomAttributes(true);
                    columnCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, ColumnName));
                    columnCreator.Append(AddColumnConstaints(attributes, TableType.Name, Property.Name, Property.PropertyType, false));

                    foreach (string commandText in columnCreator.ToString().Split("##"))
                    {
                        if (!string.IsNullOrEmpty(commandText.Trim()))
                        {
                            connection.JustExecute(commandText, null, transaction);
                            CliConsoleLogs($"{commandText};");
                        }
                    }
                }
                columnCreator.Clear();
            }

            if (inactiveColMissing)
            {
                string updateInactiveCol = $"Update Table {TableSchema}{Tablename} set {MyShit("Inactive")} = 0 where {MyShit("Inactive")} is null";
                connection.JustExecute(updateInactiveCol, null, transaction);
                CliConsoleLogs($"{updateInactiveCol};");
            }
        }

        void UpdateOpenTableSchema(Type TableType)
        {
            PKInfo pkInformation = ReadOpenEntityPrimaryKeys(TableType);
            bool canUpdatePKs = false;

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

            List<string> ColumnNames = connection.Query<string>(getColumns, null, transaction);

            bool PKsDropped = false;

            if (IsForcedUpdate)
            {
                List<string> CommonColumns = NewColumnNames.Intersect(ColumnNames).ToList();
                UpdateTableColumnsNullability(CommonColumns, Properties, TableType.Name);
                List<TableParsingInfo> ExistingPkInfo = DbConstraints.Where(x => x.TableName.ToLower() == TableType.Name.ToLower() && x.PrimaryKey).ToList();
                canUpdatePKs = CompairPrimaryKeys(ExistingPkInfo, pkInformation.PKPropertyNames, TableType.Name);
                if (canUpdatePKs)
                {
                    string commandText = $"ALTER TABLE {TableSchema}{Tablename} DROP CONSTRAINT PK_{TableType.Name} ";
                    PKsDropped = connection.JustExecute(commandText, null, transaction);
                    CliConsoleLogs($"{commandText};");
                }
            }

            List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
            List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();
            DropColumns(ColumnsToDrop, TableType.Name);

            StringBuilder columnCreator = new();

            foreach (string ColumnName in ColumnsToAdd)
            {
                columnCreator.Append($"ALTER TABLE {TableSchema}{Tablename} ADD ");
                PropertyInfo? Property = Properties.First(x => x.Name == ColumnName);
                
                object[]? attributes = Property.GetCustomAttributes(true);
                columnCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, ColumnName));
                columnCreator.Append(AddColumnConstaints(attributes, TableType.Name, Property.Name, Property.PropertyType, pkInformation.PKPropertyNames.Contains(ColumnName)));

                foreach (string commandText in columnCreator.ToString().Split("##"))
                {
                    if (!string.IsNullOrEmpty(commandText.Trim()))
                    {
                        connection.JustExecute(commandText, null, transaction);
                        CliConsoleLogs($"{commandText};");
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
                string commandTxt = $"ALTER TABLE {TableSchema}{Tablename} ADD CONSTRAINT PK_{TableType.Name} PRIMARY KEY ({primaryKeys.Remove(0, 1)})";
                connection.JustExecute(commandTxt, null, transaction);
                CliConsoleLogs($"{commandTxt};");
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
                throw ProtectDbAndThrow($"Error at Table '{TableName}' on Primary Keys Configuration. You CAN ONLY change the PRIMARY KEYS of a Table in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'");
            }

            return false;
        }

        private void UpdateTableColumnsNullability(List<string> ColumnNames, PropertyInfo[] Properties, string TableName)
        {
            foreach(string ColumnName in ColumnNames)
            {
                PropertyInfo Property = Properties.First(x => x.Name == ColumnName);
                TableParsingInfo Column = DbConstraints.First(x=> x.TableName.ToLower() == TableName.ToLower() && x.ColumnName.ToLower() == ColumnName.ToLower());
                NullabilityUpdateCheck(Property, Column, TableName);
            }
        }

        private void NullabilityUpdateCheck(PropertyInfo newColumn,TableParsingInfo existingColumn, string TableName)
        {
            bool isNullable = existingColumn.Nullable;
            object[] bhAttributes = newColumn.GetCustomAttributes(true);

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

                if (nullabilityAtr is bool nullableColAtr)
                {
                    isNullable = nullableColAtr;
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
                if (existingColumn.Nullable)
                {
                    SetColumnToNotNull(newColumn.PropertyType, newColumn.Name, TableName, existingColumn);
                }
                else
                {
                    SetColumnToNull(TableName, newColumn.Name, existingColumn);
                }
            }
        }

        private void SetColumnToNotNull(Type PropType, string PropName, string TableName, TableParsingInfo ColumnInfo)
        {
            string defaultValCommand = GetDefaultValue(PropType);
            string updateTxt = $"Update {TableSchema}{MyShit(TableName)} set {MyShit(PropName)} = {defaultValCommand} where {MyShit(PropName)} is null";
            string alterTxt = $"ALTER TABLE {TableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} NOT NULL";
            connection.JustExecute(updateTxt, null, transaction);
            connection.JustExecute(alterTxt, null, transaction);
            CliConsoleLogs($"{updateTxt};");
            CliConsoleLogs($"{alterTxt};");
        }

        private void SetColumnToNull(string TableName, string PropName, TableParsingInfo ColumnInfo)
        {
            string alterTxt = $"ALTER TABLE {TableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} NULL";
            connection.JustExecute(alterTxt, null, transaction);
            CliConsoleLogs($"{alterTxt};");
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
                    throw ProtectDbAndThrow($"Error at Table '{TableName}' on Dropping Columns. You CAN ONLY Drop Columns of a Table in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'");
                }
            }
        }

        string AddColumnConstaints(object[] attributes, string TableName, string PropName, Type PropType, bool isOpenPk)
        {
            if(isOpenPk && !IsForcedUpdate)
            {
                throw ProtectDbAndThrow($"Error at Entity '{TableName}' and Property '{PropName}'. You CAN ONLY change PRIMARY KEYS of a Table by using " +
                    "'DeveloperMode' or the 'update' CLI command with '--force' argument");
            }

            bool isNullable = true;
            bool mandatoryNull = false;
            string constraintsCommand = "NULL";
            string alterTable = $"ALTER TABLE {TableSchema}{MyShit(TableName)}";

            if (PropType.Name.Contains("Nullable"))
            {
                if (PropType.GenericTypeArguments != null && PropType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            Type fkAttributeType = typeof(ForeignKey);
            object? fkAttribute = attributes.FirstOrDefault(x => x.GetType() == fkAttributeType);

            if (fkAttribute != null)
            {
                var tName = fkAttributeType.GetProperty("TableName")?.GetValue(fkAttribute, null);
                var tColumn = fkAttributeType.GetProperty("Column")?.GetValue(fkAttribute, null);
                string cascadeInfo = "on delete cascade";

                if (fkAttributeType.GetProperty("Nullability")?.GetValue(fkAttribute, null) is bool Nullability)
                {
                    isNullable = Nullability;
                    cascadeInfo = "on delete set null";
                }

                if (mandatoryNull && !isNullable)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a NOT NULL column in the Database." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove the (?) from the Property's Type.");
                }

                if (isOpenPk && isNullable)
                {
                    throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' is marked as a PRIMARY KEY and it CAN NOT be NULLABLE." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove Property from the Primary Keys.");
                }

                if (!isNullable)
                {
                    throw ProtectDbAndThrow("CAN NOT Add a 'NOT NULLABLE' Foreign Key on an Existing Table. Please Change the Nullability on the " +
                        $"'[ForeignKey]' Attribute on the Property '{PropName}' of the Entity '{TableName}'.");
                }

                return $"{constraintsCommand} ## {MyShitConstraint(alterTable, TableName, PropName, tName, tColumn, cascadeInfo)}";
            }

            object? nnAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));

            if (nnAttribute != null)
            {
                isNullable = false;
                string nullPhase = "NULL ";

                if (mandatoryNull)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a 'NOT NULL' column in the Database." +
                        $"Please remove the (?) from the Property's Type or Remove the [NotNullable] Attribute.");
                }

                string defaultValCommand = GetDefaultValue(PropType);

                if (IsForcedUpdate)
                {
                    TableParsingInfo ColumnInfo = DbConstraints.First(x => x.TableName.ToLower() == TableName.ToLower() && x.ColumnName.ToLower() == PropName.ToLower());
                    nullPhase += $"## Update {TableSchema}{MyShit(TableName)} set {MyShit(PropName)} = {defaultValCommand} where {MyShit(PropName)} is null ";
                    nullPhase += $"## ALTER TABLE {TableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} NOT NULL";
                    return nullPhase;
                }
                else
                {
                    throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' REQUIRES 'DeveloperMode' or the 'update' CLI command with '--force' argument, " +
                        $"to be added as 'NOT NULLABLE' Column on an Existing Table. The default value of it, will be {defaultValCommand}.");
                }
            }

            if (isOpenPk)
            {
                throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' is marked as a PRIMARY KEY and it Requires the '[NotNullable]' Attribute.");
            }

            return constraintsCommand;
        }

        string GetSqlColumn(object[] attributes, Type PropertyType, bool isOpenPk, string PropName, string TableName)
        {
            bool mandatoryNull = false;
            bool isNullable = true;
            string nullPhase = "NULL, ";

            if (PropertyType.Name.Contains("Nullable"))
            {
                if (PropertyType.GenericTypeArguments != null && PropertyType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            object? fkAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));

            if (fkAttribute != null)
            {
                if(typeof(ForeignKey).GetProperty("Nullability")?.GetValue(fkAttribute, null) is bool Nullability)
                {
                    isNullable = Nullability;
                }

                nullPhase = isNullable ? "NULL, " : "NOT NULL, ";

                if (mandatoryNull && !isNullable)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a NOT NULL column in the Database." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove the (?) from the Property's Type.");
                }

                if (isOpenPk && isNullable)
                {
                    throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' is marked as a PRIMARY KEY and it CAN NOT be NULLABLE." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove Property from the Primary Keys.");
                }

                return nullPhase;
            }

            object? nnAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));

            if (nnAttribute != null)
            {
                isNullable = false;
                nullPhase = "NOT NULL, ";

                if (mandatoryNull)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a 'NOT NULL' column in the Database." +
                        $"Please remove the (?) from the Property's Type or Remove the [NotNullable] Attribute.");
                }

                return nullPhase;
            }

            if (isOpenPk)
            {
                throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' is marked as a PRIMARY KEY and it Requires the '[NotNullable]' Attribute.");
            }

            return "NULL, ";
        }

        private string SQLiteColumn(object[] attributes, bool firstTime, Type PropertyType, bool isOpenPk, string PropName, string TableName, bool wasNotNull)
        {
            if (isOpenPk && !firstTime && !IsForcedUpdate)
            {
                throw ProtectDbAndThrow($"Error at Entity '{TableName}' and Property '{PropName}'. You CAN ONLY change PRIMARY KEYS of a Table by using " +
                    "'DeveloperMode' or the 'update' CLI command with '--force' argument");
            }

            bool mandatoryNull = false;
            bool isNullable = true;
            string nullPhase = "NULL, ";

            if (PropertyType.Name.Contains("Nullable"))
            {
                if (PropertyType.GenericTypeArguments != null && PropertyType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            object? fkAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));

            if (fkAttribute != null)
            {
                if (typeof(ForeignKey).GetProperty("Nullability")?.GetValue(fkAttribute, null) is bool Nullability)
                {
                    isNullable = Nullability;
                }

                nullPhase = isNullable ? "NULL, " : "NOT NULL, ";

                if (mandatoryNull && !isNullable)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a NOT NULL column in the Database." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove the (?) from the Property's Type.");
                }

                if (isOpenPk && isNullable)
                {
                    throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' is marked as a PRIMARY KEY and it CAN NOT be NULLABLE." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove Property from the Primary Keys.");
                }

                if(!wasNotNull && !firstTime && !isNullable)
                {
                    throw ProtectDbAndThrow("CAN NOT Add a 'NOT NULLABLE' Foreign Key on an Existing Table. Please Change the Nullability on the " +
                        $"'[ForeignKey]' Attribute on the Property '{PropName}' of the Entity '{TableName}'.");
                }

                return nullPhase;
            }

            object? nnAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));

            if (nnAttribute != null)
            {
                isNullable = false;
                nullPhase = "NOT NULL, ";

                if (mandatoryNull)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a 'NOT NULL' column in the Database." +
                        $"Please remove the (?) from the Property's Type or Remove the [NotNullable] Attribute.");
                }

                if (!wasNotNull && !firstTime)
                {
                    string defaultValCommand = GetDefaultValue(PropertyType);

                    if (IsForcedUpdate)
                    {
                        return CheckLitePreviousColumnState(defaultValCommand, nullPhase, TableName, PropName);
                    }
                    else
                    {
                        throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' REQUIRES 'DeveloperMode' or the 'update' CLI command with '--force' argument, " +
                            $"to be added as 'NOT NULLABLE' Column on an Existing Table. The default value of it, will be {defaultValCommand}.");
                    }
                }
                return nullPhase;
            }

            if (isOpenPk)
            {
                throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' is marked as a PRIMARY KEY and it Requires the '[NotNullable]' Attribute.");
            }

            return "NULL, ";
        }

        string CheckLitePreviousColumnState(string defaultVal, string nullPhase, string TableName , string ColumnName)
        {
            TableParsingInfo? oldColumn = DbConstraints.FirstOrDefault(x=>x.TableName.ToLower() == TableName.ToLower() && x.ColumnName.ToLower() == ColumnName.ToLower());
            if(oldColumn != null && !oldColumn.Nullable)
            {
                return nullPhase;
            }
            return $"default {defaultVal} {nullPhase}";
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

        string TransferOldTableData(List<string> CommonList, string newTablename, string oldTablename)
        {
            string result = "";
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

        Exception ProtectDbAndThrow(string errorMessage)
        {
            transaction.DoNotCommit();
            transaction.Dispose();
            CliConsoleLogs(errorMessage);
            return new Exception(errorMessage);
        }

        internal void UpdateWithoutForceWarning()
        {
            if (CliCommand.CliExecution)
            {
                Console.WriteLine("_bhLog_ \t Update finished");
                Console.WriteLine("_bhLog_");
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
                   throw ProtectDbAndThrow($"Unsupported property type {PropertyType.FullName}");
            }

            return dataCommand;
        }

        string GetDefaultValue(Type PropertyType)
        {
            return PropertyType.Name switch
            {
                "String" => "'-'",
                "Char" => "'-'",
                "Int16" => "0",
                "Int32" => "0",
                "Int64" => "0",
                "Decimal" => "0",
                "Single" => "0",
                "Double" => "0",
                "Guid" => $"'{Guid.Empty}'",
                "Boolean" => "0",
                "DateTime" => $"'{DateTime.MinValue.ToString(DatabaseStatics.DbDateFormat)}'",
                "Byte[]" => $"'{new byte[0]}'",
                _ => throw ProtectDbAndThrow($"Unsupported property type {PropertyType.FullName}"),
            };
        }
    }
}
