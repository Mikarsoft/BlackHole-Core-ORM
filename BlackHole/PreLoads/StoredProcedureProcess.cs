using BlackHole.Enums;
using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    internal class StoredProcedureProcess<Dto>: IStoredProcedureProcess<Dto> where Dto : BHDtoIdentifier
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

        public void StoredInSqlServer(string connectionString)
        {
            new StoredProcedureComplete<Dto>(ProcedureName, CommandText, connectionString, IsExistent, BlackHoleSqlTypes.SqlServer, false).StoreAsProcedure();
        }

        public void StoredInSqlServer(string connectionString, bool quotedDb)
        {
            new StoredProcedureComplete<Dto>(ProcedureName, CommandText, connectionString, IsExistent, BlackHoleSqlTypes.SqlServer, quotedDb).StoreAsProcedure();
        }

        public void StoredInNpgsql(string connectionString)
        {
            new StoredProcedureComplete<Dto>(ProcedureName, CommandText, connectionString, IsExistent, BlackHoleSqlTypes.Postgres, true).StoreAsProcedure();
        }

        public void StoredInOracle(string connectionString)
        {
            new StoredProcedureComplete<Dto>(ProcedureName, CommandText, connectionString, IsExistent, BlackHoleSqlTypes.Oracle, true).StoreAsProcedure();
        }

        public void StoredInMySql(string connectionString)
        {
            new StoredProcedureComplete<Dto>(ProcedureName, CommandText, connectionString, IsExistent, BlackHoleSqlTypes.MySql, false).StoreAsProcedure();
        }

        public void StoredInSqlite(string connectionString)
        {
            new StoredProcedureComplete<Dto>(ProcedureName, CommandText, connectionString, IsExistent, BlackHoleSqlTypes.SqlLite, false).StoreAsProcedure();
        }

        public void StoredInSqlite(string connectionString, bool quotedDb)
        {
            new StoredProcedureComplete<Dto>(ProcedureName, CommandText, connectionString, IsExistent, BlackHoleSqlTypes.SqlLite, quotedDb).StoreAsProcedure();
        }
    }

    internal class StoredProcedureComplete<Dto> where Dto : BHDtoIdentifier
    {
        internal StoredProcedureComplete(string procName, string commandText, string connectionString, bool existing, BlackHoleSqlTypes sqlType, bool quotedDb)
        {
            ProcedureName = procName;
            CommandText = commandText;
            ConnectionString = connectionString;
            UseQuotedDb = quotedDb;
            ConnectionType = sqlType;
            IsExistent = existing;
        }

        internal string ProcedureName { get; set; }
        internal string CommandText { get; set; } = string.Empty;
        internal bool IsExistent { get; set; }
        internal BlackHoleSqlTypes ConnectionType { get; set; }
        internal string ConnectionString { get; set; } = string.Empty;
        internal bool UseQuotedDb { get; set; }

        internal void StoreAsProcedure()
        {

        }
    }
}
