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
        /// <summary>
        /// Returns and IDBConnection with the specified database
        /// </summary>
        /// <returns></returns>
        internal IDbConnection GetConnection()
        {
            string _connectionString = DatabaseStatics.ConnectionString;
            IDbConnection _Sconnection = new SqlConnection();

            switch (DatabaseStatics.DatabaseType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    _Sconnection = new SqlConnection(_connectionString);
                    break;
                case BlackHoleSqlTypes.MySql:
                    _Sconnection = new MySqlConnection(_connectionString);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    _Sconnection = new NpgsqlConnection(_connectionString);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    _Sconnection = new SqliteConnection(_connectionString);
                    break;
                case BlackHoleSqlTypes.Oracle:
                    _Sconnection = new OracleConnection(_connectionString);
                    break;
            }

            return _Sconnection;
        }
    }
}
