using BlackHole.Enums;
using BlackHole.Statics;
using Dapper;

namespace BlackHole.Services
{
    internal static class DatabaseConfiguration
    {
        internal static void ScanConnectionString(string connectionString, BHSqlTypes sqlType, string LogsPath)
        {
            DatabaseStatics.LogsPath = LogsPath;

            switch (sqlType)
            {
                case BHSqlTypes.MicrosoftSql:
                    ScanMsSqlString(connectionString);
                    break;
                case BHSqlTypes.MySql:
                    ScanMySqlString(connectionString);
                    break;
                case BHSqlTypes.Postgres:
                    ScanPostgresString(connectionString);
                    break;
                case BHSqlTypes.SqlLite:
                    ScanLiteString(connectionString);
                    break;
            }
        }

        private static void ScanMsSqlString(string connectionString)
        {
            string[] parts = connectionString.Split(";");
            string serverConnection = string.Empty;

            foreach (string part in parts)
            {
                string tempPart = part.Replace(" ", "").ToLower();
                if (tempPart.Contains("database=") || tempPart.Contains("initialcatalog="))
                {
                    DatabaseStatics.DatabaseName = part;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(part))
                    {
                        serverConnection += $"{part};";
                    }
                }
            }

            DatabaseStatics.DatabaseType = BHSqlTypes.MicrosoftSql;
            DatabaseStatics.ConnectionString = connectionString;
            DatabaseStatics.ServerConnection = serverConnection;
        }

        private static void ScanMySqlString(string connectionString)
        {
            string[] parts = connectionString.Split(";");
            string serverConnection = string.Empty;

            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.AddTypeHandler(new GuidMapperLite());

            foreach (string part in parts)
            {
                string tempPart = part.Replace(" ", "").ToLower();
                if (tempPart.Contains("database="))
                {
                    DatabaseStatics.DatabaseName = part;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(part))
                    {
                        serverConnection += $"{part};";
                    }
                }
            }

            DatabaseStatics.DatabaseType = BHSqlTypes.MySql;
            DatabaseStatics.ConnectionString = connectionString; // +"OldGuids=true;";
            DatabaseStatics.ServerConnection = serverConnection;
        }

        private static void ScanPostgresString(string connectionString)
        {
            string[] parts = connectionString.Split(";");
            string serverConnection = string.Empty;

            foreach (string part in parts)
            {
                string tempPart = part.Replace(" ", "").ToLower();
                if (tempPart.Contains("database=") || tempPart.Contains("location="))
                {
                    DatabaseStatics.DatabaseName = part;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(part))
                    {
                        serverConnection += $"{part};";
                    }
                }
            }

            DatabaseStatics.DatabaseType = BHSqlTypes.Postgres;
            DatabaseStatics.ConnectionString = connectionString;
            DatabaseStatics.ServerConnection = serverConnection;
        }

        private static void ScanLiteString(string connectionString)
        {
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.AddTypeHandler(new GuidMapperLite());

            try
            {
                string[] pathSplit = connectionString.Split("\\");
                string[] nameOnly = pathSplit[pathSplit.Length - 1].Split(".");
                DatabaseStatics.DatabaseName = nameOnly[0];
            }
            catch
            {
                DatabaseStatics.DatabaseName = connectionString;
            }

            DatabaseStatics.DatabaseType = BHSqlTypes.SqlLite;
            DatabaseStatics.ServerConnection = connectionString;
            DatabaseStatics.ConnectionString = $"Data Source={connectionString};";
        }
    }
}