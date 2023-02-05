using BlackHole.Enums;

namespace BlackHole.Statics
{
    internal static class DatabaseStatics
    {
        internal static string ConnectionString { get; set; } = string.Empty;
        internal static string ServerConnection { get; set; } = string.Empty;
        internal static string DatabaseName { get; set; } = string.Empty;
        internal static string LogsPath { get; set; } = string.Empty;
        internal static BHSqlTypes DatabaseType { get; set; }
    }
}
