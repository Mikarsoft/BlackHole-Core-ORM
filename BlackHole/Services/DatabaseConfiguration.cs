using BlackHole.Enums;
using BlackHole.FunctionalObjects;
using BlackHole.Statics;
using Dapper;

namespace BlackHole.Services
{
    public static class DatabaseConfiguration
    {
        public static int SqlTypeId(BHSqlTypes databaseType)
        {
            int sqlTypeId = 0;
            switch (databaseType)
            {
                case BHSqlTypes.MsSql:
                    sqlTypeId = 0;
                    break;
                case BHSqlTypes.MySql:
                    sqlTypeId = 1;
                    break;
                case BHSqlTypes.Postgres:
                    sqlTypeId = 2;
                    break;
                case BHSqlTypes.SqlLite:
                    sqlTypeId = 3;
                    break;
            }
            return sqlTypeId;
        }

        public static BHSqlTypes SqlTypeEnum(int sqlId)
        {
            BHSqlTypes sqlType = BHSqlTypes.MsSql;
            switch (sqlId)
            {
                case 0:
                    sqlType = BHSqlTypes.MsSql;
                    break;
                case 1:
                    sqlType = BHSqlTypes.MySql;
                    break;
                case 2:
                    sqlType = BHSqlTypes.Postgres;
                    break;
                case 3:
                    sqlType = BHSqlTypes.SqlLite;
                    break;
            }
            return sqlType;
        }

        public static void Generic(DatabaseInfo databaseInfo)
        {           
            ConfigureDatabase(GetConnectionString(databaseInfo),GetServerConnectionString(databaseInfo),databaseInfo.DatabaseName,databaseInfo.SqlType);
        }

        public static void ScanConnectionString(string connectionString, BHSqlTypes sqlType, string LogsPath)
        {
            DatabaseStatics.LogsPath = LogsPath;

            switch (sqlType)
            {
                case BHSqlTypes.MsSql:
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

            DatabaseStatics.DatabaseType = BHSqlTypes.MsSql;
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

        static void ConfigureDatabase(string ConnectionString,string ServerConnection,string DatabaseName,BHSqlTypes SqlType)
        {
            DatabaseStatics.ConnectionString = ConnectionString;
            DatabaseStatics.ServerConnection = ServerConnection;
            DatabaseStatics.DatabaseName = DatabaseName;
            DatabaseStatics.DatabaseType = SqlType;
        }

        private static string GetConnectionString(DatabaseInfo dataInfo)
        {
            string connectionString = string.Empty;
            switch (dataInfo.SqlType)
            {
                case BHSqlTypes.MsSql:
                    connectionString = $"Server={dataInfo.Servername};Database={dataInfo.DatabaseName};User Id={dataInfo.User};Password={dataInfo.Password};";
                    break;
                case BHSqlTypes.MySql:
                    connectionString = $"Server={dataInfo.Servername}; Port={dataInfo.Port}; Database={dataInfo.DatabaseName}; Uid={dataInfo.User}; Pwd={dataInfo.Password};";                  
                    break;
                case BHSqlTypes.Postgres:
                    connectionString = $"Host={dataInfo.Servername};Port={dataInfo.Port};Database={dataInfo.DatabaseName};User Id={dataInfo.User};Password={dataInfo.Password};";
                    break;
                case BHSqlTypes.SqlLite:
                    connectionString = $"Data Source={dataInfo.Servername};";
                    break;
            }
            return connectionString;
        }

        private static string GetServerConnectionString(DatabaseInfo dataInfo)
        {
            string connectionString = string.Empty;
            switch (dataInfo.SqlType)
            {
                case BHSqlTypes.MsSql:
                    connectionString = $"Server={dataInfo.Servername};User Id={dataInfo.User};Password={dataInfo.Password};";
                    break;
                case BHSqlTypes.MySql:
                    connectionString = $"Server={dataInfo.Servername}; Port={dataInfo.Port};Uid={dataInfo.User}; Pwd={dataInfo.Password};";
                    break;
                case BHSqlTypes.Postgres:
                    connectionString = $"Host={dataInfo.Servername};Port={dataInfo.Port};User Id={dataInfo.User};Password={dataInfo.Password};";
                    break;
                case BHSqlTypes.SqlLite:
                    connectionString = dataInfo.Servername;
                    break;
            }
            return connectionString;
        }
    }
}