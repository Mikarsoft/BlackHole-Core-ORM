using BlackHole.Enums;
using BlackHole.Statics;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BlackHole.ConnectionProvider
{
    internal class ConnectionBuilder
    {
        internal IDbConnection GetConnection()
        {
            return DatabaseStatics.DatabaseType switch
            {
                BlackHoleSqlTypes.SqlServer => new SqlConnection(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.MySql => new MySqlConnection(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.Postgres => new NpgsqlConnection(DatabaseStatics.ConnectionString),
                BlackHoleSqlTypes.SqlLite => new SqliteConnection(DatabaseStatics.ConnectionString),
                _ => new OracleConnection(DatabaseStatics.ConnectionString),
            };
        }
    }
}
