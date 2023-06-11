using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHDatabaseParser
    {
        private readonly IExecutionProvider connection;
        private readonly IBHDatabaseSelector _multiDatabaseSelector;

        internal BHDatabaseParser()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
        }

        internal void ParseDatabase()
        {
            string parseCommand = GetParseCommand();

            List<TableParsingInfo> tableInfo = connection.Query<TableParsingInfo>(parseCommand, null);
        }

        internal string GetParseCommand()
        {
            string parseCommand = string.Empty;
            string schemaCheck = GetSchemaCheckCommand();

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    parseCommand = @"SELECT tb.name as TableName, c.name as ColumnName, t.Name as DataType, c.max_length as MaxLength,
                        c.precision as NumPrecision , c.scale  as NumScale, c.is_nullable as Nullable, ISNULL(i.is_primary_key, 0) as PrimaryKey,
	                    RC.DELETE_RULE as DeleteRule, TC.TABLE_NAME as ReferencedTable, K.CONSTRAINT_NAME as ConstraintName FROM sys.columns c
                        inner join sys.types t ON c.user_type_id = t.user_type_id
                        inner join sys.tables tb  on tb.object_id = c.object_id
                        left outer join sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                        left outer join sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                        left outer join INFORMATION_SCHEMA.KEY_COLUMN_USAGE K on K.COLUMN_NAME = c.name and K.TABLE_NAME = tb.name
                        left join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC on RC.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                        left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON TC.CONSTRAINT_NAME= RC.UNIQUE_CONSTRAINT_NAME " +
                        $" {schemaCheck} order by TableName";
                    break;
                case BlackHoleSqlTypes.Postgres:
                    parseCommand = @"select K.table_name as TableName, K.column_name as ColumnName, K.udt_name as DataType,
                        K.character_maximum_length as MaxLength, K.numeric_precision as NumPrecision, K.numeric_scale as NumScale,
                        case when K.is_nullable='YES' then true else false end as Nullable, 
                        case when T.constraint_type ='PRIMARY KEY' then true else false end as PrimaryKey,
                        case when T.constraint_type = 'FOREIGN KEY' then CU.table_name else '' end as ReferencedTable,
						case when (T.constraint_type = 'FOREIGN KEY' and K.is_nullable = 'YES') then 'SET NULL'
							 when (T.constraint_type = 'FOREIGN KEY' and K.is_nullable = 'NO') then	'CASCADE'
							 else '' end as DeleteRule, T.CONSTRAINT_NAME as ConstraintName
						from INFORMATION_SCHEMA.COLUMNS K 
                        left join INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME)
                        left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS T on T.CONSTRAINT_NAME = C.CONSTRAINT_NAME 
                        left join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CU on CU.CONSTRAINT_NAME = T.CONSTRAINT_NAME " +
                        $" {schemaCheck} order by TableName";
                    break;
                case BlackHoleSqlTypes.MySql:
                    parseCommand = @"select K.TABLE_NAME as TableName, K.COLUMN_NAME as ColumnName, K.DATA_TYPE as DataType, 
                        K.CHARACTER_MAXIMUM_LENGTH as MaxLength, K.NUMERIC_PRECISION as NumPrecision, K.NUMERIC_SCALE as NumScale , 
                        case when K.IS_NULLABLE = 'YES' THEN true else false end as Nullable,
                        case when K.COLUMN_KEY ='PRI' then true else false end as PrimaryKey,
                        case when (K.COLUMN_KEY ='MUL' and  K.IS_NULLABLE = 'YES') then 'SET NULL'
	                         when (K.COLUMN_KEY ='MUL' and K.IS_NULLABLE = 'NO') then 'CASCADE'
                             else '' end as DeleteRule,
                        C.REFERENCED_TABLE_NAME as ReferencedTable, C.CONSTRAINT_NAME as ConstraintName
                        from INFORMATION_SCHEMA.COLUMNS K 
		                left outer join information_schema.key_column_usage C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME) " +
                        $"where K.TABLE_SCHEMA ='{schemaCheck}' order by TableName";
                    break;
                case BlackHoleSqlTypes.Oracle:
                    parseCommand = @"select col.table_name as TableName, col.column_name as ColumnName, col.data_type as DataType, col.data_length as MaxLength,
                        col.data_precision as NumPrecision, col.data_scale as NumScale,
                        case when col.nullable = 'Y' then 1 else 0 end as Nullable,
                        case when c.constraint_type = 'P' then 1 else 0 end as PrimaryKey,
                        c.delete_rule as DeleteRule, c.constraint_name as ConstraintName,
                        case when c.constraint_type = 'R' then c_pk.table_name else '' end as ReferencedTable
                        from sys.all_tab_columns col
                        left join sys.all_cons_columns a on (a.column_name = col.column_name and a.table_name = col.table_name and a.owner = col.owner and a.position is not null)
                        left join all_constraints c ON a.owner = c.owner AND a.constraint_name = c.constraint_name
                        left JOIN all_constraints c_pk ON c.r_owner = c_pk.owner AND c.r_constraint_name = c_pk.constraint_name "+
                        $"where col.owner ='{schemaCheck}' order by TableName";
                    break;
            }

            return parseCommand;
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
    }
}
