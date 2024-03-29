﻿using BlackHole.Enums;

namespace BlackHole.Statics
{
    /// <summary>
    /// A place to store the Cli Command info for execution
    /// </summary>
    public static class CliCommand
    {
        /// <summary>
        /// The main Cli Command
        /// </summary>
        public static string? BHRun { get; set; }
        internal static bool ExportSql { get; set; }
        internal static bool ForceAction { get; set; }
        internal static bool CliExecution { get; set; }
        internal static string ProjectPath { get; set; } = string.Empty;
    }

    internal static class BHStaticSettings
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
        internal static bool OnUpdateLogs { get; set; } = true;
        internal static bool IsDevMove { get; set; } = false;
        internal static bool AutoUpdate { get; set; } = false;
        internal static bool IsQuotedDatabase { get; set; } = false;
        internal static bool InitializeData { get; set; } = false;
        internal static string DbDateFormat { get; set; } = "yyyy-MM-dd";
    }

    internal static class WormHoleData
    {
        internal static byte[][] EntitiesCodes { get; set; } = new byte[0][];
        internal static int[] EntityTargetDb { get; set; } = new int[0];
        internal static string[] ConnectionStrings { get; set; } = new string[0];
        internal static List<string> DatabaseIdentities { get; set; } = new();
        internal static bool[] IsQuotedDb { get; set; } = new bool[0];
        internal static string[] DbSchemas { get; set; } = new string[0];
        internal static DatabaseRole[] DatabaseRoles { get; set; } = new DatabaseRole[0];
        internal static BlackHoleSqlTypes[] DbTypes { get; set; } = new BlackHoleSqlTypes[0];
        internal static BHMode BlackHoleMode { get; set; } = BHMode.Single;
        internal static bool IsSchemaFromNamespace { get; set; }
        internal static bool IsSchemaFromAssembly { get; set; }

        internal static string[] ServerConnections { get; set; } = new string[0];
        internal static string[] DatabaseNames { get; set; } = new string[0];
        internal static string[] OwnerNames { get; set; } = new string[0];
        internal static string[] DbDateFormats { get; set; } = new string[0];
        internal static string[] DbDefaultSchemas { get; set; } = new string[0];
        internal static bool[] InitializeData { get; set; } = new bool[0];
    }

    internal class EntityInfo
    {
        internal string EntitySchema { get; set; } = string.Empty;
        internal int DBTIndex { get; set; }
    }
}
