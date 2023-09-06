
namespace BlackHole.CoreSupport
{
    internal interface IExecutionProvider
    {
        bool SkipQuotes();

        G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters);
        G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);

        Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters);
        Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);

        object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters);
        object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);

        Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters);
        Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);

        bool JustExecute(string commandText, List<BlackHoleParameter>? parameters);
        bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);

        Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters);
        Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);

        T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters);
        T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);

        List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters);
        List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction BlackHoleTransaction);

        Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters);
        Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);

        Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters);
        Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction);
    }
}
