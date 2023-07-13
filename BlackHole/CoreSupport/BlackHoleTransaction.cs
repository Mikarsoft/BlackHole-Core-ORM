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
        private ILoggerService _loggerService;
        private bool commited = false;

        internal BlackHoleTransaction()
        {
            ConnectionBuilder connectionBuilder = new ConnectionBuilder();
            connection = connectionBuilder.GetConnection();
            connection.Open();
            _transaction = connection.BeginTransaction();
            _loggerService = new LoggerService();
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
                new Thread(() => _loggerService.CreateErrorLogs($"Transaction_Commit", "Commit", ex.Message, ex.ToString())).Start();
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
                new Thread(() => _loggerService.CreateErrorLogs($"Transaction_Rollback", "Rollback", ex.Message, ex.ToString())).Start();
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
                new Thread(() => _loggerService.CreateErrorLogs($"Transaction_Commit", "Commit", ex.Message, ex.ToString())).Start();
                _transaction.Rollback();
            }

            _transaction.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
