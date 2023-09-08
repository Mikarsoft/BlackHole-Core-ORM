using BlackHole.ConnectionProvider;
using BlackHole.Logger;
using System.Data;

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

        internal BlackHoleTransaction()
        {
            ConnectionBuilder connectionBuilder = new();
            connection = connectionBuilder.GetConnection();
            connection.Open();
            _transaction = connection.BeginTransaction();
        }

        internal bool Commit()
        {
            commited = true;
            bool result = false;
            try
            {
                _transaction.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => ex.Message.CreateErrorLogs($"Transaction_Commit", "Commit", ex.ToString())).Start();
                result = false;
            }
            return result;
        }

        internal bool DoNotCommit()
        {
            commited = true;
            return commited;
        }

        internal bool RollBack()
        {
            commited = true;
            bool result = false;
            try
            {
                _transaction.Rollback();
                result = true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => ex.Message.CreateErrorLogs($"Transaction_Rollback", "Rollback", ex.ToString())).Start();
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Commit uncommited transaction. Dispose the connection and the transaction
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (!commited)
                {
                    _transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => ex.Message.CreateErrorLogs($"Transaction_Commit", "Commit", ex.ToString()));
                _transaction.Rollback();
            }

            _transaction.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
