using BlackHole.Statics;
using System.Data;
using System.Data.Common;

namespace BlackHole.RayCore
{
    internal class RayConnection : DbConnection, IAsyncDisposable
    {
        private string _databaseName;
        private string _databaseConnectionString;
        private string _dataSource;
        internal RayConnection(string connectionString)
        {
            _databaseConnectionString = connectionString;
            _databaseName = DatabaseStatics.DatabaseName;
            _dataSource = DatabaseStatics.ServerConnection;
        }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override string ConnectionString {  get { return _databaseConnectionString; } set { _databaseConnectionString = value ?? string.Empty; } }
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

        public override string DataSource => _dataSource;

        public override string ServerVersion => throw new NotImplementedException();

        public override ConnectionState State => ConnectionState.Connecting;

        public override string Database => _databaseName;

        public override void ChangeDatabase(string databaseName)
        {
            _databaseName = databaseName;
        }

        public async override void Close()
        {
            await CloseAsync();
        }

        public async override void Open()
        {
            await OpenAsync();
        }

        public override ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return CreateCommand();
        }
    }
}
