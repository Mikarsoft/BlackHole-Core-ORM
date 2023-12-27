using BlackHole.Enums;
using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public class StoredProcedureProcess<Dto> where Dto : BHDtoIdentifier
    {
        internal string ProcedureName { get; set; }
        internal string CommandText { get; set; } = string.Empty;

        internal bool IsExistent { get; set; }

        internal StoredProcedureProcess(string procedureName)
        {
            ProcedureName = procedureName;
            IsExistent = true;
        }

        internal StoredProcedureProcess(string procedureName, string commandText)
        {
            ProcedureName = procedureName;
            CommandText = commandText;
            IsExistent = false;
        }
    }

    public class ProcedureConnection
    {
        internal string ConnectionString { get; set; } = string.Empty;
        internal string TableSchema { get; set; } = string.Empty;
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal bool UseQuotedDb { get; set; }
    }
}
