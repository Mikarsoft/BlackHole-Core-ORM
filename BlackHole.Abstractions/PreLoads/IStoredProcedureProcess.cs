using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public interface IStoredProcedureProcess<Dto>
        where Dto : BHDtoIdentifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        void StoredInSqlServer(string connectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="quotedDb"></param>
        void StoredInSqlServer(string connectionString, bool quotedDb);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        void StoredInNpgsql(string connectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        void StoredInOracle(string connectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>

        void StoredInMySql(string connectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        void StoredInSqlite(string connectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="quotedDb"></param>
        void StoredInSqlite(string connectionString, bool quotedDb);
    }
}
