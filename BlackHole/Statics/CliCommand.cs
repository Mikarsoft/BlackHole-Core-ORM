
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
}
