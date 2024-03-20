using BlackHole.Engine;
using BlackHole.Enums;
using BlackHole.Logger;
using BlackHole.Statics;

namespace BlackHole.Configuration
{
    internal class DatabaseInitializer
    {
        internal void InitializeProviders(int databasesCount)
        {
            databasesCount.InitializeEngine();
            WormHoleData.ConnectionStrings = new string[databasesCount];
            WormHoleData.DbTypes = new BlackHoleSqlTypes[databasesCount];
            WormHoleData.IsQuotedDb = new bool[databasesCount];
            WormHoleData.DbSchemas = new string[databasesCount];
            WormHoleData.DatabaseNames = new string[databasesCount];
            WormHoleData.OwnerNames = new string[databasesCount];
            WormHoleData.DbDateFormats = new string[databasesCount];
            WormHoleData.ServerConnections = new string[databasesCount];
            WormHoleData.InitializeData = new bool[databasesCount];

            if(WormHoleData.BlackHoleMode == BHMode.HighAvailability)
            {
                WormHoleData.DatabaseRoles = new DatabaseRole[databasesCount];
            }
        }

        internal void AssignWormholeSettings(string connectionString, BlackHoleSqlTypes sqlType, string schema, string dbIdentity, bool quotedDb, int timeoutSeconds, int index)
        {
            WormHoleData.DbTypes[index] = sqlType;
            WormHoleData.IsQuotedDb[index] = quotedDb;
            WormHoleData.DbSchemas[index] = schema;
            WormHoleData.DatabaseIdentities.Add(dbIdentity);
            WormHoleData.DbDateFormats[index] = "yyyy-MM-dd";

            switch (sqlType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    ScanMsSqlString(connectionString, timeoutSeconds, index);
                    break;
                case BlackHoleSqlTypes.MySql:
                    ScanMySqlString(connectionString, timeoutSeconds, index);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    ScanPostgresString(connectionString, timeoutSeconds, index);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    string dbPath = Path.Combine(BHStaticSettings.DataPath, $"{connectionString}.db3");
                    ScanLiteString(dbPath, connectionString, index);
                    break;
                case BlackHoleSqlTypes.Oracle:
                    ScanOracleString(connectionString, timeoutSeconds, index);
                    break;
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
                    WormHoleData.DatabaseNames[connectionIndex] = part;
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

            WormHoleData.ServerConnections[connectionIndex] = connectionString;
            WormHoleData.ConnectionStrings[connectionIndex] = connectionString;
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
                    WormHoleData.OwnerNames[connectionIndex] = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

                if (tempPart.Contains("database=") || tempPart.Contains("initialcatalog="))
                {
                    WormHoleData.DatabaseNames[connectionIndex] = part;
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

            WormHoleData.ServerConnections[connectionIndex] = serverConnection;
            WormHoleData.ConnectionStrings[connectionIndex] = connectionString;
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
                    WormHoleData.OwnerNames[connectionIndex] = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

                if (tempPart.Contains("database="))
                {
                    WormHoleData.DatabaseNames[connectionIndex] = part;
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

            WormHoleData.ServerConnections[connectionIndex] = serverConnection;
            WormHoleData.ConnectionStrings[connectionIndex] = connectionString;
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
                    WormHoleData.OwnerNames[connectionIndex] = part;
                }

                if (tempPart.Contains("commandtimeout="))
                {
                    hasCommandTimeout = true;
                }

                if (tempPart.Contains("database=") || tempPart.Contains("location="))
                {
                    WormHoleData.DatabaseNames[connectionIndex] = part;
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

            WormHoleData.ServerConnections[connectionIndex] = serverConnection;
            WormHoleData.ConnectionStrings[connectionIndex] = connectionString;
        }

        private static void ScanLiteString(string connectionString, string dbName, int connectionIndex)
        {
            WormHoleData.DatabaseNames[connectionIndex] = dbName;
            WormHoleData.ServerConnections[connectionIndex] = connectionString;
            WormHoleData.ConnectionStrings[connectionIndex] = $"Data Source={connectionString};Pooling=true;";
        }

        internal void SetBHMode(BHMode modeBH)
        {
            WormHoleData.BlackHoleMode = modeBH;
        }

        internal void SetMode(bool isDevMode)
        {
            BHStaticSettings.IsDevMove = isDevMode;
        }

        internal void SetAutoUpdateMode(bool automaticUpdate)
        {
            BHStaticSettings.AutoUpdate = automaticUpdate;
        }

        internal void LogsSettings(string LogsPath, bool useLogsCleaner, int daysToClean, bool useLogging)
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
    }
}
