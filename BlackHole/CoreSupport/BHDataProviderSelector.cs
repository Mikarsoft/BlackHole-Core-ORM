using BlackHole.DataProviders;
using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.CoreSupport
{
    internal class BHDataProviderSelector : IBHDataProviderSelector
    {
        IDataProvider IBHDataProviderSelector.GetDataProvider(Type IdType)
        {
            string _connectionString = DatabaseStatics.ConnectionString;
            IDataProvider dataProvider;

            switch (DatabaseStatics.DatabaseType)
            {
                case BHSqlTypes.SqlServer:
                    dataProvider = new SqlServerDataProvider(_connectionString, GetIdType(IdType));
                    break;
                case BHSqlTypes.MySql:
                    dataProvider = new MySqlDataProvider(_connectionString, GetIdType(IdType));
                    break;
                case BHSqlTypes.Postgres:
                    dataProvider = new PostgresDataProvider(_connectionString, GetIdType(IdType));
                    break;
                case BHSqlTypes.SqlLite:
                    dataProvider = new SqLiteDataProvider(_connectionString, GetIdType(IdType));
                    break;
                default:
                    dataProvider = new OracleDataProvider(_connectionString, GetIdType(IdType));
                    break;
            }

            if (dataProvider == null)
            {
                dataProvider = new SqlServerDataProvider(_connectionString);
            }

            return dataProvider;
        }

        IExecutionProvider IBHDataProviderSelector.GetExecutionProvider()
        {
            throw new NotImplementedException();
        }

        private BHIdTypes GetIdType(Type type)
        {
            if (type == typeof(int))
            {
                return BHIdTypes.IntId;
            }

            if (type == typeof(Guid))
            {
                return BHIdTypes.GuidId;
            }

            return BHIdTypes.StringId;
        }
    }
}
