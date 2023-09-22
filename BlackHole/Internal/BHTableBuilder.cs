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
        internal BHDatabaseInfoReader dbInfoReader { get; set; }
        internal bool IsForcedUpdate { get; } 
        internal bool IsTrashOracleProduct { get; set; }
        private string AlterColumn { get; }
        private List<string> CreateTablesTransaction { get; set; } = new();
        private List<string> CustomTransaction { get; set; } = new();
        private List<string> AfterMath { get; set; } = new();
        private List<string> DropTransaction { get; set; } = new();
        private List<string> RevertShittyOracleTables { get; set; } = new();
        private List<string> RevertShittyOracleTransaction { get; set; } = new();
        private List<string> RevertShittyOracleAfterMath { get; set; } = new();

        internal BHTableBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
            IsForcedUpdate = DatabaseStatics.AutoUpdate && (DatabaseStatics.IsDevMove || CliCommand.ForceAction);
            dbInfoReader = new BHDatabaseInfoReader(connection, _multiDatabaseSelector);
            AlterColumn = _multiDatabaseSelector.GetColumnModifyCommand();
            IsTrashOracleProduct = _multiDatabaseSelector.IsFuckingShittyTrashOracleProduct();

            if (IsForcedUpdate)
            {
                DbConstraints = dbInfoReader.GetDatabaseParsingInfo();
            }

            SqlDatatypes = _multiDatabaseSelector.SqlDatatypesTranslation();
            TableSchemaCheck = _multiDatabaseSelector.TableSchemaCheck();
            TableSchema = _multiDatabaseSelector.GetDatabaseSchema();
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

                if (!ExecuteTableCreation())
                {
                    Thread.Sleep(2000);
                    throw ProtectDbAndThrow("Something went wrong with the Update of the Database.The Database is not changed. Please check the BlackHole logs to detect and fix the problem.");
                }
                ClearCommands();
            }
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
                            tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
                            tableCreator.Append(GetSqlColumn(attributes, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name,TableType.Name));
                        }
                        else
                        {
                            tableCreator.Append(_multiDatabaseSelector.GetCompositePrimaryKeyCommand(Property.PropertyType, pkInformation.MainPrimaryKey));
                        }
                    }
                    else
                    {
                        tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
                        tableCreator.Append(GetSqlColumn(attributes, Property.PropertyType, pkSettings.Contains(Property.Name), Property.Name, TableType.Name));
                    }
                }

                string creationCommand = tableCreator.ToString();
                creationCommand = $"{creationCommand[..^2]}{Pkoption})";
                CliConsoleLogs($"{creationCommand};");
                CreateTablesTransaction.Add(creationCommand);
                if (IsTrashOracleProduct)
                {
                    RevertShittyOracleTables.Add($"DROP TABLE {MyShit(TableType.Name)}");
                }
                return true;
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

                tableCreator.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), "Inactive", TableType.Name));
                tableCreator.Append(" NULL, ");

                foreach (PropertyInfo Property in Properties)
                {
                    object[] attributes = Property.GetCustomAttributes(true);

                    if (Property.Name != "Id")
                    {
                        tableCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));

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
                CreateTablesTransaction.Add(creationCommand);
                if (IsTrashOracleProduct)
                {
                    RevertShittyOracleTables.Add($"DROP TABLE {MyShit(TableType.Name)}");
                }
                return true;
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
                ForeignKeyLiteOpenAsignment(TableType, true, ReadOpenEntityPrimaryKeys(TableType));
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

            List<string> CommonList = ColumnNames.Intersect(NewColumnNames).ToList();

            if (CommonList.Count != NewColumnNames.Count || CommonList.Count != ColumnNames.Count)
            {
                ForeignKeyLiteAsignment(TableType, false);
            }
        }

        void UpdateLiteOpenTableSchema(Type TableType)
        {
            List<string> ColumnNames = new();
            List<string> NewColumnNames = new();
            PKInfo pkInformation = ReadOpenEntityPrimaryKeys(TableType);
            List<string> existingPks = new();

            foreach (PropertyInfo Property in TableType.GetProperties())
            {
                NewColumnNames.Add(Property.Name);
            }

            foreach (SqLiteTableInfo column in connection.Query<SqLiteTableInfo>($"PRAGMA table_info({MyShit(TableType.Name)}); ", null))
            {
                ColumnNames.Add(column.name);
                if(column.pk > 0)
                {
                    existingPks.Add(column.name);
                }
            }

            List<string> CommonList = ColumnNames.Intersect(NewColumnNames).ToList();
            List<string> CommonPk = pkInformation.PKPropertyNames.Intersect(existingPks).ToList();

            if (CommonList.Count != NewColumnNames.Count || CommonList.Count != ColumnNames.Count
                || CommonPk.Count != existingPks.Count || CommonPk.Count != pkInformation.PKPropertyNames.Count)
            {
                ForeignKeyLiteOpenAsignment(TableType, false, pkInformation);
            }
        }

        void ForeignKeyLiteAsignment(Type TableType, bool firstTime)
        {
            string Tablename = MyShit(TableType.Name);
            string OldTablename = MyShit($"{TableType.Name}_Old");
            List<FKInfo> FkOptions = new();
            Type FkType = typeof(ForeignKey);
            List<UniqueInfo> UniqueOptions = new();
            Type UQType = typeof(Unique);

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
            bool missingInactiveColumn = ColumnsToAdd.Contains("Inactive");
            List<string> CommonColumns = ColumnNames.Intersect(NewColumnNames).ToList();

            StringBuilder alterTable = new();
            StringBuilder foreignKeys = new();
            StringBuilder closingCommand = new();

            alterTable.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; CREATE TABLE {Tablename} (");
            alterTable.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), "Inactive", TableType.Name));
            alterTable.Append(" NULL, ");

            foreach (string AddColumn in CommonColumns.Where(x=> x != "Inactive"))
            { 
                PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
                object[] attributes = Property.GetCustomAttributes(true);

                if (Property.Name != "Id")
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
                    SqLiteTableInfo existingCol = ColumnsInfo.First(x => x.name == AddColumn);
                    alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, false, Property.Name, TableType.Name, existingCol.notnull, false));
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
                    object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == FkType);
                    bool nullability = true;
                    if (FK_attribute != null)
                    {
                        var tName = FkType.GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FkType.GetProperty("Column")?.GetValue(FK_attribute, null);
                        if (FkType.GetProperty("Nullability")?.GetValue(FK_attribute, null) is bool isNullable)
                        {
                            nullability = isNullable;
                            FkOptions.Add(AddForeignKey(Property.Name, tName, tColumn, isNullable));
                        }
                    }
                    else
                    {
                        object? NN_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
                        if (NN_attribute != null)
                        {
                            nullability = false;
                        }
                    }

                    object? UQ_attribute = attributes.FirstOrDefault(x => x.GetType() == UQType);

                    if (UQ_attribute != null)
                    {
                        if (UQType.GetProperty("UniqueGroupId")?.GetValue(UQ_attribute, null) is int groupId)
                        {
                            if (!nullability)
                            {
                                UniqueOptions.Add(AddUniqueConstraint(Property.Name, groupId));
                            }
                            else
                            {
                                throw ProtectDbAndThrow($"Property '{Property.Name}' of Entity '{TableType.Name}' is marked with UNIQUE CONSTRAINT and it Requires to be 'NOT NULL'.");
                            }
                        }
                    }
                }
            }

            foreach (string AddColumn in ColumnsToAdd.Where(x => x != "Inactive"))
            {
                if(AddColumn != "Id")
                {
                    PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
                    object[] attributes = Property.GetCustomAttributes(true);
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
                    alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, false, Property.Name, TableType.Name, false, false));

                    if (attributes.Length > 0)
                    {
                        object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == FkType);
                        bool nullability = true;
                        if (FK_attribute != null)
                        {
                            var tName = FkType.GetProperty("TableName")?.GetValue(FK_attribute, null);
                            var tColumn = FkType.GetProperty("Column")?.GetValue(FK_attribute, null);
                            if (FkType.GetProperty("Nullability")?.GetValue(FK_attribute, null) is bool isNullable)
                            {
                                nullability = isNullable;
                                FkOptions.Add(AddForeignKey(Property.Name, tName, tColumn, isNullable));
                            }
                        }
                        else
                        {
                            object? NN_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
                            if (NN_attribute != null)
                            {
                                nullability = false;
                            }
                        }

                        object? UQ_attribute = attributes.FirstOrDefault(x => x.GetType() == UQType);

                        if (UQ_attribute != null)
                        {
                            if (UQType.GetProperty("UniqueGroupId")?.GetValue(UQ_attribute, null) is int groupId)
                            {
                                if (!nullability)
                                {
                                    UniqueOptions.Add(AddUniqueConstraint(Property.Name, groupId));
                                }
                                else
                                {
                                    throw ProtectDbAndThrow($"Property '{Property.Name}' of Entity '{TableType.Name}' is marked with UNIQUE CONSTRAINT and it Requires to be 'NOT NULL'.");
                                }
                            }
                        }
                    }
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
            }

            string FkCommand = $"{alterTable}{CreateForeignKeyConstraintLite(FkOptions, TableType.Name)}{CreateUniqueConstraintLite(UniqueOptions, TableType.Name)}";

            if (FkCommand.Length > 1)
            {
                FkCommand = $"{FkCommand[..(FkCommand.Length - 2)]}); ";
            }

            alterTable.Clear(); foreignKeys.Clear();
            closingCommand.Append(FkCommand);
            closingCommand.Append($"{TransferOldTableData(CommonColumns, Tablename, OldTablename)} DROP TABLE {OldTablename};");
            closingCommand.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; ALTER TABLE {OldTablename} RENAME TO {Tablename};");
            closingCommand.Append($" DROP INDEX IF EXISTS {OldTablename}");
            CustomTransaction.Add(closingCommand.ToString());
            CliConsoleLogs($"{closingCommand}");
            closingCommand.Clear();

            if (missingInactiveColumn)
            {
                string updateInactiveCol = $"Update {TableSchema}{Tablename} set {MyShit("Inactive")} = 0 where {MyShit("Inactive")} is null;";
                AfterMath.Add(updateInactiveCol);
                CliConsoleLogs($"{updateInactiveCol};");
            }
        }

        void ForeignKeyLiteOpenAsignment(Type TableType, bool firstTime, PKInfo pkInformation)
        {
            string Tablename = MyShit(TableType.Name);
            string OldTablename = MyShit($"{TableType.Name}_Old");
            List<string> pkSettings = pkInformation.PKPropertyNames;
            string Pkoption = OpenPrimaryKey(pkSettings, TableType.Name);
            List<FKInfo> FkOptions = new();
            Type FkType = typeof(ForeignKey);
            List<UniqueInfo> UniqueOptions = new();
            Type UQType = typeof(Unique);

            List<string> ColumnNames = new();
            List<string> NewColumnNames = new();

            List<SqLiteForeignKeySchema> SchemaInfo = connection.Query<SqLiteForeignKeySchema>($"PRAGMA foreign_key_list({Tablename});", null);
            List<SqLiteTableInfo> ColumnsInfo = connection.Query<SqLiteTableInfo>($"PRAGMA table_info({Tablename});", null);

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

            alterTable.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; CREATE TABLE {Tablename} (");

            foreach (string AddColumn in CommonColumns)
            {
                PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
                object[] attributes = Property.GetCustomAttributes(true);
                SqLiteTableInfo existingCol = ColumnsInfo.First(x => x.name == AddColumn);
                bool isOpenPk = pkSettings.Contains(Property.Name);
                if (pkInformation.HasAutoIncrement)
                {
                    if (Property.Name != pkInformation.MainPrimaryKey)
                    {
                        alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
                        alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, isOpenPk,
                            Property.Name, TableType.Name, existingCol.notnull, HasLitePkChanged(existingCol.pk,isOpenPk)));
                    }
                    else
                    {
                        alterTable.Append(_multiDatabaseSelector.GetCompositePrimaryKeyCommand(Property.PropertyType, pkInformation.MainPrimaryKey));
                    }
                }
                else
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
                    alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, isOpenPk,
                        Property.Name, TableType.Name, existingCol.notnull, HasLitePkChanged(existingCol.pk,isOpenPk)));
                }

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == FkType);
                    bool nullability = true;

                    if (FK_attribute != null)
                    {
                        var tName = FkType.GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FkType.GetProperty("Column")?.GetValue(FK_attribute, null);
                        if(FkType.GetProperty("Nullability")?.GetValue(FK_attribute, null) is bool isNullable)
                        {
                            nullability = isNullable;
                            FkOptions.Add(AddForeignKey(Property.Name,tName,tColumn,isNullable));
                        }
                    }
                    else
                    {
                        object? NN_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
                        if (NN_attribute != null)
                        {
                            nullability = false;
                        }
                    }

                    object? UQ_attribute = attributes.FirstOrDefault(x => x.GetType() == UQType);

                    if(UQ_attribute != null)
                    {
                        if(UQType.GetProperty("UniqueGroupId")?.GetValue(UQ_attribute,null) is int groupId)
                        {
                            if (!nullability)
                            {
                                UniqueOptions.Add(AddUniqueConstraint(Property.Name, groupId));
                            }
                            else
                            {
                                throw ProtectDbAndThrow($"Property '{Property.Name}' of Entity '{TableType.Name}' is marked with UNIQUE CONSTRAINT and it Requires to be 'NOT NULL'.");
                            }
                        }
                    }
                }
            }

            foreach (string AddColumn in ColumnsToAdd)
            {
                PropertyInfo Property = Properties.First(x => x.Name == AddColumn);
                object[] attributes = Property.GetCustomAttributes(true);
                bool isOpenPk = pkSettings.Contains(Property.Name);

                if (pkInformation.HasAutoIncrement)
                {
                    if(Property.Name != pkInformation.MainPrimaryKey)
                    {
                        alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
                        alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, isOpenPk,
                            Property.Name,TableType.Name, false, HasLitePkChanged(0,isOpenPk)));
                    }
                    else
                    {
                        alterTable.Append(_multiDatabaseSelector.GetCompositePrimaryKeyCommand(Property.PropertyType, pkInformation.MainPrimaryKey));
                    }
                }
                else
                {
                    alterTable.Append(GetDatatypeCommand(Property.PropertyType, attributes, Property.Name, TableType.Name));
                    alterTable.Append(SQLiteColumn(attributes, firstTime, Property.PropertyType, isOpenPk,
                        Property.Name,TableType.Name, false, HasLitePkChanged(0,isOpenPk)));
                }

                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.FirstOrDefault(x => x.GetType() == FkType);
                    bool nullability = true;
                    if (FK_attribute != null)
                    {
                        var tName = FkType.GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FkType.GetProperty("Column")?.GetValue(FK_attribute, null);
                        if(FkType.GetProperty("Nullability")?.GetValue(FK_attribute, null) is bool isNullable)
                        {
                            nullability = isNullable;
                            FkOptions.Add(AddForeignKey(Property.Name,tName,tColumn,isNullable));
                        }
                    }
                    else
                    {
                        object? NN_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
                        if (NN_attribute != null)
                        {
                            nullability = false;
                        }
                    }

                    object? UQ_attribute = attributes.FirstOrDefault(x => x.GetType() == UQType);

                    if (UQ_attribute != null)
                    {
                        if (UQType.GetProperty("UniqueGroupId")?.GetValue(UQ_attribute, null) is int groupId)
                        {
                            if (!nullability)
                            {
                                UniqueOptions.Add(AddUniqueConstraint(Property.Name, groupId));
                            }
                            else
                            {
                                throw ProtectDbAndThrow($"Property '{Property.Name}' of Entity '{TableType.Name}' is marked with UNIQUE CONSTRAINT and it Requires to be 'NOT NULL'.");
                            }
                        }
                    }
                }
            }

            if(Pkoption != string.Empty)
            {
                Pkoption = $"{Pkoption.Remove(0, 1)}, ";
            }

            string FkCommand = $"{alterTable}{Pkoption}{CreateForeignKeyConstraintLite(FkOptions,TableType.Name)}{CreateUniqueConstraintLite(UniqueOptions,TableType.Name)}";

            if (FkCommand.Length > 1)
            {
                FkCommand = $"{FkCommand[..(FkCommand.Length - 2)]}); ";
            }

            alterTable.Clear(); foreignKeys.Clear();
            closingCommand.Append(FkCommand);
            closingCommand.Append($"{TransferOldTableData(CommonColumns, Tablename, OldTablename)} DROP TABLE {OldTablename};");
            closingCommand.Append($"ALTER TABLE {Tablename} RENAME TO {OldTablename}; ALTER TABLE {OldTablename} RENAME TO {Tablename};");
            closingCommand.Append($" DROP INDEX IF EXISTS {OldTablename}");
            CustomTransaction.Add(closingCommand.ToString());
            CliConsoleLogs($"{closingCommand}");
            closingCommand.Clear();
        }

        void ForeignKeyAsignment(Type TableType)
        {
            string Tablename = TableType.Name;
            PropertyInfo[] Properties = TableType.GetProperties();
            List<FKInfo> fkInfos = new();
            Type FkType = typeof(ForeignKey);
            List<UniqueInfo> UniqueOptions = new();
            Type UQType = typeof(Unique);

            foreach (PropertyInfo Property in Properties)
            {
                object[] attributes = Property.GetCustomAttributes(true);
                if (attributes.Length > 0)
                {
                    object? FK_attribute = attributes.SingleOrDefault(x => x.GetType() == FkType);
                    bool nullability = true;

                    if (FK_attribute != null)
                    {
                        var tName = FkType.GetProperty("TableName")?.GetValue(FK_attribute, null);
                        var tColumn = FkType.GetProperty("Column")?.GetValue(FK_attribute, null);
                        if (FkType.GetProperty("Nullability")?.GetValue(FK_attribute, null) is bool isNullable)
                        {
                            nullability = isNullable;
                            fkInfos.Add(AddForeignKey(Property.Name, tName, tColumn, isNullable));
                        }
                    }
                    else
                    {
                        object? NN_attribute = attributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
                        if(NN_attribute != null)
                        {
                            nullability = false;
                        }
                    }

                    object? UQ_attribute = attributes.FirstOrDefault(x => x.GetType() == UQType);

                    if (UQ_attribute != null)
                    {
                        if (UQType.GetProperty("UniqueGroupId")?.GetValue(UQ_attribute, null) is int groupId)
                        {
                            if (!nullability)
                            {
                                UniqueOptions.Add(AddUniqueConstraint(Property.Name, groupId));
                            }
                            else
                            {
                                throw ProtectDbAndThrow($"Property '{Property.Name}' of Entity '{TableType.Name}' is marked with UNIQUE CONSTRAINT and it Requires to be 'NOT NULL'.");
                            }
                        }
                    }
                }
            }

            if (fkInfos.Any())
            {
                AfterMath.AddRange(CreateForeignKeyConstraint(fkInfos, TableType.Name, $" ALTER TABLE {TableSchema}{MyShit(Tablename)}"));
            }

            if (UniqueOptions.Any())
            {
                AfterMath.AddRange(CreateUniqueConstraint(UniqueOptions, TableType.Name, $"ALTER TABLE {TableSchema}{MyShit(TableType.Name)}"));
            }
        }

        void UpdateTableSchema(Type TableType)
        {
            string getColumns = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableType.Name}' {TableSchemaCheck}";
            List<FKInfo> FkOptions = new();
            List<UniqueInfo> UniqueOptions = new();

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.Oracle)
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

            if (IsForcedUpdate)
            {
                List<string> CommonColumns = NewColumnNames.Intersect(ColumnNames).ToList();
                UpdateTableColumnsNullability(CommonColumns.Where(x=> x != "Inactive").ToList(), Properties, TableType.Name);
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
                    columnCreator.Append(GetDatatypeCommand(typeof(int), Array.Empty<object>(), ColumnName, TableType.Name));
                    CustomTransaction.Add(columnCreator.ToString() + " NULL");
                    CliConsoleLogs($"{columnCreator} NULL;");

                    if (IsTrashOracleProduct)
                    {
                        RevertShittyOracleTransaction.Add($"ALTER TABLE {MyShit(TableType.Name)} DROP COLUMN {MyShit("Inactive")}");
                    }
                }
                else
                {
                    PropertyInfo Property = Properties.First(x => x.Name == ColumnName);
                    object[]? attributes = Property.GetCustomAttributes(true);
                    columnCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, ColumnName, TableType.Name));
                    columnCreator.Append(AddColumnConstaints(attributes, TableType.Name, Property.Name, Property.PropertyType, false, FkOptions, UniqueOptions));
                    CustomTransaction.Add(columnCreator.ToString());
                    CliConsoleLogs($"{columnCreator};");

                    if (IsTrashOracleProduct)
                    {
                        RevertShittyOracleTransaction.Add($"ALTER TABLE {MyShit(TableType.Name)} DROP COLUMN {MyShit(Property.Name)}");
                    }
                }
                columnCreator.Clear();
            }

            if (FkOptions.Any())
            {
                AfterMath.AddRange(CreateForeignKeyConstraint(FkOptions, TableType.Name, $"ALTER TABLE {TableSchema}{MyShit(TableType.Name)}"));
            }

            if (UniqueOptions.Any())
            {
                AfterMath.AddRange(CreateUniqueConstraint(UniqueOptions, TableType.Name, $"ALTER TABLE {TableSchema}{MyShit(TableType.Name)}"));
            }

            if (inactiveColMissing)
            {
                string updateInactiveCol = $"Update {TableSchema}{Tablename} set {MyShit("Inactive")} = 0 where {MyShit("Inactive")} is null";
                AfterMath.Add(updateInactiveCol);
                CliConsoleLogs($"{updateInactiveCol};");
            }
        }

        void UpdateOpenTableSchema(Type TableType)
        {
            PKInfo pkInformation = ReadOpenEntityPrimaryKeys(TableType);
            List<string> existingPKs = dbInfoReader.GetExistingPrimaryKeys(TableType.Name);
            bool canUpdatePKs = CompairPrimaryKeys(existingPKs, pkInformation.PKPropertyNames, TableType.Name);
            List<FKInfo> FkOptions = new();
            List<UniqueInfo> UniqueOptions = new();

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

            List<string> ColumnNames = connection.Query<string>(getColumns, null);

            bool PKsDropped = false;

            if (IsForcedUpdate)
            {
                List<string> CommonColumns = NewColumnNames.Intersect(ColumnNames).ToList();
                UpdateTableColumnsNullability(CommonColumns, Properties, TableType.Name);
                if (canUpdatePKs)
                {
                    PKsDropped = true;
                    string commandText = $"ALTER TABLE {TableSchema}{Tablename} DROP CONSTRAINT IF EXISTS PK_{TableType.Name}";
                    CustomTransaction.Add(commandText);
                    CliConsoleLogs($"{commandText};");

                    if (IsTrashOracleProduct)
                    {
                        string primaryKeys = string.Empty;
                        foreach (string pkName in existingPKs)
                        {
                            primaryKeys += $",{MyShit(pkName)}";
                        }
                        string commandTxt = $"ALTER TABLE {TableSchema}{Tablename} ADD CONSTRAINT PK_{TableType.Name} PRIMARY KEY ({primaryKeys.Remove(0, 1)})";
                        RevertShittyOracleTransaction.Add(commandTxt);
                    }
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
                columnCreator.Append(GetDatatypeCommand(Property.PropertyType, attributes, ColumnName, TableType.Name));
                columnCreator.Append(AddColumnConstaints(attributes, TableType.Name, Property.Name, Property.PropertyType, pkInformation.PKPropertyNames.Contains(ColumnName),FkOptions, UniqueOptions));
                CustomTransaction.Add(columnCreator.ToString());
                columnCreator.Clear();

                if (IsTrashOracleProduct)
                {
                    RevertShittyOracleTransaction.Add($"ALTER TABLE {MyShit(TableType.Name)} DROP COLUMN {MyShit(Property.Name)}");
                }
            }

            if (FkOptions.Any())
            {
                AfterMath.AddRange(CreateForeignKeyConstraint(FkOptions, TableType.Name, $"ALTER TABLE {TableSchema}{MyShit(TableType.Name)}"));
            }

            if (UniqueOptions.Any())
            {
                AfterMath.AddRange(CreateUniqueConstraint(UniqueOptions, TableType.Name, $"ALTER TABLE {TableSchema}{MyShit(TableType.Name)}"));
            }

            if (pkInformation.PKPropertyNames.Any() && PKsDropped)
            {
                string primaryKeys = string.Empty;
                foreach(string pkName in pkInformation.PKPropertyNames)
                {
                    primaryKeys += $",{MyShit(pkName)}";
                }
                string commandTxt = $"ALTER TABLE {TableSchema}{Tablename} ADD CONSTRAINT PK_{TableType.Name} PRIMARY KEY ({primaryKeys.Remove(0, 1)})";
                CustomTransaction.Add(commandTxt);
                CliConsoleLogs($"{commandTxt};");

                if (IsTrashOracleProduct)
                {
                    RevertShittyOracleTransaction.Add($"ALTER TABLE {TableSchema}{Tablename} DROP CONSTRAINT IF EXISTS PK_{TableType.Name}");
                }
            }
        }

        private bool CompairPrimaryKeys(List<string> existingPKs, List<string> newPKs, string TableName)
        {
            List<string> PKsToAdd = newPKs.Except(existingPKs).ToList();
            List<string> PKsToDrop = existingPKs.Except(newPKs).ToList();

            bool result = PKsToAdd.Any() || PKsToDrop.Any();

            if (IsForcedUpdate)
            {
                return result;
            }

            if (result)
            {
                throw ProtectDbAndThrow($"Error at Table '{TableName}' on Primary Keys Configuration. You CAN ONLY change the PRIMARY KEYS of a Table in Developer Mode, or by using the CLI 'update' command with the '--force' argument => 'bhl update --force'");
            }

            return result;
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
            bool mandatoryNull = false;

            object[] bhAttributes = newColumn.GetCustomAttributes(true);

            if (newColumn.PropertyType.Name.Contains("Nullable"))
            {
                if (newColumn.PropertyType.GenericTypeArguments != null && newColumn.PropertyType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            object? fkAttribute = bhAttributes.FirstOrDefault(x => x.GetType() == typeof(ForeignKey));
            if(fkAttribute != null)
            {
                object? nullability = typeof(ForeignKey).GetProperty("Nullability")?.GetValue(fkAttribute, null);
                if(nullability is bool nullableCol)
                {
                    isNullable = nullableCol;
                }

                if (mandatoryNull && !isNullable)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{newColumn.Name}' of Entity '{TableName}' CAN NOT become a NOT NULL column in the Database." +
                    $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove the (?) from the Property's Type.");
                }
            }

            object? notNullableAtr = bhAttributes.FirstOrDefault(x => x.GetType() == typeof(NotNullable));
            if (notNullableAtr != null)
            {
                isNullable = false;

                if (mandatoryNull)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{newColumn.Name}' of Entity '{TableName}' CAN NOT become a NOT NULL column in the Database." +
                    $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove the (?) from the Property's Type.");
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
            string defaultValCommand = GetDefaultValue(PropType, PropName, TableName);
            string updateTxt = $"Update {TableSchema}{MyShit(TableName)} set {MyShit(PropName)} = {defaultValCommand} where {MyShit(PropName)} is null";
            string alterTxt = $"ALTER TABLE {TableSchema}{MyShit(TableName)} {AlterColumn} {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} NOT NULL";

            CustomTransaction.Add(updateTxt);
            CustomTransaction.Add(alterTxt);
            CliConsoleLogs($"{updateTxt};");
            CliConsoleLogs($"{alterTxt};");

            if (IsTrashOracleProduct)
            {
                RevertShittyOracleTransaction.Add($"ALTER TABLE {TableSchema}{MyShit(TableName)} {AlterColumn} {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} NULL");
                RevertShittyOracleTransaction.Add(string.Empty);
            }
        }

        private void SetColumnToNull(string TableName, string PropName, TableParsingInfo ColumnInfo)
        {
            string alterTxt = $"ALTER TABLE {TableSchema}{MyShit(TableName)} {AlterColumn} {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} NULL";
            CustomTransaction.Add(alterTxt);
            CliConsoleLogs($"{alterTxt};");

            if (IsTrashOracleProduct)
            {
                RevertShittyOracleTransaction.Add($"ALTER TABLE {TableSchema}{MyShit(TableName)} {AlterColumn} {MyShit(PropName)} {GetSqlDataType(ColumnInfo)} NOT NULL");
            }
        }

        private string GetSqlDataType(TableParsingInfo columnInfo)
        {
            string DataType = columnInfo.DataType;

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.SqlServer)
            {
                columnInfo.MaxLength = columnInfo.MaxLength / 2;
            }

            if (DatabaseStatics.DatabaseType == BlackHoleSqlTypes.Oracle)
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
                        string dropConstraint = $"ALTER TABLE {TableSchema}{MyShit(referenced.TableName)} DROP CONSTRAINT IF EXISTS {referenced.ConstraintName}";
                        DropTransaction.Add(dropConstraint);
                        CliConsoleLogs($"{dropConstraint};");
                    }

                    TableParsingInfo? thisColumn = DbConstraints.FirstOrDefault(x =>
                    x.TableName.ToLower() == lowerTbName
                    && x.ColumnName.ToLower() == ColumnName.ToLower() && !string.IsNullOrEmpty(x.ReferencedTable));

                    if(thisColumn != null)
                    {
                        string dropColConstraint = $"ALTER TABLE {TableSchema}{MyShit(TableName)} DROP CONSTRAINT IF EXISTS {thisColumn.ConstraintName}";
                        DropTransaction.Add(dropColConstraint);
                        CliConsoleLogs($"{dropColConstraint};");
                    }

                    string dropCommand = $"ALTER TABLE {TableSchema}{MyShit(TableName)} DROP COLUMN {MyShit(ColumnName)}";
                    DropTransaction.Add(dropCommand);
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

        string AddColumnConstaints(object[] attributes, string TableName, string PropName, Type PropType, bool isOpenPk, List<FKInfo> fKInfos, List<UniqueInfo> uqInfos)
        {
            if(isOpenPk && !IsForcedUpdate)
            {
                throw ProtectDbAndThrow($"Error at Entity '{TableName}' and Property '{PropName}'. You CAN ONLY change PRIMARY KEYS of a Table by using " +
                    "'DeveloperMode' or the 'update' CLI command with '--force' argument");
            }

            bool isNullable = true;
            bool mandatoryNull = false;
            bool isUnique = false;
            int uniqueGroupId = 0;
            string constraintsCommand = "NULL";

            if (PropType.Name.Contains("Nullable"))
            {
                if (PropType.GenericTypeArguments != null && PropType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            object? ucAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(Unique));

            if(ucAttribute != null)
            {
                if (mandatoryNull)
                {
                    throw ProtectDbAndThrow($"Nullable Property '{PropName}' of Entity '{TableName}' CAN NOT become a NOT NULL UNIQUE column in the Database." +
                        $"Please Remove the (?) from the Property's Type or Remove the '[Unique]' Attribute.");
                }

                isUnique = true;

                if (typeof(Unique).GetProperty("UniqueGroupId")?.GetValue(ucAttribute, null) is int groupId)
                {
                    uniqueGroupId = groupId;
                }
            }

            Type fkAttributeType = typeof(ForeignKey);
            object? fkAttribute = attributes.FirstOrDefault(x => x.GetType() == fkAttributeType);

            if (fkAttribute != null)
            {
                var tName = fkAttributeType.GetProperty("TableName")?.GetValue(fkAttribute, null);
                var tColumn = fkAttributeType.GetProperty("Column")?.GetValue(fkAttribute, null);

                if (fkAttributeType.GetProperty("Nullability")?.GetValue(fkAttribute, null) is bool Nullability)
                {
                    isNullable = Nullability;
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

                if(isUnique && isNullable)
                {
                    throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' is marked with UNIQUE CONSTRAINT and it CAN NOT be NULLABLE." +
                        $"Please change the Nullability on the '[ForeignKey]' Attribute or Remove Property from the Primary Keys.");
                }

                if (!isNullable)
                {
                    throw ProtectDbAndThrow("CAN NOT Add a 'NOT NULLABLE' Foreign Key on an Existing Table. Please Change the Nullability on the " +
                        $"'[ForeignKey]' Attribute on the Property '{PropName}' of the Entity '{TableName}'.");
                }

                fKInfos.Add(AddForeignKey(PropName,tName,tColumn,isNullable));
                return $"{constraintsCommand} ";
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

                string defaultValCommand = GetDefaultValue(PropType, PropName, TableName);

                if (IsForcedUpdate)
                {
                    AfterMath.Add($"Update {TableSchema}{MyShit(TableName)} set {MyShit(PropName)} = {defaultValCommand} where {MyShit(PropName)} is null");
                    AfterMath.Add($"ALTER TABLE {TableSchema}{MyShit(TableName)} {AlterColumn} {GetDatatypeCommand(PropType, attributes, PropName, TableName)} NOT NULL");
                    if (IsTrashOracleProduct)
                    {
                        RevertShittyOracleAfterMath.Add($"ALTER TABLE {TableSchema}{MyShit(TableName)} {AlterColumn} {GetDatatypeCommand(PropType, attributes, PropName, TableName)} NULL");
                        RevertShittyOracleAfterMath.Add(string.Empty);

                    }
                    if (isUnique)
                    {
                        uqInfos.Add(AddUniqueConstraint(PropName,uniqueGroupId));
                    }
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

            if (isUnique)
            {
                throw ProtectDbAndThrow($"Property '{PropName}' of Entity '{TableName}' is marked with UNIQUE CONSTRAINT and it Requires the '[NotNullable]' Attribute.");
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

        private string SQLiteColumn(object[] attributes, bool firstTime, Type PropertyType, bool isOpenPk, string PropName, string TableName, bool wasNotNull, bool hasPkChanged)
        {
            if (hasPkChanged && !firstTime && !IsForcedUpdate)
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
                    string defaultValCommand = GetDefaultValue(PropertyType, PropName, TableName);

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

        bool HasLitePkChanged(int pk, bool isOpenPk)
        {
            bool wasPk = pk > 0;
            if(wasPk != isOpenPk)
            {
                return true;
            }
            return false;
        }

        FKInfo AddForeignKey(string propName, object? tName, object? tColumn, bool nullability)
        {
            return new FKInfo
            {
                PropertyName = propName,
                ReferencedTable = $"{tName}",
                ReferencedColumn = $"{tColumn}",
                IsNullable = nullability
            };
        }

        UniqueInfo AddUniqueConstraint(string propName, int groupId)
        {
            return new UniqueInfo
            {
                PropertyName = propName,
                GroupId = groupId
            };
        }

        List<string> CreateForeignKeyConstraint(List<FKInfo> tableFKs, string TableName, string AlterCommand)
        {
            List<string> result = new();
            foreach(string referencedTable in tableFKs.Select(x => x.ReferencedTable).Distinct())
            {
                result.Add($"{CreateFkConstraint(tableFKs.Where(x => x.ReferencedTable == referencedTable).ToList(), TableName, referencedTable, AlterCommand)}");
                if (IsTrashOracleProduct)
                {
                    RevertShittyOracleAfterMath.Add($"{AlterCommand} DROP CONSTRAINT fk_{TableName}_{referencedTable}");
                }
            }
            return result;
        }

        string CreateForeignKeyConstraintLite(List<FKInfo> tableFKs, string TableName)
        {
            string result = string.Empty;
            foreach (string referencedTable in tableFKs.Select(x => x.ReferencedTable).Distinct())
            {
                result += $"{CreateFkConstraint(tableFKs.Where(x => x.ReferencedTable == referencedTable).ToList(), TableName, referencedTable, string.Empty)}, ";
            }
            return result;
        }

        List<string> CreateUniqueConstraint(List<UniqueInfo> tableUQs, string TableName, string AlterCommand)
        {
            List<string> result = new();
            foreach (int columnsGroup in tableUQs.Select(x => x.GroupId).Distinct())
            {
                result.Add($"{CreateUQConstraint(tableUQs.Where(x => x.GroupId == columnsGroup).ToList(), TableName, AlterCommand)}");
                if (IsTrashOracleProduct)
                {
                    RevertShittyOracleAfterMath.Add($"{AlterCommand} DROP CONSTRAINT uc_{TableName}_{columnsGroup}");
                }
            }
            return result;
        }

        string CreateUniqueConstraintLite(List<UniqueInfo> tableUQs, string TableName)
        {
            string result = string.Empty;
            foreach (int columnsGroup in tableUQs.Select(x => x.GroupId).Distinct())
            {
                result += $"{CreateUQConstraint(tableUQs.Where(x => x.GroupId == columnsGroup).ToList(), TableName, string.Empty)}, ";
            }
            return result;
        }

        string CreateUQConstraint(List<UniqueInfo> groupUQ, string TableName, string AlterTable)
        {
            string constraintBegin = "ADD CONSTRAINT";

            if (IsLite)
            {
                constraintBegin = "CONSTRAINT";
            }
            int groupId = 0;
            string uniqueColumns = string.Empty;

            foreach(UniqueInfo Uq in groupUQ)
            {
                uniqueColumns += $",{MyShit(Uq.PropertyName)}";
                groupId = Uq.GroupId;
            }

            uniqueColumns = uniqueColumns.Remove(0, 1);
            string uniqueConstraint = $"{AlterTable} {constraintBegin} uc_{TableName}_{groupId} UNIQUE ({uniqueColumns})";
            CliConsoleLogs($"{uniqueConstraint};");
            return uniqueConstraint;
        }

        string CreateFkConstraint(List<FKInfo> commonFKs, string TableName,string ReferencedTable, string AlterCommand)
        {
            string constraintBegin = "ADD CONSTRAINT";

            if (IsLite)
            {
                constraintBegin = "CONSTRAINT";
            }

            string fromColumn = string.Empty;
            string toColumn = string.Empty;
            bool isNullable = true;

            foreach(FKInfo fk in commonFKs)
            {
                if (!fk.IsNullable)
                {
                    isNullable = false;
                }

                fromColumn += $",{MyShit(fk.PropertyName)}";
                toColumn += $",{MyShit(fk.ReferencedColumn)}";
            }

            fromColumn = fromColumn.Remove(0, 1);
            toColumn = toColumn.Remove(0, 1);
            string onDeleteRule = isNullable ? "on delete set null" : "on delete cascade";
            string constraintCommand = $"{AlterCommand} {constraintBegin} fk_{TableName}_{ReferencedTable} FOREIGN KEY ({fromColumn}) REFERENCES {TableSchema}{MyShit(ReferencedTable)}({toColumn}) {onDeleteRule}";
            CliConsoleLogs($"{constraintCommand};");
            return constraintCommand;
        }

        string CheckLitePreviousColumnState(string defaultVal, string nullPhase, string TableName, string ColumnName)
        {
            TableParsingInfo? oldColumn = DbConstraints.FirstOrDefault(x => x.TableName.ToLower() == TableName.ToLower() && x.ColumnName.ToLower() == ColumnName.ToLower());
            if (oldColumn != null && !oldColumn.Nullable)
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

            if (CliCommand.ExportSql)
            {
                SqlWriter.CreateSqlFile();
            }
        } 

        string GetDatatypeCommand(Type PropertyType, object[] attributes, string Propertyname, string TableName)
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
                    if(CharLength == null && ForeignKeyAtt != null)
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
                   throw ProtectDbAndThrow($"Unsupported property type '{PropertyType.FullName}' at Property '{Propertyname}' of Entity '{TableName}'");
            }
            return dataCommand;
        }

        string GetDefaultValue(Type PropertyType, string PropName, string TableName)
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
                "DateTime" => $"'{new DateTime(1970,1,1).ToString(DatabaseStatics.DbDateFormat)}'",
                "Byte[]" => $"'{new byte[0]}'",
                _ => throw ProtectDbAndThrow($"Unsupported property type '{PropertyType.FullName}' at Property '{PropName}' of Entity '{TableName}'"),
            };
        }

        internal void ClearCommands()
        {
            CreateTablesTransaction.Clear();
            CustomTransaction.Clear();
            AfterMath.Clear();
            DropTransaction.Clear();
            RevertShittyOracleTables.Clear();
            RevertShittyOracleTransaction.Clear();
            RevertShittyOracleAfterMath.Clear();
        }

        internal bool ExecuteTableCreation()
        {
            List<string> AllCommands = new();
            AllCommands.AddRange(CreateTablesTransaction);
            AllCommands.AddRange(CustomTransaction);
            AllCommands.AddRange(AfterMath);
            AllCommands.AddRange(DropTransaction);

            if (AllCommands.Any())
            {
                if (IsLite)
                {
                    BlackHoleTransaction transaction = new();
                    connection.JustExecute("PRAGMA foreign_keys = off", null, transaction);
                    foreach (string command in AllCommands)
                    {
                        connection.JustExecute(command, null, transaction);
                    }
                    connection.JustExecute("PRAGMA foreign_keys = on", null, transaction);
                    bool result = transaction.Commit();
                    transaction.Dispose();
                    return result;
                }

                if (IsTrashOracleProduct)
                {
                    List<string> UpdateCommands = new();
                    UpdateCommands.AddRange(CreateTablesTransaction);
                    UpdateCommands.AddRange(CustomTransaction);
                    UpdateCommands.AddRange(AfterMath);

                    List<string> RevertChanges = new();
                    RevertChanges.AddRange(RevertShittyOracleTables);
                    RevertChanges.AddRange(RevertShittyOracleTransaction);
                    RevertChanges.AddRange(RevertShittyOracleAfterMath);

                    BlackHoleTransaction transaction = new();
                    int failedIndex = 0;

                    for (int i = 0; i < UpdateCommands.Count; i++)
                    {
                        bool success = connection.JustExecute(AllCommands[i], null, transaction);
                        if (!success)
                        {
                            failedIndex = i;
                            transaction.DoNotCommit();
                            break;
                        }
                    }
                    UpdateCommands.Clear();
                    bool result = false;

                    if(failedIndex > 0)
                    {
                        for(int i = 1; i < failedIndex + 1; i++)
                        {
                            connection.JustExecute(RevertChanges[failedIndex - i], null);
                        }
                    }
                    else
                    {
                        foreach(string command in DropTransaction)
                        {
                            connection.JustExecute(command, null, transaction);
                        }
                        result = transaction.Commit();
                    }
                    RevertChanges.Clear();
                    transaction.Dispose();
                    return result;
                }

                string[] safeTransaction = _multiDatabaseSelector.GetSafeTransactionTry();
                StringBuilder KickInTheTeeth = new();
                KickInTheTeeth.Append(safeTransaction[0]);

                foreach (string command in AllCommands)
                {
                    KickInTheTeeth.Append($"{command};");
                }

                KickInTheTeeth.Append(safeTransaction[1]);
                return connection.JustExecute(KickInTheTeeth.ToString(), null);
            }
            return true;
        }
    }
}
