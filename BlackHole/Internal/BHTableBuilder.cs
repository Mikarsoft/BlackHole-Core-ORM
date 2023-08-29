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

        private List<DataConstraints> AllConstraints { get; set; }
        private readonly string[] SqlDatatypes;
        private readonly bool IsMyShit;
        private readonly bool IsLite;
        private readonly bool IsOpenPKConstraint;
        private string TableSchemaCheck { get; set; }
        private string TableSchema { get; set; }
        private string TableSchemaFk { get; set; }

        internal BHTableBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
            SqlDatatypes = _multiDatabaseSelector.SqlDatatypesTranslation();
            TableSchemaCheck = _multiDatabaseSelector.TableSchemaCheck();
            TableSchema = _multiDatabaseSelector.GetDatabaseSchema();
            TableSchemaFk = _multiDatabaseSelector.GetDatabaseSchemaFk();
            IsMyShit = _multiDatabaseSelector.GetMyShit();
            IsLite = _multiDatabaseSelector.IsLite();
            IsOpenPKConstraint = _multiDatabaseSelector.GetOpenPKConstraint();
            AllConstraints = GetConstraints();
            SqlWriter = new BHSqlExportWriter("2_TablesSql","SqlFiles","sql");
        }

        /// <summary>
        /// Build a Table using a List of BlazarEntity or BlazarEntityWithActivator Types. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableTypes"></param>
        /// <param name="OpenTableTypes"></param>
        internal void BuildMultipleTables(List<Type> TableTypes, List<Type> OpenTableTypes)
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

            if (CliCommand.ExportSql)
            {
                SqlWriter.CreateSqlFile();
            }
        }

        bool CreateOpenTable(Type TableType)
        {
            if (!CheckTable(TableType.Name))
            {
                List<string> pkSettings = ReadOpenEntityPrimaryKeys(TableType);

                PropertyInfo[] Properties = TableType.GetProperties();
                StringBuilder tableCreator = new();
                string Pkoption = OpenPrimaryKey(pkSettings, TableType.Name);

                tableCreator.Append($"CREATE TABLE {TableSchema}{MyShit(TableType.Name)} (");

                foreach (PropertyInfo Property in Properties)
                {
                    object[] attributes = Property.GetCustomAttributes(true);

                    tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                    tableCreator.Append(GetSqlColumn(attributes, true, Property.PropertyType));
                }

                string creationCommand = tableCreator.ToString();
                creationCommand = $"{creationCommand[..^2]}{Pkoption})";
                CliConsoleLogs($"{creationCommand};");
                return connection.JustExecute(creationCommand, null);
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
                tableCreator.Append(GetSqlColumn(new object[] {new DefaultValue(0)}, true, typeof(int)));

                foreach (PropertyInfo Property in Properties)
                {
                    object[] attributes = Property.GetCustomAttributes(true);

                    if (Property.Name != "Id")
                    {
                        tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));

                        tableCreator.Append(GetSqlColumn(attributes, true, Property.PropertyType));
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
                return connection.JustExecute(creationCommand , null);
            }

            DatabaseStatics.InitializeData = false;
            return false;
        }

        bool CheckTable(string Tablename)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlLite => connection.ExecuteScalar<string>($@"SELECT name FROM sqlite_master WHERE type='table' AND name='" + Tablename + "'", null) == Tablename,
                BlackHoleSqlTypes.Oracle => connection.ExecuteScalar<string>($"SELECT table_name FROM all_tables WHERE owner ='{_multiDatabaseSelector.GetDatabaseName()}' and TABLE_NAME = '{Tablename}'", null) == Tablename,
                _ => connection.ExecuteScalar<int>($"select case when exists((select * from information_schema.tables where table_name = '" + Tablename + $"' {TableSchemaCheck})) then 1 else 0 end", null) == 1,
            };
        }

        List<string> ReadOpenEntityPrimaryKeys(Type openEntity)
        {
            List<string> pkNames = new();
            var pkOptionsBuilderType = typeof(PKOptionsBuilder<>).MakeGenericType(openEntity);
            object? pkOptionsBuilderObj = Activator.CreateInstance(pkOptionsBuilderType, new object[] { });

            ConstructorInfo? openEntityConstructor = openEntity.GetConstructor(Type.EmptyTypes);
            object? openEntityObj = openEntityConstructor?.Invoke(new object[] { });

            MethodInfo? pkOptionsMethod = openEntity?.GetMethod("PrimaryKeyOptions");
            object? pkSettingsObj = pkOptionsMethod?.Invoke(openEntityObj, new object?[] { pkOptionsBuilderObj });

            if(pkSettingsObj != null)
            {
                if(pkSettingsObj.GetType().GetProperty("PKSettingsList")?.GetValue(pkSettingsObj, null) is List<PrimaryKeySettings> pkSettings)
                {
                    foreach(PrimaryKeySettings pkSetting in pkSettings)
                    {
                        pkNames.Add(pkSetting.PropertyName);
                    }
                }
            }
            return pkNames;
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

            foreach (SqLiteTableInfo column in connection.Query<SqLiteTableInfo>($"PRAGMA table_info({MyShit(TableType.Name)}); ", null))
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

            List<string> ColumnNames = new();
            List<string> NewColumnNames = new()
            {
                "Inactive"
            };

            List<SqLiteForeignKeySchema> SchemaInfo = connection.Query<SqLiteForeignKeySchema>($"PRAGMA foreign_key_list({Tablename});", null);
            List<SqLiteTableInfo> ColumnsInfo = connection.Query<SqLiteTableInfo>($"PRAGMA table_info({Tablename});", null);

            foreach (SqLiteTableInfo column in ColumnsInfo)
            {
                ColumnNames.Add(column.name);
            }

            StringBuilder alterTable = new();
            StringBuilder foreignKeys = new();
            StringBuilder closingCommand = new();

            alterTable.Append($"PRAGMA foreign_keys = off; ALTER TABLE {Tablename} RENAME TO {Tablename}_Old; CREATE TABLE {Tablename} (");
            alterTable.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), "Inactive"));
            alterTable.Append(GetSqlColumn(new object[] { new DefaultValue(0) }, true, typeof(int)));

            foreach (PropertyInfo Property in TableType.GetProperties())
            {
                object[] attributes = Property.GetCustomAttributes(true);
                NewColumnNames.Add(Property.Name);

                if (Property.Name != "Id")
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));

                    alterTable.Append(GetSqlColumn(attributes, firstTime, Property.PropertyType));
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
                        foreignKeys.Append(LiteConstraint(Tablename, Property.Name, tName, tColumn, cascadeInfo));
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
            closingCommand.Append($"{TransferOldTableData(ColumnNames, NewColumnNames, $"{Tablename}", $"{Tablename}_Old")} DROP TABLE {Tablename}_Old;");
            closingCommand.Append($"ALTER TABLE {Tablename} RENAME TO {Tablename}_Old; ALTER TABLE {Tablename}_Old RENAME TO {Tablename};");
            closingCommand.Append($"PRAGMA foreign_keys = on; DROP INDEX IF EXISTS {Tablename}_Old");
            connection.JustExecute(closingCommand.ToString(), null);
            CliConsoleLogs($"{closingCommand}");
            closingCommand.Clear();
        }


        void ForeignKeyLiteOpenAsignment(Type TableType, bool firstTime)
        {
            string Tablename = MyShit(TableType.Name);
            List<string> pkSettings = ReadOpenEntityPrimaryKeys(TableType);
            string Pkoption = OpenPrimaryKey(pkSettings, Tablename);

            List<string> ColumnNames = new();
            List<string> NewColumnNames = new();

            List<SqLiteForeignKeySchema> SchemaInfo = connection.Query<SqLiteForeignKeySchema>($"PRAGMA foreign_key_list({Tablename});", null);
            List<SqLiteTableInfo> ColumnsInfo = connection.Query<SqLiteTableInfo>($"PRAGMA table_info({Tablename});", null);

            foreach (SqLiteTableInfo column in ColumnsInfo)
            {
                ColumnNames.Add(column.name);
            }

            StringBuilder alterTable = new();
            StringBuilder foreignKeys = new();
            StringBuilder closingCommand = new();

            alterTable.Append($"PRAGMA foreign_keys=off; ALTER TABLE {Tablename} RENAME TO {Tablename}_Old; CREATE TABLE {Tablename} (");

            foreach (PropertyInfo Property in TableType.GetProperties())
            {
                object[] attributes = Property.GetCustomAttributes(true);
                NewColumnNames.Add(Property.Name);

                alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name));
                alterTable.Append(GetSqlColumn(attributes, firstTime, Property.PropertyType));

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.SingleOrDefault(x => x.GetType() == typeof(ForeignKey));

                    if (FK_attribute != null)
                    {
                        var tName = FK_attribute.GetType().GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FK_attribute.GetType().GetProperty("Column")?.GetValue(FK_attribute, null);
                        var cascadeInfo = FK_attribute.GetType().GetProperty("CascadeInfo")?.GetValue(FK_attribute, null);
                        foreignKeys.Append(LiteConstraint(Tablename, Property.Name, tName, tColumn, cascadeInfo));
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
                            foreignKeys.Append($"CONSTRAINT fk_{Tablename}_{fkC.table} FOREIGN KEY ({fkC.from}) REFERENCES {fkC.table}({fkC.to}) on delete {fkC.on_delete}, ");
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
            closingCommand.Append($"{TransferOldTableData(ColumnNames, NewColumnNames, $"{Tablename}", $"{Tablename}_Old")} DROP TABLE {Tablename}_Old;");
            closingCommand.Append($"ALTER TABLE {Tablename} RENAME TO {Tablename}_Old; ALTER TABLE {Tablename}_Old RENAME TO {Tablename};");
            closingCommand.Append($"PRAGMA foreign_keys = on; DROP INDEX IF EXISTS {Tablename}_Old");
            string test = closingCommand.ToString();
            connection.JustExecute(closingCommand.ToString(), null);
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
                        connection.JustExecute(alterColumn, null);
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

            List<string> ColumnNames = connection.Query<string>(getColumns, null);
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
                            connection.JustExecute(commandText, null);
                            CliConsoleLogs($"{commandText};");
                        }
                    }
                }

                if (ColumnName == "Inactive")
                {
                    columnCreator.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), ColumnName));
                    connection.JustExecute(columnCreator.ToString(), null);
                    CliConsoleLogs($"{columnCreator};");
                }

                columnCreator.Clear();
            }
        }

        void UpdateOpenTableSchema(Type TableType)
        {
            string getColumns = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableType.Name}' {TableSchemaCheck}";

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.Oracle)
            {
                string owner = _multiDatabaseSelector.GetDatabaseName();
                getColumns = $"SELECT Column_name From all_tab_cols where owner = '{owner}' and TABLE_NAME = '{TableType.Name}'";
            }

            PropertyInfo[] Properties = TableType.GetProperties();
            string Tablename = MyShit(TableType.Name);

            List<string> NewColumnNames = new();

            foreach (PropertyInfo Property in Properties)
            {
                NewColumnNames.Add(Property.Name);
            }

            List<string> ColumnNames = connection.Query<string>(getColumns, null);
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
                            connection.JustExecute(commandText, null);
                            CliConsoleLogs($"{commandText};");
                        }
                    }
                }

                columnCreator.Clear();
            }
        }

        private void DropColumns(List<string> ColumnsToDrop, string TableName)
        {
            if (DatabaseStatics.IsDevMove || CliCommand.ForceAction)
            {
                foreach (string ColumnName in ColumnsToDrop)
                {
                    List<DataConstraints> columnForeignKeys = AllConstraints.Where(x => x.TABLE_NAME.ToLower() == TableName.ToLower()
                            && x.COLUMN_NAME.ToLower() == ColumnName.ToLower()).ToList();

                    foreach(DataConstraints columnConstraint in columnForeignKeys)
                    {
                        string dropConstraint = $"ALTER TABLE {TableSchema}{MyShit(TableName)} DROP CONSTRAINT {columnConstraint.CONSTRAINT_NAME}";
                        connection.JustExecute(dropConstraint, null);
                        CliConsoleLogs($"{dropConstraint};");
                    }

                    string dropCommand = $"ALTER TABLE {TableSchema}{MyShit(TableName)} DROP COLUMN {MyShit(ColumnName)}";
                    connection.JustExecute(dropCommand, null);
                    CliConsoleLogs($"{dropCommand};");
                }
            }
            else
            {
                string setColumnToNull = "";
                List<SqlTableInfo> TableInfo = new();
                switch (DatabaseStatics.DatabaseType)
                {
                    case BlackHoleSqlTypes.Oracle:
                        string getColumnInfo = @$"SELECT Column_name, Data_type, data_length, data_precision, NULLABLE From all_tab_cols 
                                           where owner = '{_multiDatabaseSelector.GetDatabaseName()}' and TABLE_NAME = '{TableName}'";

                        List<OracleTableInfo> OraTableInfo = connection.Query<OracleTableInfo>(getColumnInfo, null);

                        if (OraTableInfo.Count > 0)
                        {
                            foreach (string ColumnName in ColumnsToDrop)
                            {
                                OracleTableInfo? columnInfo = OraTableInfo.Where(x => x.COLUMN_NAME.ToLower() == ColumnName.ToLower()).FirstOrDefault();
                                if (columnInfo != null)
                                {
                                    string DataType = GetOracleDataType(columnInfo);
                                    string setToNullable = $"ALTER TABLE {MyShit(TableName)} Modify ({MyShit(ColumnName)} NULL)";
                                    connection.JustExecute(setToNullable, null);
                                    CliConsoleLogs($"{setToNullable};");
                                }
                            }
                        }
                        break;
                    case BlackHoleSqlTypes.MySql:
                        setColumnToNull = @$" select table_name,column_name, ordinal_position, data_type, character_maximum_length, is_nullable, column_type 
                                            from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{TableName}' {TableSchemaCheck};";

                        TableInfo = connection.Query<SqlTableInfo>(setColumnToNull, null);

                        if (TableInfo.Count > 0)
                        {
                            foreach (string ColumnName in ColumnsToDrop)
                            {
                                SqlTableInfo? columnInfo = TableInfo.Where(x => x.column_name.ToLower() == ColumnName.ToLower()).FirstOrDefault();
                                if (columnInfo != null)
                                {
                                    string DataType = columnInfo.column_type;
                                    string setToNullable = $"ALTER TABLE {TableSchema}{MyShit(TableName)} MODIFY {MyShit(ColumnName)} {DataType} NULL ";
                                    connection.JustExecute(setToNullable, null);
                                    CliConsoleLogs($"{setToNullable};");
                                }
                            }
                        }

                        break;
                    case BlackHoleSqlTypes.Postgres:
                        setColumnToNull = @$" select table_name,column_name, ordinal_position, data_type, character_maximum_length, is_nullable, udt_name 
                                            from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{TableName}' {TableSchemaCheck};";

                        TableInfo = connection.Query<SqlTableInfo>(setColumnToNull, null);

                        if (TableInfo.Count > 0)
                        {
                            foreach (string ColumnName in ColumnsToDrop)
                            {
                                SqlTableInfo? columnInfo = TableInfo.Where(x => x.column_name.ToLower() == ColumnName.ToLower()).FirstOrDefault();
                                if (columnInfo != null)
                                {
                                    string DataType = columnInfo.udt_name;

                                    if(columnInfo.character_maximum_length > 0)
                                    {
                                        DataType += $"({columnInfo.character_maximum_length})";
                                    }

                                    string setToNullable = $"ALTER TABLE {TableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(ColumnName)} {DataType} NULL ";
                                    connection.JustExecute(setToNullable, null);
                                    CliConsoleLogs($"{setToNullable};");
                                }
                            }
                        }

                        break;
                    default:
                        setColumnToNull = @$" select table_name,column_name, ordinal_position, data_type, character_maximum_length, is_nullable 
                                            from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{TableName}' {TableSchemaCheck};";

                        TableInfo = connection.Query<SqlTableInfo>(setColumnToNull, null);

                        if (TableInfo.Count > 0)
                        {
                            foreach (string ColumnName in ColumnsToDrop)
                            {
                                SqlTableInfo? columnInfo = TableInfo.Where(x => x.column_name.ToLower() == ColumnName.ToLower()).FirstOrDefault();
                                if (columnInfo != null)
                                {
                                    string DataType = columnInfo.data_type;

                                    if (columnInfo.character_maximum_length > 0)
                                    {
                                        DataType += $"({columnInfo.character_maximum_length})";
                                    }

                                    string setToNullable = $"ALTER TABLE {TableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(ColumnName)} {DataType} NULL ";
                                    connection.JustExecute(setToNullable, null);
                                    CliConsoleLogs($"{setToNullable};");
                                }
                            }
                        }

                        break;
                }
            }
        }

        private List<DataConstraints> GetConstraints()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => LoadMsSqlConstraints(),
                BlackHoleSqlTypes.MySql => LoadMySqlConstraints(),
                BlackHoleSqlTypes.Postgres => LoadPgConstraints(),
                BlackHoleSqlTypes.Oracle => LoadOracleConstraints(),
                _ => new(),
            };
        }

        private List<DataConstraints> LoadMySqlConstraints()
        {
            string GetConstrainsCommand = @"SELECT K.TABLE_NAME, K.COLUMN_NAME, K.REFERENCED_TABLE_NAME, C.IS_NULLABLE as DELETE_RULE, K.CONSTRAINT_NAME FROM
                INFORMATION_SCHEMA.KEY_COLUMN_USAGE K
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON (C.COLUMN_NAME= K.COLUMN_NAME AND C.TABLE_NAME=K.TABLE_NAME)
                WHERE REFERENCED_TABLE_NAME is not null";
            return connection.Query<DataConstraints>(GetConstrainsCommand, null);
        }

        private List<DataConstraints> LoadMsSqlConstraints()
        {
            string GetConstrainsCommand = @"SELECT K.TABLE_NAME,K.COLUMN_NAME,REFERENCED_TABLE_NAME=TC.TABLE_NAME,T.DELETE_RULE,T.CONSTRAINT_NAME FROM
	            INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS T
	            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON TC.CONSTRAINT_NAME=T.UNIQUE_CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE K ON K.CONSTRAINT_NAME= T.CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON (C.COLUMN_NAME= K.COLUMN_NAME AND C.TABLE_NAME=K.TABLE_NAME)";
            return connection.Query<DataConstraints>(GetConstrainsCommand, null);
        }

        private List<DataConstraints> LoadOracleConstraints()
        {
            string GetConstrainsCommand = @"SELECT a.table_name, a.column_name, a.constraint_name, c_pk.table_name REFERENCED_TABLE_NAME, c.delete_rule
                FROM all_cons_columns a
                JOIN all_constraints c ON a.owner = c.owner AND a.constraint_name = c.constraint_name
                JOIN all_constraints c_pk ON c.r_owner = c_pk.owner AND c.r_constraint_name = c_pk.constraint_name
                WHERE c.constraint_type = 'R' and a.owner =" + $"'{_multiDatabaseSelector.GetDatabaseName()}'";
            return connection.Query<DataConstraints>(GetConstrainsCommand, null);
        }

        private List<DataConstraints> LoadPgConstraints()
        {
            string GetConstrainsCommand = @"SELECT T.TABLE_NAME, K.COLUMN_NAME,CU.TABLE_NAME AS REFERENCED_TABLE_NAME,
                C.IS_NULLABLE AS DELETE_RULE, CU.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE K ON K.CONSTRAINT_NAME = T.CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME)
                INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CU ON CU.CONSTRAINT_NAME = T.CONSTRAINT_NAME
                WHERE T.CONSTRAINT_TYPE = 'FOREIGN KEY'";
            return connection.Query<DataConstraints>(GetConstrainsCommand, null);
        }

        private string GetOracleDataType(OracleTableInfo columnInfo)
        {
            string DataType = columnInfo.DATA_TYPE;

            if(DataType.ToLower().Contains("varchar"))
            {
                return DataType + $"({columnInfo.DATA_LENGTH})";
            }

            if(DataType.ToLower() == "number")
            {
                 return DataType + $"({columnInfo.DATA_PRECISION,0})";
            }

            return DataType;
        }

        string AddColumnConstaints(object[] attributes, string Tablename, string PropName, Type PropType)
        {
            string constraintsCommand = "NULL ##";
            string alterTable = $"ALTER TABLE {TableSchema}{MyShit(Tablename)}";
            Type fkAttributeType = typeof(ForeignKey);
            object? fkAttribute = attributes.SingleOrDefault(x => x.GetType() == fkAttributeType);

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

            object? nnAttribute = attributes.SingleOrDefault(x => x.GetType() == typeof(NotNullable));

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
                return $"{DefaultValueCheck(PropType, defaultValNotnull, usingDateTime)} NOT NULL ";
            }

            object? dvAttribute = attributes.SingleOrDefault(x => x.GetType() == typeof(DefaultValue));

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
                return $"{DefaultValueCheck(PropType, defaultVal, useDateTimeVal)} NULL ";
            }

            return constraintsCommand;
        }

        string GetSqlColumn(object[] attributes, bool firstTime, Type PropertyType)
        {
            bool mandatoryNull = false;
            string nullPhase = string.Empty;

            if (PropertyType.Name.Contains("Nullable"))
            {
                if (PropertyType.GenericTypeArguments != null && PropertyType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            Type Fk_Type = typeof(ForeignKey);
            object? fkAttribute = attributes.SingleOrDefault(x => x.GetType() == Fk_Type);

            if (fkAttribute != null)
            {
                var tNull = Fk_Type.GetProperty("IsNullable")?.GetValue(fkAttribute, null);
                nullPhase = firstTime ? $"{tNull}, " : "NULL, ";

                if (mandatoryNull)
                {
                    nullPhase = "NULL, ";
                }

                return nullPhase;
            }

            object? nnAttribute = attributes.SingleOrDefault(x => x.GetType() == typeof(NotNullable));

            if (nnAttribute != null)
            {
                Type nnAttributeType = typeof(NotNullable);

                var defaultValNotnull = nnAttributeType.GetProperty("ValueDefault")?.GetValue(nnAttribute, null);
                nullPhase = firstTime ? "NOT NULL, " : "NULL, ";

                if (mandatoryNull)
                {
                    nullPhase = "NULL, ";
                }

                var isDatetimeVal = nnAttributeType.GetProperty("IsDatetimeValue")?.GetValue(nnAttribute, null);
                bool useDateTime = false;
                if (isDatetimeVal is bool)
                {
                    useDateTime = (bool)isDatetimeVal;
                }

                return $"{DefaultValueCheck(PropertyType, defaultValNotnull,useDateTime)} {nullPhase}";
            }

            object? dvAttribute = attributes.SingleOrDefault(x => x.GetType() == typeof(DefaultValue));

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
                return $"{DefaultValueCheck(PropertyType, defaultVal,useDateTimeVal)} NULL, ";
            }

            if (mandatoryNull)
            {
                return "NULL, ";
            }

            return ", ";
        }

        string MyShit(string propName)
        {
            string result = propName;

            if (!IsMyShit)
            {
                result = $@"""{propName}""";
            }

            return result;
        }

        string MyShitConstraint(string alterTable, string Tablename, string propName, object? tName, object? tColumn, object? cascadeInfo)
        {
            string constraint = $"ADD CONSTRAINT fk_{Tablename}_{tName}{TableSchemaFk}";

            string result = $"{alterTable} {constraint} FOREIGN KEY ({propName}) REFERENCES {TableSchema}{tName}({tColumn}) {cascadeInfo}";

            if (!IsMyShit)
            {
                result = $@"{alterTable} {constraint} FOREIGN KEY (""{propName}"") REFERENCES {TableSchema}""{tName}""(""{tColumn}"") {cascadeInfo}";
            }

            return result;
        }

        string LiteConstraint(string Tablename, string propName, object? tName, object? tColumn, object? cascadeInfo)
        {
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
                    result += $"{column},";
                    selectionClosing += $"{column},";
                }
                result = $"{result[..^1]}{selection}{selectionClosing[..^1]} FROM {oldTablename};";
            }

            return result;
        }

        string DefaultValueCheck(Type PropertyType, object? defaultValue, bool useDateTime)
        {
            if(defaultValue != null)
            {
                if (IsNumericType(defaultValue))
                {
                    if (IsNumericType(PropertyType))
                    {
                        return $" default {defaultValue} ";
                    }
                }
                else
                {
                    bool isDateTimeProp = IsDateTimeType(PropertyType);

                    if (useDateTime)
                    {
                        if (isDateTimeProp)
                        {
                            return $" default '{defaultValue}' ";
                        }
                    }
                    else
                    {
                        if (!IsNumericType(PropertyType) && !isDateTimeProp)
                        {
                            return $" default '{defaultValue}' ";
                        }
                    }
                }
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
                    object? CharLength = attributes.Where(x => x.GetType() == typeof(VarCharSize)).FirstOrDefault();
                    object? ForeignKeyAtt = attributes.Where(x => x.GetType() == typeof(ForeignKey)).FirstOrDefault();

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
