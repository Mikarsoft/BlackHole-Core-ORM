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
            BHStaticSettings.IsDevMove = isDevMode;
        }

        internal static void SetAutoUpdateMode(bool automaticUpdate)
        {
            BHStaticSettings.AutoUpdate = automaticUpdate;
        }

        internal static void LogsSettings(string LogsPath, bool useLogsCleaner, int daysToClean, bool useLogging)
        {
            BHStaticSettings.DataPath = LogsPath;

            if (useLogging)
            {
                BHStaticSettings.UseLogging = useLogging;
                BHStaticSettings.UseLogsCleaner = useLogsCleaner;
                BHStaticSettings.CleanUpDays = daysToClean;
            }
            else
            {
                BHStaticSettings.UseLogging = useLogging;
                BHStaticSettings.UseLogsCleaner = false;
            }

            if (BHStaticSettings.UseLogsCleaner)
            {
                new LogsCleaner();
            }

            LoggerService.SetUpLogger();
        }

        internal static void ScanConnectionString(string connectionString, BlackHoleSqlTypes sqlType, string databaseSchema, int timeoutSeconds, bool isQuoted, int connectionIndex)
        {
            BHStaticSettings.IsQuotedDatabase = isQuoted;

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
                    connectionString = Path.Combine(BHStaticSettings.DataPath, $"{connectionString}.db3");
                    ScanLiteString(connectionString, connectionIndex);
                    break;
                case BlackHoleSqlTypes.Oracle:
                    ScanOracleString(connectionString,timeoutSeconds, connectionIndex);
                    break;
            }

            if (databaseSchema != string.Empty && BHStaticSettings.OwnerName != string.Empty)
            {
                BHStaticSettings.DatabaseSchema = databaseSchema;
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
                    BHStaticSettings.DatabaseName = part;
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

            BHStaticSettings.DatabaseType = BlackHoleSqlTypes.Oracle;
            BHStaticSettings.ConnectionString = connectionString;
            BHStaticSettings.ServerConnection = connectionString;
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
                    BHStaticSettings.OwnerName = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

                if (tempPart.Contains("database=") || tempPart.Contains("initialcatalog="))
                {
                    BHStaticSettings.DatabaseName = part;
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

            BHStaticSettings.DatabaseType = BlackHoleSqlTypes.SqlServer;
            BHStaticSettings.ConnectionString = connectionString;
            BHStaticSettings.ServerConnection = serverConnection;
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
                    BHStaticSettings.OwnerName = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

                if (tempPart.Contains("database="))
                {
                    BHStaticSettings.DatabaseName = part;
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

            BHStaticSettings.DatabaseType = BlackHoleSqlTypes.MySql;
            BHStaticSettings.ConnectionString = connectionString; // +"OldGuids=true;";
            BHStaticSettings.ServerConnection = serverConnection;
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
                    BHStaticSettings.OwnerName = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

                if (tempPart.Contains("database=") || tempPart.Contains("location="))
                {
                    BHStaticSettings.DatabaseName = part;
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

            BHStaticSettings.DatabaseType = BlackHoleSqlTypes.Postgres;
            BHStaticSettings.ConnectionString = connectionString;
            BHStaticSettings.ServerConnection = serverConnection;
        }

        private static void ScanLiteString(string connectionString, int connectionIndex)
        {
            try
            {
                string[] pathSplit = connectionString.Split("\\");
                string[] nameOnly = pathSplit[^1].Split(".");
                BHStaticSettings.DatabaseName = nameOnly[0];
            }
            catch
            {
                BHStaticSettings.DatabaseName = connectionString;
            }

            BHStaticSettings.DatabaseType = BlackHoleSqlTypes.SqlLite;
            BHStaticSettings.ServerConnection = connectionString;
            BHStaticSettings.ConnectionString = $"Data Source={connectionString};";
        }
    }
}