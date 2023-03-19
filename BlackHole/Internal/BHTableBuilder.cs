using System.Data;
using System.Reflection;
using BlackHole.Entities;
using BlackHole.Logger;
using Dapper;

namespace BlackHole.Internal
{
    internal class BHTableBuilder : IBHTableBuilder
    {
        private IBHDatabaseSelector _multiDatabaseSelector;
        private ILoggerService _loggerService;
        private string[] SqlDatatypes;
        private bool isMyShit;
        private bool isLite;

        internal BHTableBuilder()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            _loggerService = new LoggerService();
            SqlDatatypes = _multiDatabaseSelector.SqlDatatypesTranslation();
            isMyShit = _multiDatabaseSelector.GetMyShit();
            isLite = _multiDatabaseSelector.IsLite();
        }

        /// <summary>
        /// Build a Table using a List of BlazarEntity or BlazarEntityWithActivator Types. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableTypes"></param>
        void IBHTableBuilder.BuildMultipleTables(List<Type> TableTypes)
        {
            bool[] Builded = new bool[TableTypes.Count];

            for (int i = 0; i < Builded.Length; i++)
            {
                Builded[i] = CreateTable(TableTypes[i]);
            }

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
        }

        /// <summary>
        /// Build a Table using a BlazarEntity or BlazarEntityWithActivator Type. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableType"></param>
        void IBHTableBuilder.BuildTable(Type TableType)
        {
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
            bool success = false;

            using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
            {
                try
                {
                    string tableCheck = $@"select case when exists((select * from information_schema.tables where table_name = '" + Tablename + "')) then 1 else 0 end";

                    if (isLite)
                    {
                        tableCheck = $@"SELECT name FROM sqlite_master WHERE type='table' AND name='" + Tablename + "'";
                        tExists = connection.ExecuteScalar<string>(tableCheck) == Tablename;
                    }
                    else
                    {
                        tExists = connection.ExecuteScalar<int>(tableCheck) == 1;
                    }

                    if (!tExists)
                    {
                        PropertyInfo[] Properties = TableType.GetProperties();
                        string creationCommand = $"CREATE TABLE {MyShit(Tablename)} (";

                        creationCommand += GetDatatypeCommand("Int32", new object[0], "Inactive");
                        creationCommand += GetSqlColumn(new object[0], true);

                        foreach (PropertyInfo Property in Properties)
                        {
                            string propertyType = Property.PropertyType.Name;
                            object[] attributes = Property.GetCustomAttributes(true);

                            if (Property.Name != "Id")
                            {
                                creationCommand += GetDatatypeCommand(propertyType, attributes, Property.Name);

                                creationCommand += GetSqlColumn(attributes, true);
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

                        creationCommand = creationCommand.Substring(0, creationCommand.Length - 2) + ");";
                        connection.Execute(creationCommand);
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    _loggerService.CreateErrorLogs($"TableBuilder_{Tablename}", ex.Message, ex.ToString());
                }
            }

            return success;
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

            using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
            {
                string Tablename = MyShit(TableType.Name);

                List<LiteTableInfo> ColumnsInfo = new List<LiteTableInfo>();
                List<string> ColumnNames = new List<string>();
                List<string> NewColumnNames = new List<string>();
                NewColumnNames.Add("Inactive");

                PropertyInfo[] Properties = TableType.GetProperties();

                foreach (PropertyInfo Property in Properties)
                {
                    NewColumnNames.Add(Property.Name);
                }

                string getColumns = $"PRAGMA table_info({Tablename}); ";
                ColumnsInfo = connection.Query<LiteTableInfo>(getColumns).ToList();

                foreach (LiteTableInfo column in ColumnsInfo)
                {
                    ColumnNames.Add(column.name);
                }

                IEnumerable<string> CommonList = new List<string>();
                CommonList = ColumnNames.Intersect(NewColumnNames);

                if (CommonList.Count() != ColumnNames.Count)
                {
                    updateSchema = true;
                }
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

            using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
            {
                List<LiteTableInfo> ColumnsInfo = new List<LiteTableInfo>();
                List<string> ColumnNames = new List<string>();
                List<string> NewColumnNames = new List<string>();

                ColumnsInfo = connection.Query<LiteTableInfo>(getColumns).ToList();

                foreach (LiteTableInfo column in ColumnsInfo)
                {
                    ColumnNames.Add(column.name);
                }

                alterTable += GetDatatypeCommand("Int32", new object[0], "Inactive");
                alterTable += GetSqlColumn(new object[0], true);

                foreach (PropertyInfo Property in Properties)
                {
                    string propertyType = Property.PropertyType.Name;
                    object[] attributes = Property.GetCustomAttributes(true);
                    NewColumnNames.Add(Property.Name);

                    if (Property.Name != "Id")
                    {
                        alterTable += GetDatatypeCommand(propertyType, attributes, Property.Name);

                        alterTable += GetSqlColumn(attributes, firstTime);
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

                string trasferData = TransferOldTableData(ColumnNames, NewColumnNames, $"{Tablename}", $"{Tablename}_Old");
                closingCommand = $"{alterTable}{foreignKeys}";
                closingCommand = closingCommand.Substring(0, closingCommand.Length - 2);
                closingCommand = $"{closingCommand});{trasferData} DROP TABLE {Tablename}_Old;";
                closingCommand += $"ALTER TABLE {Tablename} RENAME TO {Tablename}_Old; ALTER TABLE {Tablename}_Old RENAME TO {Tablename};";
                closingCommand += $"PRAGMA foreign_keys=on; DROP INDEX IF EXISTS {Tablename}_Old;";

                try
                {
                    connection.Execute(closingCommand);
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"Constraints_{Tablename}", ex.Message, ex.ToString());
                }
            }
        }

        void ForeignKeyAsignment(Type TableType)
        {
            string Tablename = TableType.Name;
            PropertyInfo[] Properties = TableType.GetProperties();
            string alterTable = $" ALTER TABLE {MyShit(Tablename)}";

            using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
            {
                foreach (PropertyInfo Property in Properties)
                {
                    object[] attributes = Property.GetCustomAttributes(true);

                    if (attributes.Length > 0)
                    {
                        object? FK_attribute = attributes.SingleOrDefault(x => x.GetType() == typeof(ForeignKey));

                        if (FK_attribute != null)
                        {
                            try
                            {
                                var tName = FK_attribute.GetType().GetProperty("TableName")?.GetValue(FK_attribute, null);
                                var tColumn = FK_attribute.GetType().GetProperty("Column")?.GetValue(FK_attribute, null);
                                var cascadeInfo = FK_attribute.GetType().GetProperty("CascadeInfo")?.GetValue(FK_attribute, null);
                                string alterColumn = MyShitConstraint(alterTable, Tablename, Property.Name, tName, tColumn, cascadeInfo);
                                connection.Execute(alterColumn);
                            }
                            catch (Exception ex)
                            {
                                _loggerService.CreateErrorLogs($"Constraints_{Tablename}", ex.Message, ex.ToString());
                            }
                        }
                    }
                }
            }
        }

        void UpdateTableSchema(Type TableType)
        {
            if (TableType.BaseType == typeof(BlackHoleEntity))
            {
                string getColumns = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableType.Name}';";
                PropertyInfo[] Properties = TableType.GetProperties();
                string Tablename = MyShit(TableType.Name);
                List<string> NewColumnNames = new List<string>();

                foreach (PropertyInfo Property in Properties)
                {
                    NewColumnNames.Add(Property.Name);
                }

                NewColumnNames.Add("Inactive");

                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<string> ColumnNames = new List<string>();
                    ColumnNames = connection.Query<string>(getColumns).ToList();

                    List<string> ColumnsToAdd = NewColumnNames.Except(ColumnNames).ToList();
                    List<string> ColumnsToDrop = ColumnNames.Except(NewColumnNames).ToList();

                    foreach (string ColumnName in ColumnsToDrop)
                    {
                        string dropCommand = $"ALTER TABLE {Tablename} DROP COLUMN {MyShit(ColumnName)} ";

                        try
                        {
                            connection.Execute(dropCommand);
                        }
                        catch (Exception ex)
                        {
                            _loggerService.CreateErrorLogs($"ColumnDrop_{Tablename}", ex.Message, ex.ToString());
                        }
                    }

                    foreach (string ColumnName in ColumnsToAdd)
                    {
                        string addCommand = $"ALTER TABLE {Tablename} ADD ";
                        var Property = Properties.Where(x => x.Name == ColumnName).FirstOrDefault();

                        if (Property != null)
                        {
                            string PropName = Property.Name;
                            string propertyType = Property.PropertyType.Name;

                            object[] attributes = Property.GetCustomAttributes(true);
                            addCommand += GetDatatypeCommand(propertyType, attributes, ColumnName);
                            addCommand += AddColumnConstaints(attributes, Tablename, PropName, propertyType);

                            try
                            {
                                connection.Execute(addCommand);
                            }
                            catch (Exception ex)
                            {
                                _loggerService.CreateErrorLogs($"ColumnAdd_{Tablename}", ex.Message, ex.ToString());
                            }
                        }

                        if (ColumnName == "Inactive")
                        {
                            addCommand += GetDatatypeCommand("Int32", new object[0], ColumnName);
                            try
                            {
                                connection.Execute(addCommand);
                            }
                            catch (Exception ex)
                            {
                                _loggerService.CreateErrorLogs($"ColumnAdd_{Tablename}", ex.Message, ex.ToString());
                            }
                        }
                    }
                }
            }
        }

        string AddColumnConstaints(object[] attributes, string Tablename, string PropName, string PropType)
        {
            string constraintsCommand = "NULL;";
            string alterTable = $"ALTER TABLE {Tablename}";

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
                            constraintsCommand = $"NOT NULL DEFAULT '{GetDefaultValue(PropType)}';";
                            break;
                    }
                }
            }
            return constraintsCommand;
        }

        string GetSqlColumn(object[] attributes, bool firstTime)
        {
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
            string constraint = $"ADD CONSTRAINT fk_{Tablename}_{tName}";

            string result = $"{alterTable} {constraint} FOREIGN KEY ({propName}) REFERENCES {tName}({tColumn}) {cascadeInfo};";

            if (!isMyShit)
            {
                result = $@"{alterTable} {constraint} FOREIGN KEY (""{propName}"") REFERENCES ""{tName}""(""{tColumn}"") {cascadeInfo};";
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

        string GetDatatypeCommand(string PropertyType, object[] attributes, string Propertyname)
        {
            string dataCommand = "";

            switch (PropertyType)
            {
                case "String":
                    object? CharLength = attributes.Where(x => x.GetType().Name == "VarCharSize").FirstOrDefault();

                    if (CharLength != null)
                    {
                        var Lngth = CharLength.GetType().GetProperty("Charlength")?.GetValue(CharLength, null);
                        dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[0]}({Lngth?.ToString()}) ";
                    }
                    else
                    {
                        dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[0]}(255) ";
                    }
                    break;

                case "Int32":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[1]} ";
                    break;
                case "Int64":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[2]} ";
                    break;
                case "Decimal":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[3]} ";
                    break;
                case "Single":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[4]} ";
                    break;
                case "Double":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[5]} ";
                    break;
                case "Guid":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[6]} ";
                    break;
                case "Boolean":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[7]} ";
                    break;
                case "DateTime":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[8]} ";
                    break;
                case "Byte[]":
                    dataCommand = $"{MyShit(Propertyname)} {SqlDatatypes[9]} ";
                    break;
            }

            return dataCommand;
        }
    }
}
