
namespace BlackHole.CoreSupport
{
    internal interface IExecutionProvider
    {
        G? ExecuteScalar<G>(string commandText, BlackHoleParameter[]? parameters);
        G? ExecuteScalar<G>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction);

        Task<G?> ExecuteScalarAsync<G>(string commandText, BlackHoleParameter[]? parameters);
        Task<G?> ExecuteScalarAsync<G>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction);

        bool JustExecute(string commandText, BlackHoleParameter[]? parameters);
        bool JustExecute(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction);

        Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters);
        Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction);

        List<T> Query<T>(string command, BlackHoleParameter[]? parameters);
        List<T> Query<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction);

        Task<List<T>> QueryAsync<T>(string command, BlackHoleParameter[]? parameters);
        Task<List<T>> QueryAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction);
    }
}
