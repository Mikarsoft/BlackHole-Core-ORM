using BlackHole.Data;
using System.Data;

namespace BlackHole.Interfaces
{
    public class BHTransaction : IDisposable
    {
        public IDbConnection connection;
        private IBHDatabaseSelector _multiDatabaseSelector;
        private IDbTransaction _transaction;

        public BHTransaction()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            connection = _multiDatabaseSelector.GetConnection();
            _transaction = connection.BeginTransaction();
        }

        public void Dispose()
        {
            _transaction.Commit();
            _transaction.Dispose();
            connection.Dispose();
        }
    }
}
