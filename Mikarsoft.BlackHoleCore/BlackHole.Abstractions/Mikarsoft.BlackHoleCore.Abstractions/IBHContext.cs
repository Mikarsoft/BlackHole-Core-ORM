
using Mikarsoft.BlackHoleCore.Abstractions.Models;
using Mikarsoft.BlackHoleCore.Entities;

namespace Mikarsoft.BlackHoleCore
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHContextBase
    {
        #region Optional Methods
        /// <summary>
        /// 
        /// </summary>
        IBHCommand Command(string commandText, string? databaseIdentity = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        BHParameters CreateParameters();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBHTransaction BeginTransaction();
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBHContext : IBHContextBase
    {
        #region Table Getters
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IBHTable<T> Table<T>() where T : BHEntity<T>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <returns></returns>
        IBHTable<T, G> Table<T, G>() where G : struct, IBHStruct where T : BHEntityAI<T, G>;
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBHTransaction : IDisposable
    {
        /// <summary>
        /// Commit the transaction.
        /// </summary>
        /// <returns></returns>
        public bool Commit();

        /// <summary>
        /// Block the transaction.
        /// </summary>
        /// <returns></returns>
        public bool DoNotCommit();

        /// <summary>
        /// RollBack the transaction
        /// </summary>
        /// <returns></returns>
        public bool RollBack();
    }

    /// <summary>
    /// 
    /// </summary>
    public class BHParameters
    {
        private List<BlackHoleParameter> _parameters = new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public void Add(string Name, object? Value)
        {
            _parameters.Add(new(Name, Value));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _parameters.Clear();
        }

        public List<BlackHoleParameter> Parameters => _parameters;
    }

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
        IBHCommandExecution Parameters(Action<BHParameters> parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IBHCommandExecution Parameters(BHParameters parameters);


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
