using BlackHole.Enums;
using BlackHole.Statics;
using BlackHole.Logger;

namespace BlackHole.Configuration
{
    internal static class DatabaseConfiguration
    {
        internal static void SetUpDataProviders()
        {

        }

        internal static void SetMode(bool isDevMode)
        {
            DatabaseStatics.IsDevMove = isDevMode;
        }

        internal static void SetAutoUpdateMode(bool automaticUpdate)
        {
            DatabaseStatics.AutoUpdate = automaticUpdate;
        }

        internal static void LogsSettings(string LogsPath, bool useLogsCleaner, int daysToClean, bool useLogging)
        {
            DatabaseStatics.DataPath = LogsPath;

            if (useLogging)
            {
                DatabaseStatics.UseLogging = useLogging;
                DatabaseStatics.UseLogsCleaner = useLogsCleaner;
                DatabaseStatics.CleanUpDays = daysToClean;
            }
            else
            {
                DatabaseStatics.UseLogging = useLogging;
                DatabaseStatics.UseLogsCleaner = false;
            }

            if (DatabaseStatics.UseLogsCleaner)
            {
                new LogsCleaner();
            }
            LoggerService.SetUpLogger();
        }

        internal static void ScanConnectionString(string connectionString, BlackHoleSqlTypes sqlType, string databaseSchema, int timeoutSeconds, bool isQuoted, int connectionIndex)
        {
            DatabaseStatics.IsQuotedDatabase = isQuoted;

            switch (sqlType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    ScanMsSqlString(connectionString,timeoutSeconds, connectionIndex);
                    break;
                case BlackHoleSqlTypes.MySql:
                    ScanMySqlString(connectionString,timeoutSeconds, connectionIndex);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    ScanPostgresString(connectionString,timeoutSeconds, connectionIndex);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    ScanLiteString(connectionString, connectionIndex);
                    break;
                case BlackHoleSqlTypes.Oracle:
                    ScanOracleString(connectionString,timeoutSeconds, connectionIndex);
                    break;
            }

            if (databaseSchema != string.Empty && DatabaseStatics.OwnerName != string.Empty)
            {
                DatabaseStatics.DatabaseSchema = databaseSchema;
            }
        }

        private static void ScanOracleString(string connectionString, int timeoutSeconds, int connectionIndex)
        {
            string[] parts = connectionString.Split(";");
            bool hasCommandTimeout = false;

            foreach (string part in parts)
            {
                string tempPart = part.Replace(" ", "").ToLower();
                if (tempPart.Contains("userid=") || tempPart.Contains("uid="))
                {
                    DatabaseStatics.DatabaseName = part;
                }

                if (tempPart.Contains("connectiontimeout="))
                {
                    hasCommandTimeout = true;
                }
            }

            if (!hasCommandTimeout)
            {
                connectionString = $"Connection Timeout = {timeoutSeconds};{connectionString}";
            }

            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.Oracle;
            DatabaseStatics.ConnectionString = connectionString;
            DatabaseStatics.ServerConnection = connectionString;
        }

        private static void ScanMsSqlString(string connectionString, int timeoutSeconds, int connectionIndex)
        {
            string[] parts = connectionString.Split(";");
            string serverConnection = string.Empty;
            bool hasCommandTimeout = false;

            foreach (string part in parts)
            {
                string tempPart = part.Replace(" ", "").ToLower();

                if (tempPart.Contains("userid=") || tempPart.Contains("uid="))
                {
                    DatabaseStatics.OwnerName = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

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

            if (!hasCommandTimeout)
            {
                connectionString = $"Command Timeout = {timeoutSeconds};{connectionString}";
            }

            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.SqlServer;
            DatabaseStatics.ConnectionString = connectionString;
            DatabaseStatics.ServerConnection = serverConnection;
        }

        private static void ScanMySqlString(string connectionString, int timeoutSeconds, int connectionIndex)
        {
            string[] parts = connectionString.Split(";");
            string serverConnection = string.Empty;
            bool hasCommandTimeout = false;

            foreach (string part in parts)
            {
                string tempPart = part.Replace(" ", "").ToLower();

                if (tempPart.Contains("userid=") || tempPart.Contains("uid="))
                {
                    DatabaseStatics.OwnerName = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

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

            if (!hasCommandTimeout)
            {
                connectionString = $"default command timeout = {timeoutSeconds};{connectionString}";
            }

            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.MySql;
            DatabaseStatics.ConnectionString = connectionString; // +"OldGuids=true;";
            DatabaseStatics.ServerConnection = serverConnection;
        }

        private static void ScanPostgresString(string connectionString, int timeoutSeconds, int connectionIndex)
        {
            string[] parts = connectionString.Split(";");
            string serverConnection = string.Empty;
            bool hasCommandTimeout = false;

            foreach (string part in parts)
            {
                string tempPart = part.Replace(" ", "").ToLower();

                if (tempPart.Contains("userid=") || tempPart.Contains("uid="))
                {
                    DatabaseStatics.OwnerName = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

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

            if (!hasCommandTimeout)
            {
                connectionString = $"Command Timeout = {timeoutSeconds};{connectionString}";
            }

            DatabaseStatics.DatabaseType = BlackHoleSqlTypes.Postgres;
            DatabaseStatics.ConnectionString = connectionString;
            DatabaseStatics.ServerConnection = serverConnection;
        }

        private static void ScanLiteString(string connectionString, int connectionIndex)
        {
            try
            {
                string[] pathSplit = connectionString.Split("\\");
                string[] nameOnly = pathSplit[^1].Split(".");
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