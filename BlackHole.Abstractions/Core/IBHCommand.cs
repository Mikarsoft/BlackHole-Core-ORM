using BlackHole.Core;

namespace BlackHole.Abstractions.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="databaseIdentity"></param>
        /// <returns></returns>
        IBHCommandExecution Set(string commandText, string? databaseIdentity = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <param name="databaseIdentity"></param>
        /// <returns></returns>
        IBHCommandExecution Set(string commandText, Action<IBHParameters> parameters, string? databaseIdentity = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <param name="databaseIdentity"></param>
        /// <returns></returns>
        IBHCommandExecution Set(string commandText, object[] parameters, string? databaseIdentity = null);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBHCommandExecution
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        G? ExecuteScalar<G>(IBHTransaction? transaction = null) where G : IComparable<G>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Execute(IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        T? QueryFirst<T>(IBHTransaction? transaction = null) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        List<T> Query<T>(IBHTransaction? transaction = null) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<G?> ExecuteScalarAsync<G>(IBHTransaction? transaction = null) where G : IComparable<G>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> ExecuteAsync(IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<T?> QueryFirstAsync<T>(IBHTransaction? transaction = null) where T : class;

       /// <summary>
       /// 
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="transaction"></param>
       /// <returns></returns>
        Task<List<T>> QueryAsync<T>(IBHTransaction? transaction = null) where T : class;
    }
}
