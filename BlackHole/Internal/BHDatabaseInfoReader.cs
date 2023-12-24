using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHDatabaseInfoReader
    {
        private readonly IDataProvider connection;
        private readonly BHDatabaseSelector _multiDatabaseSelector;

        internal BHDatabaseInfoReader(IDataProvider _connection , BHDatabaseSelector databaseSelector)
        {
            connection = _connection;
            _multiDatabaseSelector = databaseSelector;
        }

        internal List<TableAspectsInfo> GetDatabaseInformation(int mode = 0)
        {
            List<TableAspectsInfo> tableAspects = new();
            List<TableParsingInfo> parsingData = GetDatabaseParsingInfo(mode);
           
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

        internal List<string> GetExistingPrimaryKeys(string TableName)
        {
            string pkSelectCommand = DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $@"SELECT ccu.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc 
                        JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
                        WHERE tc.TABLE_NAME = '{TableName}' AND tc.CONSTRAINT_TYPE = 'Primary Key' AND tc.TABLE_SCHEMA = '{GetSchema()}'",
                BlackHoleSqlTypes.MySql => $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_SCHEMA = '{GetSchema()}' AND TABLE_NAME = '{TableName}' AND COLUMN_KEY = 'PRI';",
                BlackHoleSqlTypes.Postgres => $@"select kc.column_name from information_schema.table_constraints tc
                        join information_schema.key_column_usage kc on kc.table_name = tc.table_name and kc.table_schema = tc.table_schema and kc.constraint_name = tc.constraint_name
                        where tc.constraint_type = 'PRIMARY KEY' and tc.table_schema ='{GetSchema()}' and tc.table_name = '{TableName}'",
                _ => $@"SELECT cols.column_name FROM all_constraints cons, all_cons_columns cols
                        WHERE cols.table_name = '{TableName}' AND cons.constraint_type = 'P'
                        AND cons.constraint_name = cols.constraint_name AND cons.owner = '{GetSchema()}'"
            };
            return connection.Query<string>(pkSelectCommand, null);
        }

        internal List<TableParsingInfo> GetDatabaseParsingInfo(int mode = 0)
        {
            string parseCommand = string.Empty;
            string schemaCheck = GetSchemaCheckCommand(mode);

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
                    return connection.Query<TableParsingInfo>(parseCommand, null);
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
                    return connection.Query<TableParsingInfo>(parseCommand, null);
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
                    return connection.Query<TableParsingInfo>(parseCommand, null);
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
                    return connection.Query<TableParsingInfo>(parseCommand, null);
                default:
                    return SqLiteParsing();
            }
        }

        internal List<TableParsingInfo> SqLiteParsing()
        {
            List<TableParsingInfo> parsingLiteData = new List<TableParsingInfo>();
            List<string> tableNames = connection.Query<string>("SELECT name FROM sqlite_master  where type = 'table' and name != 'sqlite_sequence';", null);
            foreach (string tableName in tableNames)
            {
                List<SqLiteTableInfo> tableInfo = connection.Query<SqLiteTableInfo>($"PRAGMA table_info({tableName});", null);
                List<SqLiteForeignKeySchema> foreignKeys = connection.Query<SqLiteForeignKeySchema>($"PRAGMA foreign_key_list({tableName});", null);
                LiteAutoIncrementInfo? isAutoIncrement = connection.QueryFirst<LiteAutoIncrementInfo>($"SELECT * FROM sqlite_sequence WHERE name='{tableName}'", null);
                foreach (SqLiteTableInfo info in tableInfo)
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

                    if (info.type.ToLower().Contains("bigint") && info.dflt_value.ToLower().Contains("last_insert_rowid()"))
                    {
                        parsingLine.Extra = "auto_increment";
                        parsingLine.PrimaryKey = true;
                    }

                    SqLiteForeignKeySchema? foreignKey = foreignKeys.Where(x => x.from == parsingLine.ColumnName).FirstOrDefault();
                    if (foreignKey != null)
                    {
                        parsingLine.DeleteRule = foreignKey.on_delete;
                        parsingLine.ReferencedTable = foreignKey.table;
                        parsingLine.ReferencedColumn = foreignKey.to;
                    }
                    parsingLiteData.Add(parsingLine);
                }
            }
            return parsingLiteData;
        }

        internal string GetSchema()
        {
            if (DatabaseStatics.DatabaseSchema != string.Empty)
            {
                return DatabaseStatics.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => DatabaseStatics.DatabaseSchema,
                    BlackHoleSqlTypes.MySql => _multiDatabaseSelector.GetDatabaseName(),
                    BlackHoleSqlTypes.Postgres => DatabaseStatics.DatabaseSchema,
                    _ => _multiDatabaseSelector.GetDatabaseName()
                };
            }
            else
            {
                return DatabaseStatics.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => "dbo",
                    BlackHoleSqlTypes.MySql => _multiDatabaseSelector.GetDatabaseName(),
                    BlackHoleSqlTypes.Postgres => "public",
                    _ => _multiDatabaseSelector.GetDatabaseName()
                };
            }
        }

        internal string GetSchemaCheckCommand(int mode)
        {
            string schemaCheck = string.Empty;

            if (DatabaseStatics.DatabaseSchema != string.Empty)
            {
                switch (DatabaseStatics.DatabaseType)
                {
                    case BlackHoleSqlTypes.SqlServer:
                        schemaCheck = $"WHERE SCHEMA_NAME(tb.schema_id) = '{DatabaseStatics.DatabaseSchema}'";
                        if (mode == 1) { schemaCheck += " and TC.TABLE_NAME is not null "; }
                        if (mode == 2) { schemaCheck += " and i.is_primary_key in not null "; }
                        break;
                    case BlackHoleSqlTypes.Postgres:
                        schemaCheck = $"where K.table_schema='{DatabaseStatics.DatabaseSchema}'";
                        if (mode == 1) { schemaCheck += " and T.constraint_type = 'FOREIGN KEY' "; }
                        if (mode == 2) { schemaCheck += " and T.constraint_type ='PRIMARY KEY' "; }
                        break;
                    case BlackHoleSqlTypes.MySql:
                        schemaCheck = _multiDatabaseSelector.GetDatabaseName();
                        if (mode == 1) { schemaCheck += " and C.REFERENCED_TABLE_NAME is not null "; }
                        if (mode == 2) { schemaCheck += " and K.COLUMN_KEY ='PRI' "; }
                        break;
                    case BlackHoleSqlTypes.Oracle:
                        schemaCheck = _multiDatabaseSelector.GetDatabaseName();
                        if (mode == 1) { schemaCheck += " and c.constraint_type = 'R' "; }
                        if (mode == 2) { schemaCheck += " and c.constraint_type = 'P' "; }
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
                        if (mode == 1) { schemaCheck += " and TC.TABLE_NAME is not null "; }
                        if (mode == 2) { schemaCheck += " and i.is_primary_key in not null "; }
                        break;
                    case BlackHoleSqlTypes.Postgres:
                        schemaCheck = $"where K.table_schema = 'public'";
                        if (mode == 1) { schemaCheck += " and T.constraint_type = 'FOREIGN KEY' "; }
                        if (mode == 2) { schemaCheck += " and T.constraint_type = 'PRIMARY KEY' "; }
                        break;
                    case BlackHoleSqlTypes.MySql:
                        schemaCheck = _multiDatabaseSelector.GetDatabaseName();
                        if (mode == 1) { schemaCheck += " and C.REFERENCED_TABLE_NAME is not null "; }
                        if (mode == 2) { schemaCheck += " and K.COLUMN_KEY ='PRI' "; }
                        break;
                    case BlackHoleSqlTypes.Oracle:
                        schemaCheck = _multiDatabaseSelector.GetDatabaseName();
                        if (mode == 1) { schemaCheck += " and c.constraint_type = 'R' "; }
                        if (mode == 2) { schemaCheck += " and c.constraint_type = 'P' "; }
                        break;
                    default:
                        break;
                }
            }

            return schemaCheck;
        }
    }
}
