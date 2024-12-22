using Mikarsoft.BlackHoleCore.Connector;

namespace Mikarsoft.BlackHoleCore.Npgsql
{
    internal class BlackHoleNpgsqlDataProvider : IBHDataProvider
    {
        public bool ExecuteEntry<T>(string commandText, T entry)
        {
            throw new NotImplementedException();
        }

        public bool ExecuteEntry<T>(string commandText, T entry, IBHInnerTransaction bhTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExecuteEntryAsync<T>(string commandText, T entry)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, IBHInnerTransaction bhTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleInnerParameter>? parameters)
        {
            throw new NotImplementedException();
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleInnerParameter>? parameters)
        {
            throw new NotImplementedException();
        }

        public Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleInnerParameter>? parameters)
        {
            throw new NotImplementedException();
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleInnerParameter>? parameters)
        {
            throw new NotImplementedException();
        }

        public Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput)
        {
            throw new NotImplementedException();
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput)
        {
            throw new NotImplementedException();
        }

        public Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public bool JustExecute(string commandText, List<BlackHoleInnerParameter>? parameters = null)
        {
            throw new NotImplementedException();
        }

        public bool JustExecute(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bhTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public Task<bool> JustExecuteAsync(string commandText, List<BlackHoleInnerParameter>? parameters)
        {
            throw new NotImplementedException();
        }

        public Task<bool> JustExecuteAsync(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bhTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public List<G?> MultiInsertScalar<T, G>(string commandStart, string commandEnd, List<T> entries, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public List<T> Query<T>(string commandText, List<BlackHoleInnerParameter>? parameters)
        {
            throw new NotImplementedException();
        }

        public List<T> Query<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleInnerParameter>? parameters = null)
        {
            throw new NotImplementedException();
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }

        public Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters)
        {
            throw new NotImplementedException();
        }

        public Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            throw new NotImplementedException();
        }
    }
}
