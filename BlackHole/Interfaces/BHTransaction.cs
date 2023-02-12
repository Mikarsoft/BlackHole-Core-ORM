using BlackHole.Data;
using BlackHole.Services;
using System.Data;

namespace BlackHole.Interfaces
{
    public class BHTransaction : IDisposable
    {
        public IDbConnection connection;
        private IBHDatabaseSelector _multiDatabaseSelector;
        public IDbTransaction _transaction;
        private ILoggerService _loggerService;
        private bool commited = false;

        public BHTransaction()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetConnection();
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
            catch(Exception ex)
            {
                _loggerService.CreateErrorLogs($"Transaction", ex.Message, ex.ToString());
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
                _loggerService.CreateErrorLogs($"Transaction", ex.Message, ex.ToString());
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
                _loggerService.CreateErrorLogs($"Transaction", ex.Message, ex.ToString());
                _transaction.Rollback();
            }

            _transaction.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
