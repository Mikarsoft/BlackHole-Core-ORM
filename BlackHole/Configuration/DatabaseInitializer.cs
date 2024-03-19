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

            if(WormHoleData.BlackHoleMode == BHMode.HighAvailability)
            {
                WormHoleData.DatabaseRoles = new DatabaseRole[databasesCount];
            }
        }

        internal void AssignWormholeSettings(string connectionString, BlackHoleSqlTypes sqlType, string schema, string dbIdentity, bool quotedDb, int index)
        {
            WormHoleData.ConnectionStrings[index] = connectionString;
            WormHoleData.DbTypes[index] = sqlType;
            WormHoleData.IsQuotedDb[index] = quotedDb;
            WormHoleData.DbSchemas[index] = schema;
            WormHoleData.DatabaseIdentities.Add(dbIdentity);
        }

        internal void SetBHMode(BHMode modeBH)
        {
            WormHoleData.BlackHoleMode = modeBH;
        }

        internal void SetMode(bool isDevMode)
        {
            DatabaseStatics.IsDevMove = isDevMode;
        }

        internal void SetAutoUpdateMode(bool automaticUpdate)
        {
            DatabaseStatics.AutoUpdate = automaticUpdate;
        }

        internal void LogsSettings(string LogsPath, bool useLogsCleaner, int daysToClean, bool useLogging)
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

        internal void ScanConnectionString(string connectionString, BlackHoleSqlTypes sqlType, string databaseSchema, int timeoutSeconds, bool isQuoted)
        {
            DatabaseStatics.IsQuotedDatabase = isQuoted;

            switch (sqlType)
            {
                case BlackHoleSqlTypes.SqlServer:
                    ScanMsSqlString(connectionString, timeoutSeconds);
                    break;
                case BlackHoleSqlTypes.MySql:
                    ScanMySqlString(connectionString, timeoutSeconds);
                    break;
                case BlackHoleSqlTypes.Postgres:
                    ScanPostgresString(connectionString, timeoutSeconds);
                    break;
                case BlackHoleSqlTypes.SqlLite:
                    ScanLiteString(connectionString);
                    break;
                case BlackHoleSqlTypes.Oracle:
                    ScanOracleString(connectionString, timeoutSeconds);
                    break;
            }

            if (databaseSchema != string.Empty && DatabaseStatics.OwnerName != string.Empty)
            {
                DatabaseStatics.DatabaseSchema = databaseSchema;
            }
        }

        private void ScanOracleString(string connectionString, int timeoutSeconds)
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

        private void ScanMsSqlString(string connectionString, int timeoutSeconds)
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

        private void ScanMySqlString(string connectionString, int timeoutSeconds)
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

        private void ScanPostgresString(string connectionString, int timeoutSeconds)
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

        private void ScanLiteString(string connectionString)
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
