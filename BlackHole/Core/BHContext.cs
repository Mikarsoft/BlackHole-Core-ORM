using BlackHole.Engine;
using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BHContext
    {
        internal InitialTransaction _transaction;

        /// <summary>
        /// 
        /// </summary>
        public BHContext()
        {
            _transaction = new InitialTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <returns></returns>
        public IBHDataProvider<T,G> For<T, G>() where T : BlackHoleEntity<G> where G: IComparable<G>
        {
            return new BHDataProvider<T, G>(_transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IBHOpenDataProvider<T> For<T>() where T : BHOpenEntity<T>
        {
            return new BHOpenDataProvider<T>(_transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IBHConnection Connection()
        {
            return new BHConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SaveChanges()
        {
            return _transaction.Commit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChangesAsync()
        {
            bool success = false;
            await Task.Factory.StartNew(() => success = _transaction.Commit()).ConfigureAwait(false);
            return success;
        }
    }
}
