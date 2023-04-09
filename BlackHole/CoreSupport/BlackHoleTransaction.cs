using BlackHole.ConnectionProvider;
using BlackHole.Logger;
using System.Data;

namespace BlackHole.CoreSupport
{
    public class BlackHoleTransaction : IDisposable
    {
        public IDbConnection connection;
        public IDbTransaction _transaction;
        private ILoggerService _loggerService;
        private bool commited = false;

        public BlackHoleTransaction()
        {
            ConnectionBuilder connectionBuilder = new ConnectionBuilder();
            connection = connectionBuilder.GetConnection();
            connection.Open();
            _transaction = connection.BeginTransaction();
            _loggerService = new LoggerService();
        }

        public bool Commit()
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
                new Thread(() => _loggerService.CreateErrorLogs($"Transaction_Commit", ex.Message, ex.ToString())).Start();
                result = false;
            }
            return result;
        }

        public bool DoNotCommit()
        {
            commited = true;
            return commited;
        }

        public bool RollBack()
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
                new Thread(() => _loggerService.CreateErrorLogs($"Transaction_Rollback", ex.Message, ex.ToString())).Start();
                result = false;
            }
            return result;
        }

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
                new Thread(() => _loggerService.CreateErrorLogs($"Transaction_Commit", ex.Message, ex.ToString())).Start();
                _transaction.Rollback();
            }

            _transaction.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
