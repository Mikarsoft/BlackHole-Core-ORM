using BlackHole.Core;
using BlackHole.DataProviders;
using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.ExecutionProviders;
using BlackHole.Statics;
using System.Text;

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

        internal static string[] GetReturningPrimaryKey<T>(this EntitySettings<T> pkOptions,string MainProp, string MainColumn, string Tablename)
        {
            if (pkOptions.HasAutoIncrement)
            {
                return DatabaseStatics.DatabaseType switch
                {
                    BlackHoleSqlTypes.SqlServer => new string[] { $"output Inserted.{MainColumn}", ""},
                    BlackHoleSqlTypes.MySql => new string[] {"", ";SELECT LAST_INSERT_ID();" },
                    BlackHoleSqlTypes.Postgres => new string[] {"", $"returning {Tablename}.{MainColumn}"},
                    BlackHoleSqlTypes.SqlLite => new string[] {"", $"returning {MainColumn}" },
                    _ => new string[] {"", $"returning {MainColumn} into :OracleReturningValue" },
                };
            }

            return new string[2];
        }

        internal static string[] GetLimiter(this int rowsCount)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new string[] { $" TOP {rowsCount} ", "" },
                BlackHoleSqlTypes.MySql => new string[] { "", $" limit {rowsCount} " },
                BlackHoleSqlTypes.Postgres => new string[] { "", $" limit {rowsCount} " },
                BlackHoleSqlTypes.SqlLite => new string[] { "", $" limit {rowsCount} " },
                _ => new string[] { "", $" and rownum <= {rowsCount} " },
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

        internal static string OrderByToSql<T>(this BHOrderBy<T> orderByConfig, bool isMyShit)
        {
            if (orderByConfig.orderBy.LockedByError)
            {
                return string.Empty;
            }

            StringBuilder orderby = new();
            string limiter = string.Empty;

            foreach (OrderByPair pair in orderByConfig.orderBy.OrderProperties)
            {
                orderby.Append($", {pair.PropertyName.SkipNameQuotes(isMyShit)} {pair.Oriantation}");
            }

            if (orderByConfig.orderBy.TakeSpecificRange)
            {
                limiter = orderByConfig.orderBy.RowsLimiter();
            }

            return $"order by{orderby.ToString().Remove(0,1)}{limiter}";
        }

        internal static string RowsLimiter<T>(this BlackHoleOrderBy<T> limiter)
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => $" OFFSET {limiter.FromRow} ROWS FETCH NEXT {limiter.ToRow} ROWS ONLY",
                BlackHoleSqlTypes.MySql => $" LIMIT {limiter.ToRow} OFFSET {limiter.FromRow}",
                BlackHoleSqlTypes.Postgres => $" LIMIT {limiter.ToRow} OFFSET {limiter.FromRow}",
                BlackHoleSqlTypes.Oracle => $" OFFSET {limiter.FromRow} ROWS FETCH NEXT {limiter.ToRow} ROWS ONLY",
                _ => $" LIMIT {limiter.ToRow} OFFSET {limiter.FromRow}"
            };
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
