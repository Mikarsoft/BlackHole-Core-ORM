using BlackHole.DataProviders;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.CoreSupport
{
    internal class BHDataProviderSelector : IBHDataProviderSelector
    {
        IDataProvider IBHDataProviderSelector.GetDataProvider(Type IdType)
        {
            IDataProvider dataProvider;

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    //dataProvider = new SqlServerDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType));
                    break;
                case BlackHoleSqlTypes.MySql:
                    dataProvider = new MySqlDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType));
                    break;
                case BlackHoleSqlTypes.Postgres:
                    //dataProvider = new PostgresDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType));
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    //dataProvider = new SqLiteDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType));
                    break;
                default:
                    //dataProvider = new OracleDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType));
                    break;
            }

            return new MySqlDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType));
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
