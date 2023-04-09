using BlackHole.DataProviders;
using BlackHole.Enums;
using BlackHole.ExecutionProviders;
using BlackHole.Statics;

namespace BlackHole.CoreSupport
{
    internal class BHDataProviderSelector : IBHDataProviderSelector
    {
        IDataProvider IBHDataProviderSelector.GetDataProvider(Type IdType, string tableName)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName),
                BlackHoleSqlTypes.MySql => new MySqlDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType),tableName),
                BlackHoleSqlTypes.Postgres => new PostgresDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName),
                BlackHoleSqlTypes.SqlLite => new SqLiteDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType),tableName),
                _ => new OracleDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType),tableName),
            };
        }

        IExecutionProvider IBHDataProviderSelector.GetExecutionProvider()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerExecutionProvider(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.MySql => new MySqlExecutionProvider(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.Postgres => new PostgresExecutionProvider(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.SqlLite => new SqLiteExecutionProvider(DatabaseStatics.ConnectionString),
                _ => new OracleExecutionProvider(DatabaseStatics.ConnectionString),
            };
        }

        private BlackHoleIdTypes GetIdType(Type type)
        {
            if (type == typeof(int))
            {
                return BlackHoleIdTypes.IntId;
            }

            if (type == typeof(Guid))
            {
                return BlackHoleIdTypes.GuidId;
            }

            return BlackHoleIdTypes.StringId;
        }
    }
}
