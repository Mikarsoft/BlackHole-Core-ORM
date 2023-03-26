using BlackHole.CoreSupport;
using System.Reflection;

namespace BlackHole.Core
{
    public class BHConnection : IBHConnection
    {
        private readonly IExecutionProvider _executionProvider;
        private readonly IBHDataProviderSelector _dataProviderSelector;

        public BHConnection()
        {
            _dataProviderSelector = new BHDataProviderSelector();
            _executionProvider = _dataProviderSelector.GetExecutionProvider();
        }

        public G? ExecuteScalar<G>(string commandText)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, null);
        }

        public G? ExecuteScalar<G>(string commandText, BHParameters parameters)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, parameters.Parameters);
        }

        public G? ExecuteScalar<G>(string commandText, object parametersObject)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, MapObjectToParameters(parametersObject));
        }

        public G? ExecuteScalar<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        public G? ExecuteScalar<G>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, null, bHTransaction.transaction);
        }

        public G? ExecuteScalar<G>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, null);
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, BHParameters parameters)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, parameters.Parameters);
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, MapObjectToParameters(parametersObject));
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, BHTransaction bHTransaction)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, null, bHTransaction.transaction);
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        public bool JustExecute(string commandText)
        {
            return _executionProvider.JustExecute(commandText, null);
        }

        public bool JustExecute(string commandText, BHParameters parameters)
        {
            return _executionProvider.JustExecute(commandText, parameters.Parameters);
        }

        public bool JustExecute(string commandText, object parametersObject)
        {
            return _executionProvider.JustExecute(commandText,MapObjectToParameters(parametersObject));
        }

        public bool JustExecute(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText, null, bHTransaction.transaction);
        }

        public bool JustExecute(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        public bool JustExecute(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText ,MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        public async Task<bool> JustExecuteAsync(string commandText)
        {
            return await _executionProvider.JustExecuteAsync(commandText, null);
        }

        public async Task<bool> JustExecuteAsync(string commandText, BHParameters parameters)
        {
            return await _executionProvider.JustExecuteAsync(commandText, parameters.Parameters);
        }

        public async Task<bool> JustExecuteAsync(string commandText, object parametersObject)
        {
            return await _executionProvider.JustExecuteAsync(commandText, MapObjectToParameters(parametersObject));
        }

        public async Task<bool> JustExecuteAsync(string commandText, BHTransaction bHTransaction)
        {
            return await _executionProvider.JustExecuteAsync(commandText, null, bHTransaction.transaction);
        }

        public async Task<bool> JustExecuteAsync(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return await _executionProvider.JustExecuteAsync(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        public async Task<bool> JustExecuteAsync(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return await _executionProvider.JustExecuteAsync(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        public List<T> Query<T>(string commandText)
        {
            return _executionProvider.Query<T>(commandText, null);
        }

        public List<T> Query<T>(string commandText, BHParameters parameters)
        {
            return _executionProvider.Query<T>(commandText, parameters.Parameters);
        }

        public List<T> Query<T>(string commandText, object parametersObject)
        {
            return _executionProvider.Query<T>(commandText, MapObjectToParameters(parametersObject));
        }

        public List<T> Query<T>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, null, bHTransaction.transaction);
        }

        public List<T> Query<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        public List<T> Query<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        public async Task<List<T>> QueryAsync<T>(string commandText)
        {
            return await _executionProvider.QueryAsync<T>(commandText, null);
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, BHParameters parameters)
        {
            return await _executionProvider.QueryAsync<T>(commandText, parameters.Parameters);
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, object parametersObject)
        {
            return await _executionProvider.QueryAsync<T>(commandText, MapObjectToParameters(parametersObject));
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryAsync<T>(commandText, null, bHTransaction.transaction);
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryAsync<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryAsync<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        public T? QueryFirst<T>(string commandText)
        {
            return _executionProvider.QueryFirst<T>(commandText, null);
        }

        public T? QueryFirst<T>(string commandText, BHParameters parameters)
        {
            return _executionProvider.QueryFirst<T>(commandText, parameters.Parameters);
        }

        public T? QueryFirst<T>(string commandText, object parametersObject)
        {
            return _executionProvider.QueryFirst<T>(commandText, MapObjectToParameters(parametersObject));
        }

        public T? QueryFirst<T>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, null, bHTransaction.transaction);
        }

        public T? QueryFirst<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        public T? QueryFirst<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, null);
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, BHParameters parameters)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, parameters.Parameters);
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, MapObjectToParameters(parametersObject));
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, null, bHTransaction.transaction);
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        private BlackHoleParameter[] MapObjectToParameters(object parametersObject)
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
    }
}
