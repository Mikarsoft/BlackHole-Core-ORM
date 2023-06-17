using System.Data;
using System.Reflection;
using BlackHole.CoreSupport;
using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHTableBuilder : IBHTableBuilder
    {
        private IBHDatabaseSelector _multiDatabaseSelector;
        private readonly IExecutionProvider connection;
        private BHSqlExportWriter sqlWriter { get; set; }

        private List<DataConstraints> AllConstraints { get; set; }
        private string[] SqlDatatypes;
        private bool isMyShit;
        private bool isLite;

        private string tableSchemaCheck { get; set; }
        private string tableSchema { get; set; }
        private string tableSchemaFk { get; set; }

        internal BHTableBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
            SqlDatatypes = _multiDatabaseSelector.SqlDatatypesTranslation();
            tableSchemaCheck = _multiDatabaseSelector.TableSchemaCheck();
            tableSchema = _multiDatabaseSelector.GetDatabaseSchema();
            tableSchemaFk = _multiDatabaseSelector.GetDatabaseSchemaFk();
            isMyShit = _multiDatabaseSelector.GetMyShit();
            isLite = _multiDatabaseSelector.IsLite();
            AllConstraints = GetConstraints();
            sqlWriter = new BHSqlExportWriter("2_TablesSql");
        }

        /// <summary>
        /// Build a Table using a List of BlazarEntity or BlazarEntityWithActivator Types. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableTypes"></param>
        void IBHTableBuilder.BuildMultipleTables(List<Type> TableTypes)
        {
            DatabaseStatics.InitializeData = true;

            bool[] Builded = new bool[TableTypes.Count];

            for (int i = 0; i < Builded.Length; i++)
            {
                Builded[i] = CreateTable(TableTypes[i]);
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

            if (CliCommand.ExportSql)
            {
                sqlWriter.CreateSqlFile();
            }
        }

        /// <summary>
        /// Build a Table using a BlazarEntity or BlazarEntityWithActivator Type. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableType"></param>
        void IBHTableBuilder.BuildTable(Type TableType)
        {
            DatabaseStatics.InitializeData = true;

            bool keepUp = CreateTable(TableType);

            if (keepUp)
            {
                AsignForeignKeys(TableType);
            }
            else
            {
                UpdateSchema(TableType);
            }
        }

        bool CreateTable(Type TableType)
        {
            string Tablename = TableType.Name;
            bool tExists = false;

            string tableCheck = "";

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlLite:
                    tableCheck = $@"SELECT name FROM sqlite_master WHERE type='table' AND name='" + Tablename + "'";
                    tExists = connection.ExecuteScalar<string>(tableCheck, null) == Tablename;
                    break;
                case BlackHoleSqlTypes.Oracle:
                    var dbName = _multiDatabaseSelector.GetDatabaseName();
                    tableCheck = $"SELECT table_name FROM all_tables WHERE owner ='{dbName}' and TABLE_NAME = '{Tablename}'";
                    tExists = connection.ExecuteScalar<string>(tableCheck, null) == Tablename;
                    break;
                default:
                    tableCheck = $"select case when exists((select * from information_schema.tables where table_name = '" + Tablename + $"' {tableSchemaCheck})) then 1 else 0 end";
                    tExists = connection.ExecuteScalar<int>(tableCheck, null) == 1;
                    break;
            }

            if (!tExists)
            {
                PropertyInfo[] Properties = TableType.GetProperties();
                string creationCommand = $"CREATE TABLE {tableSchema}{MyShit(Tablename)} (";

                creationCommand += GetDatatypeCommand(typeof(int), new object[0], "Inactive");
                creationCommand += GetSqlColumn(new object[0], true, typeof(int));

                foreach (PropertyInfo Property in Properties)
                {
                    object[] attributes = Property.GetCustomAttributes(true);

                    if (Property.Name != "Id")
                    {
                        creationCommand += GetDatatypeCommand(Property.PropertyType, attributes, Property.Name);

                        creationCommand += GetSqlColumn(attributes, true, Property.PropertyType);
                    }
                    else
                    {
                        if (TableType.BaseType == typeof(BlackHoleEntity<int>))
                        {
                            creationCommand += _multiDatabaseSelector.GetPrimaryKeyCommand();
                        }

                        if (TableType.BaseType == typeof(BlackHoleEntity<Guid>))
                        {
                            creationCommand += _multiDatabaseSelector.GetGuidPrimaryKeyCommand();
                        }

                        if (TableType.BaseType == typeof(BlackHoleEntity<string>))
                        {
                            creationCommand += _multiDatabaseSelector.GetStringPrimaryKeyCommand();
                        }
                    }
                }

                creationCommand = creationCommand.Substring(0, creationCommand.Length - 2) + ")";
                CliConsoleLogs($"{creationCommand};");
                return connection.JustExecute(creationCommand , null);
            }

            DatabaseStatics.InitializeData = false;
            return false;
        }

        void AsignForeignKeys(Type TableType)
        {
            if (isLite)
            {
                ForeignKeyLiteAsignment(TableType, true);
            }
            else
            {
                ForeignKeyAsignment(TableType);
            }
        }

        void UpdateSchema(Type TableType)
        {
            if (isLite)
            {
                UpdateLiteTableSchema(TableType);
            }
            else
            {
                UpdateTableSchema(TableType);
            }
        }

        void UpdateLiteTableSchema(Type TableType)
        {
            bool updateSchema = false;

            string Tablename = MyShit(TableType.Name);

            List<SqLiteTableInfo> ColumnsInfo = new List<SqLiteTableInfo>();
            List<string> ColumnNames = new List<string>();
            List<string> NewColumnNames = new List<string>();
            NewColumnNames.Add("Inactive");

            PropertyInfo[] Properties = TableType.GetProperties();

            foreach (PropertyInfo Property in Properties)
            {
                NewColumnNames.Add(Property.Name);
            }

            string getColumns = $"PRAGMA table_info({Tablename}); ";
            ColumnsInfo = connection.Query<SqLiteTableInfo>(getColumns, null);

            foreach (SqLiteTableInfo column in ColumnsInfo)
            {
                ColumnNames.Add(column.name);
            }

            IEnumerable<string> CommonList = new List<string>();
            CommonList = ColumnNames.Intersect(NewColumnNames);

            if (CommonList.Count() != ColumnNames.Count)
            {
                updateSchema = true;
            }

            if (updateSchema)
            {
                ForeignKeyLiteAsignment(TableType, false);
            }
        }

        void ForeignKeyLiteAsignment(Type TableType, bool firstTime)
        {
            string Tablename = MyShit(TableType.Name);
            PropertyInfo[] Properties = TableType.GetProperties();
            string getColumns = $"PRAGMA table_info({Tablename}); ";
            string alterTable = $"PRAGMA foreign_keys=off; ALTER TABLE {Tablename} RENAME TO {Tablename}_Old; CREATE TABLE {Tablename} (";
            string foreignKeys = "";
            string closingCommand = "";

            List<SqLiteTableInfo> ColumnsInfo = new List<SqLiteTableInfo>();
            List<string> ColumnNames = new List<string>();
            List<string> NewColumnNames = new List<string>();

            string getTableFk = $"PRAGMA foreign_key_list({Tablename});";
            List<SqLiteForeignKeySchema> SchemaInfo = new List<SqLiteForeignKeySchema>();
            SchemaInfo = connection.Query<SqLiteForeignKeySchema>(getTableFk, null);

            ColumnsInfo = connection.Query<SqLiteTableInfo>(getColumns, null).ToList();

            foreach (SqLiteTableInfo column in ColumnsInfo)
            {
                ColumnNames.Add(column.name);
            }

            alterTable += GetDatatypeCommand(typeof(int), new object[0], "Inactive");
            alterTable += GetSqlColumn(new object[0], true, typeof(int));
            NewColumnNames.Add("Inactive");

            foreach (PropertyInfo Property in Properties)
            {
                object[] attributes = Property.GetCustomAttributes(true);
                NewColumnNames.Add(Property.Name);

                if (Property.Name != "Id")
                {
                    alterTable += GetDatatypeCommand(Property.PropertyType, attributes, Property.Name);

                    alterTable += GetSqlColumn(attributes, firstTime, Property.PropertyType);
                }
                else
                {
                    if (TableType.BaseType == typeof(BlackHoleEntity<int>))
                    {
                        alterTable += _multiDatabaseSelector.GetPrimaryKeyCommand();
                    }

                    if (TableType.BaseType == typeof(BlackHoleEntity<Guid>))
                    {
                        alterTable += _multiDatabaseSelector.GetGuidPrimaryKeyCommand();
                    }

                    if (TableType.BaseType == typeof(BlackHoleEntity<string>))
                    {
                        alterTable += _multiDatabaseSelector.GetStringPrimaryKeyCommand();
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
                        foreignKeys += LiteConstraint(Tablename, Property.Name, tName, tColumn, cascadeInfo);
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

                        alterTable += $"{columnInfo.name} {columnInfo.type} NULL, ";

                        if(fkC != null)
                        {
                            foreignKeys += $"CONSTRAINT fk_{Tablename}_{fkC.table} FOREIGN KEY ({fkC.from}) REFERENCES {fkC.table}({fkC.to}) on delete {fkC.on_delete}, ";
                        }
                    }
                }
            }

            string trasferData = TransferOldTableData(ColumnNames, NewColumnNames, $"{Tablename}", $"{Tablename}_Old");
            closingCommand = $"{alterTable}{foreignKeys}";
            closingCommand = closingCommand.Substring(0, closingCommand.Length - 2);
            closingCommand = $"{closingCommand});{trasferData} DROP TABLE {Tablename}_Old;";
            closingCommand += $"ALTER TABLE {Tablename} RENAME TO {Tablename}_Old; ALTER TABLE {Tablename}_Old RENAME TO {Tablename};";
            closingCommand += $"PRAGMA foreign_keys=on; DROP INDEX IF EXISTS {Tablename}_Old;";

            connection.JustExecute(closingCommand, null);
            CliConsoleLogs($"{closingCommand}");
        }

        void ForeignKeyAsignment(Type TableType)
        {
            string Tablename = TableType.Name;
            PropertyInfo[] Properties = TableType.GetProperties();
            string alterTable = $" ALTER TABLE {tableSchema}{MyShit(Tablename)}";

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
            string getColumns = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableType.Name}' {tableSchemaCheck}";

            if(DatabaseStatics.DatabaseType == BlackHoleSqlTypes.Oracle)
            {
                string owner = _multiDatabaseSelector.GetDatabaseName();
                getColumns = $"SELECT Column_name From all_tab_cols where owner = '{owner}' and TABLE_NAME = '{TableType.Name}'";
            }

            PropertyInfo[] Properties = TableType.GetProperties();
            string Tablename = MyShit(TableType.Name);
            List<string> NewColumnNames = new List<string>();

            foreach (PropertyInfo Property in Properties)
            {
                NewColumnNames.Add(Property.Name);
            }

            NewColumnNames.Add("Inactive");

            List<string> ColumnNames = new List<string>();
            ColumnNames = connection.Query<string>(getColumns, null);

            List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
            List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();

            DropColumns(ColumnsToDrop, TableType.Name);

            foreach (string ColumnName in ColumnsToAdd)
            {
                string addCommand = $"ALTER TABLE {tableSchema}{Tablename} ADD ";
                var Property = Properties.Where(x => x.Name == ColumnName).FirstOrDefault();

                if (Property != null)
                {
                    string PropName = Property.Name;
                    string propertyType = Property.PropertyType.Name;

                    object[] attributes = Property.GetCustomAttributes(true);
                    addCommand += GetDatatypeCommand(Property.PropertyType, attributes, ColumnName);
                    addCommand += AddColumnConstaints(attributes, TableType.Name, PropName, propertyType);
                    string[] AllCommands = addCommand.Split("##");

                    foreach(string commandText in AllCommands)
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
                    addCommand += GetDatatypeCommand(typeof(int), new object[0], ColumnName);
                    connection.JustExecute(addCommand, null);
                    CliConsoleLogs($"{addCommand};");
                }
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
                        string dropConstraint = $"ALTER TABLE {tableSchema}{MyShit(TableName)} DROP CONSTRAINT {columnConstraint.CONSTRAINT_NAME}";
                        connection.JustExecute(dropConstraint, null);
                        CliConsoleLogs($"{dropConstraint};");
                    }

                    string dropCommand = $"ALTER TABLE {tableSchema}{MyShit(TableName)} DROP COLUMN {MyShit(ColumnName)}";
                    connection.JustExecute(dropCommand, null);
                    CliConsoleLogs($"{dropCommand};");
                }
            }
            else
            {
                string setColumnToNull = "";
                List<SqlTableInfo> TableInfo = new List<SqlTableInfo>();
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
                                            from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{TableName}' {tableSchemaCheck};";

                        TableInfo = connection.Query<SqlTableInfo>(setColumnToNull, null);

                        if (TableInfo.Count > 0)
                        {
                            foreach (string ColumnName in ColumnsToDrop)
                            {
                                SqlTableInfo? columnInfo = TableInfo.Where(x => x.column_name.ToLower() == ColumnName.ToLower()).FirstOrDefault();
                                if (columnInfo != null)
                                {
                                    string DataType = columnInfo.column_type;
                                    string setToNullable = $"ALTER TABLE {tableSchema}{MyShit(TableName)} MODIFY {MyShit(ColumnName)} {DataType} NULL ";
                                    connection.JustExecute(setToNullable, null);
                                    CliConsoleLogs($"{setToNullable};");
                                }
                            }
                        }

                        break;
                    case BlackHoleSqlTypes.Postgres:
                        setColumnToNull = @$" select table_name,column_name, ordinal_position, data_type, character_maximum_length, is_nullable, udt_name 
                                            from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{TableName}' {tableSchemaCheck};";

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

                                    string setToNullable = $"ALTER TABLE {tableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(ColumnName)} {DataType} NULL ";
                                    connection.JustExecute(setToNullable, null);
                                    CliConsoleLogs($"{setToNullable};");
                                }
                            }
                        }

                        break;
                    default:
                        setColumnToNull = @$" select table_name,column_name, ordinal_position, data_type, character_maximum_length, is_nullable 
                                            from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{TableName}' {tableSchemaCheck};";

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

                                    string setToNullable = $"ALTER TABLE {tableSchema}{MyShit(TableName)} ALTER COLUMN {MyShit(ColumnName)} {DataType} NULL ";
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
            List<DataConstraints> constraints = new List<DataConstraints>();

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    constraints = LoadMsSqlConstraints();
                    break;
                case BlackHoleSqlTypes.MySql:
                    constraints = LoadMySqlConstraints();
                    break;
                case BlackHoleSqlTypes.Postgres:
                    constraints = LoadPgConstraints();
                    break;
                case BlackHoleSqlTypes.Oracle:
                    constraints = LoadOracleConstraints();
                    break;
            }

            return constraints;
        }

        private List<DataConstraints> LoadMySqlConstraints()
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            string GetConstrainsCommand = @"SELECT K.TABLE_NAME, K.COLUMN_NAME, K.REFERENCED_TABLE_NAME, C.IS_NULLABLE as DELETE_RULE, K.CONSTRAINT_NAME FROM
                INFORMATION_SCHEMA.KEY_COLUMN_USAGE K
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON (C.COLUMN_NAME= K.COLUMN_NAME AND C.TABLE_NAME=K.TABLE_NAME)
                WHERE REFERENCED_TABLE_NAME is not null";
            Constraints = connection.Query<DataConstraints>(GetConstrainsCommand, null);
            return Constraints;
        }

        private List<DataConstraints> LoadMsSqlConstraints()
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            string GetConstrainsCommand = @"SELECT K.TABLE_NAME,K.COLUMN_NAME,REFERENCED_TABLE_NAME=TC.TABLE_NAME,T.DELETE_RULE,T.CONSTRAINT_NAME FROM
	            INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS T
	            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON TC.CONSTRAINT_NAME=T.UNIQUE_CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE K ON K.CONSTRAINT_NAME= T.CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON (C.COLUMN_NAME= K.COLUMN_NAME AND C.TABLE_NAME=K.TABLE_NAME)";
            Constraints = connection.Query<DataConstraints>(GetConstrainsCommand, null);
            return Constraints;
        }

        private List<DataConstraints> LoadOracleConstraints()
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            string GetConstrainsCommand = @"SELECT a.table_name, a.column_name, a.constraint_name, c_pk.table_name REFERENCED_TABLE_NAME, c.delete_rule
                FROM all_cons_columns a
                join all_constraints c ON a.owner = c.owner AND a.constraint_name = c.constraint_name
                JOIN all_constraints c_pk ON c.r_owner = c_pk.owner AND c.r_constraint_name = c_pk.constraint_name
                WHERE c.constraint_type = 'R' and a.owner =" + $"'{_multiDatabaseSelector.GetDatabaseName()}'";
            Constraints = connection.Query<DataConstraints>(GetConstrainsCommand, null);
            return Constraints;
        }

        private List<DataConstraints> LoadPgConstraints()
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            string GetConstrainsCommand = @"SELECT T.TABLE_NAME, K.COLUMN_NAME,CU.TABLE_NAME AS REFERENCED_TABLE_NAME,
                C.IS_NULLABLE AS DELETE_RULE, CU.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE K ON K.CONSTRAINT_NAME = T.CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME)
                INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CU ON CU.CONSTRAINT_NAME = T.CONSTRAINT_NAME
                WHERE T.CONSTRAINT_TYPE = 'FOREIGN KEY'";
            Constraints = connection.Query<DataConstraints>(GetConstrainsCommand, null);
            return Constraints;
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

        string AddColumnConstaints(object[] attributes, string Tablename, string PropName, string PropType)
        {
            string constraintsCommand = "NULL ##";
            string alterTable = $"ALTER TABLE {tableSchema}{MyShit(Tablename)}";

            if (attributes.Length > 0)
            {
                foreach (object Attribute in attributes)
                {
                    Type AttributeType = Attribute.GetType();

                    switch (AttributeType.Name)
                    {
                        case "ForeignKey":
                            var tName = Attribute.GetType().GetProperty("TableName")?.GetValue(Attribute, null);
                            var tColumn = Attribute.GetType().GetProperty("Column")?.GetValue(Attribute, null);
                            var tNullable = Attribute.GetType().GetProperty("IsNullable")?.GetValue(Attribute, null);

                            var cascadeInfo = "on delete cascade";

                            if (tNullable != null && tNullable.ToString() == "NULL")
                            {
                                cascadeInfo = "on delete set null";
                            }

                            constraintsCommand = $"{constraintsCommand} {MyShitConstraint(alterTable, Tablename, PropName, tName, tColumn, cascadeInfo)}";
                            break;
                        case "NotNullable":
                            constraintsCommand = $"DEFAULT '{GetDefaultValue(PropType)}' NOT NULL ";
                            break;
                    }
                }
            }
            return constraintsCommand;
        }

        string GetSqlColumn(object[] attributes, bool firstTime, Type PropertyType)
        {

            bool mandatoryNull = false;

            if (PropertyType.Name.Contains("Nullable"))
            {
                if (PropertyType.GenericTypeArguments != null && PropertyType.GenericTypeArguments.Length > 0)
                {
                    mandatoryNull = true;
                }
            }

            if (mandatoryNull)
            {
                return "NULL, ";
            }

            string constraintsCommand = ", ";

            if (attributes.Length > 0)
            {
                foreach (object Attribute in attributes)
                {
                    Type AttributeType = Attribute.GetType();

                    switch (AttributeType.Name)
                    {
                        case "ForeignKey":
                            var tNull = Attribute.GetType().GetProperty("IsNullable")?.GetValue(Attribute, null);
                            constraintsCommand = firstTime ? $"{tNull}, " : "NULL, ";
                            break;
                        case "NotNullable":
                            constraintsCommand = firstTime ? "NOT NULL, " : "NULL, ";
                            break;
                    }
                }
            }
            return constraintsCommand;
        }

        string MyShit(string propName)
        {
            string result = propName;

            if (!isMyShit)
            {
                result = $@"""{propName}""";
            }

            return result;
        }

        string MyShitConstraint(string alterTable, string Tablename, string propName, object? tName, object? tColumn, object? cascadeInfo)
        {
            string constraint = $"ADD CONSTRAINT fk_{Tablename}_{tName}{tableSchemaFk}";

            string result = $"{alterTable} {constraint} FOREIGN KEY ({propName}) REFERENCES {tableSchema}{tName}({tColumn}) {cascadeInfo}";

            if (!isMyShit)
            {
                result = $@"{alterTable} {constraint} FOREIGN KEY (""{propName}"") REFERENCES {tableSchema}""{tName}""(""{tColumn}"") {cascadeInfo}";
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
            IEnumerable<string> CommonList = new List<string>();
            CommonList = oldColumns.Intersect(newColumns);

            if (CommonList.Count() > 0)
            {
                result = $"INSERT INTO {newTablename} (";
                string selection = ") SELECT ";
                string selectionClosing = "";

                foreach (string column in CommonList)
                {
                    result += $"{column},";
                    selectionClosing += $"{column},";
                }
                result = $"{result.Substring(0, result.Length - 1)}{selection}{selectionClosing.Substring(0, selectionClosing.Length - 1)} FROM {oldTablename};";
            }

            return result;
        }

        string GetDefaultValue(string PropertyType)
        {
            string defaultValue = "";
            switch (PropertyType)
            {
                case "String":
                    defaultValue = "-";
                    break;
                case "Char":
                    defaultValue = "-";
                    break;
                case "Int16":
                    defaultValue = "0";
                    break;
                case "Int32":
                    defaultValue = "0";
                    break;
                case "Int64":
                    defaultValue = "0";
                    break;
                case "Decimal":
                    defaultValue = "0.0";
                    break;
                case "Single":
                    defaultValue = "0";
                    break;
                case "Double":
                    defaultValue = "0.0";
                    break;
                case "Guid":
                    defaultValue = Guid.Empty.ToString();
                    break;
                case "Boolean":
                    defaultValue = "false";
                    break;
                case "DateTime":
                    defaultValue = DateTime.Now.ToString();
                    break;
                case "Byte[]":
                    defaultValue = "0";
                    break;
            }
            return defaultValue;
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
                sqlWriter.AddSqlCommand(logCommand);
            }
        }

        void CliConsoleLogsNoSpace(string logCommand)
        {
            if (CliCommand.CliExecution)
            {
                Console.WriteLine($"_bhLog_{logCommand}");
            }
        }

        void IBHTableBuilder.UpdateWithoutForceWarning()
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
                    object? CharLength = attributes.Where(x => x.GetType().Name == "VarCharSize").FirstOrDefault();
                    object? ForeignKeyAtt = attributes.Where(x => x.GetType().Name == "ForeignKey").FirstOrDefault();

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
