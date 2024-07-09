using BlackHole.Core;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHCommand : IBHCommandExecution
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IBHCommandExecution Parameters(Action<IBHParameters> parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IBHCommandExecution Parameters(IBHParameters parameters);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IBHCommandExecution Parameters(object[] parameters);
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
