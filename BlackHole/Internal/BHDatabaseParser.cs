﻿using BlackHole.CoreSupport;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHDatabaseParser
    {
        private string SQLServerSelectTables = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'adoo'";
        private readonly IExecutionProvider connection;
        private readonly IBHDatabaseSelector _multiDatabaseSelector;

        private string pgParse = @"select * from INFORMATION_SCHEMA.COLUMNS K 
Left JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME)
left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS T on T.CONSTRAINT_NAME = C.CONSTRAINT_NAME
where K.table_schema='adoo'";

        internal BHDatabaseParser()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetExecutionProvider(DatabaseStatics.ConnectionString);
        }

        internal void ParseDatabase()
        {
            string schemaCheck = string.Empty;

            if(DatabaseStatics.DatabaseSchema != string.Empty)
            {
                schemaCheck = $"WHERE SCHEMA_NAME(tb.schema_id) = '{DatabaseStatics.DatabaseSchema}'";
            }

            string PgSqlParse = @"select K.table_name as TableName, K.column_name as ColumnName, K.udt_name as DataType,
                        K.character_maximum_length as MaxLength, K.numeric_precision as Precision, K.numeric_scale as Scale,
                        case when K.is_nullable='YES' then true else false end as Nullable, 
                        case when T.constraint_type ='PRIMARY KEY' then true else false end as PrimaryKey,
                        case when T.constraint_type = 'FOREIGN KEY' then CU.table_name else '' end as ReferencedTable from INFORMATION_SCHEMA.COLUMNS K 
                        left join INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME)
                        left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS T on T.CONSTRAINT_NAME = C.CONSTRAINT_NAME 
                        left join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CU on CU.CONSTRAINT_NAME = T.CONSTRAINT_NAME
                        where K.table_schema='adoo'";

            string MsSqlServerParse = @"SELECT tb.name as TableName, c.name as ColumnName, t.Name as DataType, c.max_length as MaxLength,
                        c.precision as Precision , c.scale  as Scale, c.is_nullable as Nullable, ISNULL(i.is_primary_key, 0) as PrimaryKey,
	                    RC.DELETE_RULE as DeleteRule, TC.TABLE_NAME as ReferencedTable FROM sys.columns c
                        inner join sys.types t ON c.user_type_id = t.user_type_id
                        inner join sys.tables tb  on tb.object_id = c.object_id
                        left outer join sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                        left outer join sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                        left outer join INFORMATION_SCHEMA.KEY_COLUMN_USAGE K on K.COLUMN_NAME = c.name and K.TABLE_NAME = tb.name
                        left join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC on RC.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                        left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON TC.CONSTRAINT_NAME= RC.UNIQUE_CONSTRAINT_NAME"+
                        $" {schemaCheck} order by TableName";

            List<TableParsingInfo> tableInfo = connection.Query<TableParsingInfo>(PgSqlParse, null);
        }
    }
}
