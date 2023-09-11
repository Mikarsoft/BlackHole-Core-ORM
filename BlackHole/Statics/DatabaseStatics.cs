using BlackHole.Enums;

namespace BlackHole.Statics
{
    internal static class DatabaseStatics
    {
        internal static string ConnectionString { get; set; } = string.Empty;
        internal static string DatabaseSchema { get; set; } = string.Empty;
        internal static string ServerConnection { get; set; } = string.Empty;
        internal static string DatabaseName { get; set; } = string.Empty;
        internal static string OwnerName { get; set; } = string.Empty;
        internal static string DataPath { get; set; } = string.Empty;
        internal static BlackHoleSqlTypes DatabaseType { get; set; }
        internal static bool UseLogsCleaner { get; set; } = true;
        internal static int CleanUpDays { get; set; }
        internal static bool UseLogging { get; set; } = true;
        internal static bool IsDevMove { get; set; } = false;
        internal static bool AutoUpdate { get; set; } = false;
        internal static bool IsQuotedDatabase { get; set; } = false;
        internal static bool InitializeData { get; set; } = false;
        internal static string DbDateFormat { get; set; } = "yyyy-MM-dd";
    }
}
