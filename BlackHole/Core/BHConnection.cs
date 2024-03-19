using BlackHole.Engine;
using System.Reflection;

namespace BlackHole.Core
{
    internal class BHConnection : IBHConnection
    {
        private IDataProvider _executionProvider;
        private int _connectionIndex;

        internal BHConnection(int connectionIndex)
        {
            _connectionIndex = connectionIndex;
            _executionProvider = _connectionIndex.GetDataProvider();
        }

        internal BHConnection(string dbIdentity)
        {
            _connectionIndex = dbIdentity.GetConnectionIndexByIdentity();
            _executionProvider = _connectionIndex.GetDataProvider();
        }

        G? IBHConnection.ExecuteScalar<G>(string commandText) where G: default
        {
            return _executionProvider.ExecuteScalar<G>(commandText, null);
        }

        G? IBHConnection.ExecuteScalar<G>(string commandText, IBHParameters parameters) where G :default
        {
            BHParameters bhParameters = (BHParameters)parameters;
            return _executionProvider.ExecuteScalar<G>(commandText, bhParameters.Parameters);
        }

        G? IBHConnection.ExecuteScalar<G>(string commandText, object parametersObject) where G : default
        {
            return _executionProvider.ExecuteScalar<G>(commandText, MapObjectToParameters(parametersObject));
        }

        G? IBHConnection.ExecuteScalar<G>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where G  : default
        {
            BHParameters bhParameters = (BHParameters)parameters;
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.ExecuteScalar<G>(commandText, bhParameters.Parameters, transactionBh.transaction, _connectionIndex);
        }

        G? IBHConnection.ExecuteScalar<G>(string commandText, IBHTransaction bHTransaction) where G : default
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.ExecuteScalar<G>(commandText, null, transactionBh.transaction, _connectionIndex);
        }

        G? IBHConnection.ExecuteScalar<G>(string commandText, object parametersObject, IBHTransaction bHTransaction) where G : default
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.ExecuteScalar<G>(commandText, MapObjectToParameters(parametersObject), transactionBh.transaction, _connectionIndex);
        }

        async Task<G?> IBHConnection.ExecuteScalarAsync<G>(string commandText) where G : default
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, null);
        }

        async Task<G?> IBHConnection.ExecuteScalarAsync<G>(string commandText, IBHParameters parameters) where G : default
        {
            BHParameters bhParameters = (BHParameters)parameters;
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, bhParameters.Parameters);
        }

        async Task<G?> IBHConnection.ExecuteScalarAsync<G>(string commandText, object parametersObject) where G : default
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, MapObjectToParameters(parametersObject));
        }

        async Task<G?> IBHConnection.ExecuteScalarAsync<G>(string commandText, IBHTransaction bHTransaction) where G : default
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, null, transactionBh.transaction, _connectionIndex);
        }

        async Task<G?> IBHConnection.ExecuteScalarAsync<G>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where G : default
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            BHParameters bhParameters = (BHParameters)parameters;
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, bhParameters.Parameters, transactionBh.transaction, _connectionIndex);
        }

        async Task<G?> IBHConnection.ExecuteScalarAsync<G>(string commandText, object parametersObject, IBHTransaction bHTransaction) where G : default
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, MapObjectToParameters(parametersObject), transactionBh.transaction, _connectionIndex);
        }

        public bool JustExecute(string commandText)
        {
            return _executionProvider.JustExecute(commandText, null);
        }

        public bool JustExecute(string commandText, IBHParameters parameters)
        {
            BHParameters bhParameters = (BHParameters)parameters;
            return _executionProvider.JustExecute(commandText, bhParameters.Parameters);
        }

        public bool JustExecute(string commandText, object parametersObject)
        {
            return _executionProvider.JustExecute(commandText, MapObjectToParameters(parametersObject));
        }

        bool IBHConnection.JustExecute(string commandText, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.JustExecute(commandText, null, transactionBh.transaction, _connectionIndex);
        }

        bool IBHConnection.JustExecute(string commandText, IBHParameters parameters, IBHTransaction bHTransaction)
        {
            BHParameters bhParameters = (BHParameters)parameters;
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.JustExecute(commandText, bhParameters.Parameters, transactionBh.transaction, _connectionIndex);
        }

        bool IBHConnection.JustExecute(string commandText, object parametersObject, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.JustExecute(commandText , MapObjectToParameters(parametersObject), transactionBh.transaction, _connectionIndex);
        }

        async Task<bool> IBHConnection.JustExecuteAsync(string commandText)
        {
            return await _executionProvider.JustExecuteAsync(commandText, null);
        }

        async Task<bool> IBHConnection.JustExecuteAsync(string commandText, IBHParameters parameters)
        {
            BHParameters bhParameters = (BHParameters)parameters;
            return await _executionProvider.JustExecuteAsync(commandText, bhParameters.Parameters);
        }

        async Task<bool> IBHConnection.JustExecuteAsync(string commandText, object parametersObject)
        {
            return await _executionProvider.JustExecuteAsync(commandText, MapObjectToParameters(parametersObject));
        }

        async Task<bool> IBHConnection.JustExecuteAsync(string commandText, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return await _executionProvider.JustExecuteAsync(commandText, null, transactionBh.transaction, _connectionIndex);
        }

        async Task<bool> IBHConnection.JustExecuteAsync(string commandText, IBHParameters parameters, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            BHParameters bhParameters = (BHParameters)parameters;
            return await _executionProvider.JustExecuteAsync(commandText, bhParameters.Parameters, transactionBh.transaction, _connectionIndex);
        }

        async Task<bool> IBHConnection.JustExecuteAsync(string commandText, object parametersObject, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return await _executionProvider.JustExecuteAsync(commandText, MapObjectToParameters(parametersObject), transactionBh.transaction, _connectionIndex);
        }

        List<T> IBHConnection.Query<T>(string commandText)
        {
            return _executionProvider.Query<T>(commandText, null);
        }

        List<T> IBHConnection.Query<T>(string commandText, IBHParameters parameters)
        {
            BHParameters bhParameters = (BHParameters)parameters;
            return _executionProvider.Query<T>(commandText, bhParameters.Parameters);
        }

        List<T> IBHConnection.Query<T>(string commandText, object parametersObject)
        {
            return _executionProvider.Query<T>(commandText, MapObjectToParameters(parametersObject));
        }

        List<T> IBHConnection.Query<T>(string commandText, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.Query<T>(commandText, null, transactionBh.transaction, _connectionIndex);
        }

        List<T> IBHConnection.Query<T>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            BHParameters bhParameters = (BHParameters)parameters;
            return _executionProvider.Query<T>(commandText, bhParameters.Parameters, transactionBh.transaction, _connectionIndex);
        }

        List<T> IBHConnection.Query<T>(string commandText, object parametersObject, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.Query<T>(commandText, MapObjectToParameters(parametersObject), transactionBh.transaction, _connectionIndex);
        }

        async Task<List<T>> IBHConnection.QueryAsync<T>(string commandText)
        {
            return await _executionProvider.QueryAsync<T>(commandText, null);
        }

        async Task<List<T>> IBHConnection.QueryAsync<T>(string commandText, IBHParameters parameters)
        {
            BHParameters bhParameters = (BHParameters)parameters;
            return await _executionProvider.QueryAsync<T>(commandText, bhParameters.Parameters);
        }

        async Task<List<T>> IBHConnection.QueryAsync<T>(string commandText, object parametersObject)
        {
            return await _executionProvider.QueryAsync<T>(commandText, MapObjectToParameters(parametersObject));
        }

        async Task<List<T>> IBHConnection.QueryAsync<T>(string commandText, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return await _executionProvider.QueryAsync<T>(commandText, null, transactionBh.transaction, _connectionIndex);
        }

        async Task<List<T>> IBHConnection.QueryAsync<T>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            BHParameters bhParameters = (BHParameters)parameters;
            return await _executionProvider.QueryAsync<T>(commandText, bhParameters.Parameters, transactionBh.transaction, _connectionIndex);
        }

        async Task<List<T>> IBHConnection.QueryAsync<T>(string commandText, object parametersObject, IBHTransaction bHTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return await _executionProvider.QueryAsync<T>(commandText, MapObjectToParameters(parametersObject), transactionBh.transaction, _connectionIndex);
        }

        T? IBHConnection.QueryFirst<T>(string commandText) where T : class
        {
            return _executionProvider.QueryFirst<T>(commandText, null);
        }

        T? IBHConnection.QueryFirst<T>(string commandText, IBHParameters parameters) where T : class
        {
            BHParameters bhParameters = (BHParameters)parameters;
            return _executionProvider.QueryFirst<T>(commandText, bhParameters.Parameters);
        }

        T? IBHConnection.QueryFirst<T>(string commandText, object parametersObject) where T : class
        {
            return _executionProvider.QueryFirst<T>(commandText, MapObjectToParameters(parametersObject));
        }

        T? IBHConnection.QueryFirst<T>(string commandText, IBHTransaction bHTransaction) where T : class
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.QueryFirst<T>(commandText, null, transactionBh.transaction, _connectionIndex);
        }

        T? IBHConnection.QueryFirst<T>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where T : class
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            BHParameters bhParameters = (BHParameters)parameters;
            return _executionProvider.QueryFirst<T>(commandText, bhParameters.Parameters, transactionBh.transaction, _connectionIndex);
        }

        T? IBHConnection.QueryFirst<T>(string commandText, object parametersObject, IBHTransaction bHTransaction) where T : class
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return _executionProvider.QueryFirst<T>(commandText, MapObjectToParameters(parametersObject), transactionBh.transaction, _connectionIndex);
        }

        async Task<T?> IBHConnection.QueryFirstAsync<T>(string commandText) where T : class
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, null);
        }

        async Task<T?> IBHConnection.QueryFirstAsync<T>(string commandText, IBHParameters parameters) where T : class
        {
            BHParameters bhParameters = (BHParameters)parameters;
            return await _executionProvider.QueryFirstAsync<T>(commandText, bhParameters.Parameters);
        }

        async Task<T?> IBHConnection.QueryFirstAsync<T>(string commandText, object parametersObject) where T : class
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, MapObjectToParameters(parametersObject));
        }

        async Task<T?> IBHConnection.QueryFirstAsync<T>(string commandText, IBHTransaction bHTransaction) where T : class
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return await _executionProvider.QueryFirstAsync<T>(commandText, null, transactionBh.transaction, _connectionIndex);
        }

        async Task<T?> IBHConnection.QueryFirstAsync<T>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where T : class
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            BHParameters bhParameters = (BHParameters)parameters;
            return await _executionProvider.QueryFirstAsync<T>(commandText, bhParameters.Parameters, transactionBh.transaction, _connectionIndex);
        }

        async Task<T?> IBHConnection.QueryFirstAsync<T>(string commandText, object parametersObject, IBHTransaction bHTransaction) where T : class
        {
            BHTransaction transactionBh = (BHTransaction)bHTransaction;
            return await _executionProvider.QueryFirstAsync<T>(commandText, MapObjectToParameters(parametersObject), transactionBh.transaction, _connectionIndex);
        }

        private static List<BlackHoleParameter> MapObjectToParameters(object parametersObject)
        {
            PropertyInfo[] propertyInfos = parametersObject.GetType().GetProperties();
            BHParameters parameters = new ();

            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(parametersObject);
                parameters.Add(property.Name, value);
            }

            return parameters.Parameters;
        }

        IBHParameters IBHConnection.CreateBHParameters()
        {
            return new BHParameters();
        }

        IBHTransaction IBHConnection.BeginIBHTransaction()
        {
            return new BHTransaction();
        }
    }
}
