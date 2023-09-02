using BlackHole.DataProviders;
using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.ExecutionProviders;
using BlackHole.Statics;

namespace BlackHole.CoreSupport
{
    internal static class BHDataProviderSelector
    {
        internal static IDataProvider GetDataProvider(this Type IdType, string tableName)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName, DatabaseStatics.IsQuotedDatabase),
                BlackHoleSqlTypes.MySql => new MySqlDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType),tableName),
                BlackHoleSqlTypes.Postgres => new PostgresDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType), tableName),
                BlackHoleSqlTypes.SqlLite => new SqLiteDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType),tableName, DatabaseStatics.IsQuotedDatabase),
                _ => new OracleDataProvider(DatabaseStatics.ConnectionString, GetIdType(IdType),tableName),
            };
        }

        internal static bool CheckActivator(this Type entity)
        {
            return entity.GetCustomAttributes(true).Any(x => x.GetType() == typeof(UseActivator));
        }

        internal static IExecutionProvider GetExecutionProvider()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlServerExecutionProvider(DatabaseStatics.ConnectionString, DatabaseStatics.IsQuotedDatabase),
                BlackHoleSqlTypes.MySql => new MySqlExecutionProvider(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.Postgres => new PostgresExecutionProvider(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.SqlLite => new SqLiteExecutionProvider(DatabaseStatics.ConnectionString,DatabaseStatics.IsQuotedDatabase),
                _ => new OracleExecutionProvider(DatabaseStatics.ConnectionString),
            };
        }

        internal static string GetDatabaseSchema()
        {
            if(DatabaseStatics.DatabaseSchema != string.Empty)
            {
                return $"{DatabaseStatics.DatabaseSchema}.";
            }
            return string.Empty;
        }

        private static BlackHoleIdTypes GetIdType(Type type)
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
