using BlackHole.Enums;
using BlackHole.Statics;

namespace BlackHole.Configuration
{
    internal static class DatabaseConfiguration
    {
        internal static void ScanConnectionString(string connectionString, BlackHoleSqlTypes sqlType, string LogsPath, bool useLogsCleaner , int daysToClean)
        {
            DatabaseStatics.DataPath = LogsPath;
            DatabaseStatics.UseLogsCleaner = useLogsCleaner;
            DatabaseStatics.CleanUpDays = daysToClean;

            switch (sqlType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    ScanMsSqlString(connectionString);
                    break;
                case BlackHoleSqlTypes.MySql:
                    ScanMySqlString(connectionString);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    ScanPostgresString(connectionString);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    ScanLiteString(connectionString);
                    break;
                case BlackHoleSqlTypes.Oracle:
                    ScanOracleString(connectionString);
                    break;
            }

            if (DatabaseStatics.UseLogsCleaner)
            {
                new LogsCleaner();
            }
        }

        private static void ScanOracleString(string connectionString)
        {
            string[] parts = connectionString.Split(";");
            string userName = string.Empty;

            foreach (string part in parts)
            {
                string tempPart = part.Replace(" ", "").ToLower();
                if (tempPart.Contains("userid=") || tempPart.Contains("uid="))
                {
                    DatabaseStatics.DatabaseName = part;
                }
            }
            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.Oracle;
            DatabaseStatics.ConnectionString = connectionString;
            DatabaseStatics.ServerConnection = connectionString;
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

            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.SqlServer;
            DatabaseStatics.ConnectionString = connectionString;
            DatabaseStatics.ServerConnection = serverConnection;
        }

        private static void ScanMySqlString(string connectionString)
        {
            string[] parts = connectionString.Split(";");
            string serverConnection = string.Empty;

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

            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.MySql;
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

            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.Postgres;
            DatabaseStatics.ConnectionString = connectionString;
            DatabaseStatics.ServerConnection = serverConnection;
        }

        private static void ScanLiteString(string connectionString)
        {
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

            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.SqlLite;
            DatabaseStatics.ServerConnection = connectionString;
            DatabaseStatics.ConnectionString = $"Data Source={connectionString};";
        }
    }
}