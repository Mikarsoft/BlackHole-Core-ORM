using BlackHole.Enums;

namespace BlackHole.Statics
{
    public static class DatabaseStatics
    {
        public static string ConnectionString { get; set; } = string.Empty;
        public static string ServerConnection { get; set; } = string.Empty;
        public static string DatabaseName { get; set; } = string.Empty;
        public static string LogsPath { get; set; } = string.Empty;
        public static BHSqlTypes DatabaseType { get; set; }
    }
}
