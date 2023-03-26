
namespace BlackHole.CoreSupport
{
    internal interface IExecutionProvider
    {
        G? ExecuteScalar<G>(string commandText, BlackHoleParameter[]? parameters);
        G? ExecuteScalar<G>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        Task<G?> ExecuteScalarAsync<G>(string commandText, BlackHoleParameter[]? parameters);
        Task<G?> ExecuteScalarAsync<G>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        bool JustExecute(string commandText, BlackHoleParameter[]? parameters);
        bool JustExecute(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters);
        Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        T? QueryFirst<T>(string commandText, BlackHoleParameter[]? parameters);
        T? QueryFirst<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        List<T> Query<T>(string command, BlackHoleParameter[]? parameters);
        List<T> Query<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction BlackHoleTransaction);

        Task<T?> QueryFirstAsync<T>(string command, BlackHoleParameter[]? parameters);
        Task<T?> QueryFirstAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        Task<List<T>> QueryAsync<T>(string command, BlackHoleParameter[]? parameters);
        Task<List<T>> QueryAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);
    }
}
