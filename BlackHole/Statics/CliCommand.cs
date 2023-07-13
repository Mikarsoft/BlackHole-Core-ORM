
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
        /// <summary>
        /// Flag for the --save argument
        /// </summary>
        internal static bool ExportSql { get; set; }
        /// <summary>
        /// Flag for the --force argument
        /// </summary>
        internal static bool ForceAction { get; set; }
        /// <summary>
        /// Is in Cli Mode
        /// </summary>
        internal static bool CliExecution { get; set; }
        /// <summary>
        /// The path of the main project
        /// </summary>
        internal static string ProjectPath { get; set; } = string.Empty;
    }
}
