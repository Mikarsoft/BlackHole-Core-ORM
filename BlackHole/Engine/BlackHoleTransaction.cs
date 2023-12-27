using System.Data;

namespace BlackHole.Engine
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
        private bool committed = false;
        internal bool hasError = false;
        private bool pendingRollback = false;

        internal BlackHoleTransaction()
        {
            connection = BlackHoleEngine.GetConnection();
            connection.Open();
            _transaction = connection.BeginTransaction();
        }

        internal bool Commit()
        {
            if (!committed)
            {
                committed = true;

                if (hasError)
                {
                    _transaction.Rollback();
                    return false;
                }

                _transaction.Commit();
                return committed;
            }
            return false;
        }

        internal bool DoNotCommit()
        {
            if (!committed)
            {
                committed = true;
                pendingRollback = true;
                return true;
            }

            return false;
        }

        internal bool RollBack()
        {
            if (!committed || pendingRollback)
            {
                _transaction.Rollback();
                hasError = false;
                return true;
            }
 
            return false;
        }

        /// <summary>
        /// Commit uncommitted transaction. Dispose the connection and the transaction
        /// </summary>
        public void Dispose()
        {
            if (!committed)
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
