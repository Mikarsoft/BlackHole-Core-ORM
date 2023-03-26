using BlackHole.DataProviders;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.CoreSupport
{
    internal class BHDataProviderSelector : IBHDataProviderSelector
    {
        IDataProvider IBHDataProviderSelector.GetDataProvider(Type IdType, string tableName)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType)),
                BlackHoleSqlTypes.MySql => new MySqlDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType)),
                BlackHoleSqlTypes.Postgres => new PostgresDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName),
                BlackHoleSqlTypes.SqlLite => new SqLiteDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType)),
                _ => new OracleDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType)),
            };
        }

        IExecutionProvider IBHDataProviderSelector.GetExecutionProvider()
        {
            throw new NotImplementedException();
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
