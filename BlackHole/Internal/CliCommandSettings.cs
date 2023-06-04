using BlackHole.Enums;

namespace BlackHole.Internal
{
    internal class CliCommandSettings
    {
        internal CliCommandTypes commandType { get; set; }
        internal bool forceExecution { get; set; }
        internal bool saveExecutionSql { get; set; }
    }
}
