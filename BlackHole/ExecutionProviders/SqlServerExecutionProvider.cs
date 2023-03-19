using BlackHole.CoreSupport;

namespace BlackHole.ExecutionProviders
{
    internal class SqlServerExecutionProvider : IExecutionProvider
    {
        public G? ExecuteScalar<G>(string commandText, BlackHoleParameter[]? parameters)
        {
            throw new NotImplementedException();
        }

        public G? ExecuteScalar<G>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction)
        {
            throw new NotImplementedException();
        }

        public Task<G?> ExecuteScalarAsync<G>(string commandText, BlackHoleParameter[]? parameters)
        {
            throw new NotImplementedException();
        }

        public Task<G?> ExecuteScalarAsync<G>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction)
        {
            throw new NotImplementedException();
        }

        public bool JustExecute(string commandText, BlackHoleParameter[]? parameters)
        {
            throw new NotImplementedException();
        }

        public bool JustExecute(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters)
        {
            throw new NotImplementedException();
        }

        public Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction)
        {
            throw new NotImplementedException();
        }

        public List<T> Query<T>(string command, BlackHoleParameter[]? parameters)
        {
            throw new NotImplementedException();
        }

        public List<T> Query<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string command, BlackHoleParameter[]? parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
