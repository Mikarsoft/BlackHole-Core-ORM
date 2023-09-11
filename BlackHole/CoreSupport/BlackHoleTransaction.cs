using BlackHole.ConnectionProvider;
using System.Data;
using System.Transactions;

namespace BlackHole.CoreSupport
{
    /// <summary>
    /// Transaction Object
    /// </summary>
    public class BlackHoleTransaction : IDisposable
    {
        /// <summary>
        /// Generic connection
        /// </summary>
        public IDbConnection connection;
        /// <summary>
        /// Generic transaction
        /// </summary>
        public IDbTransaction _transaction;
        private bool commited = false;
        internal bool hasError = false;
        private bool pendingRollback = false;

        internal BlackHoleTransaction()
        {
            ConnectionBuilder connectionBuilder = new();
            connection = connectionBuilder.GetConnection();
            connection.Open();
            _transaction = connection.BeginTransaction();
        }

        internal bool Commit()
        {
            if (!commited)
            {
                commited = true;

                if (hasError)
                {
                    _transaction.Rollback();
                    return false;
                }

                _transaction.Commit();
                return commited;
            }
            return false;
        }

        internal bool DoNotCommit()
        {
            if (!commited)
            {
                commited = true;
                pendingRollback = true;
                return true;
            }

            return false;
        }

        internal bool RollBack()
        {
            if (!commited || pendingRollback)
            {
                _transaction.Rollback();
                hasError = false;
                return true;
            }
 
            return false;
        }

        /// <summary>
        /// Commit uncommited transaction. Dispose the connection and the transaction
        /// </summary>
        public void Dispose()
        {
            if (!commited)
            {
                if (hasError)
                {
                    _transaction.Rollback();
                }
                else
                {
                    _transaction.Commit();
                }
            }

            if (pendingRollback)
            {
                _transaction.Rollback();
            }

            _transaction.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
