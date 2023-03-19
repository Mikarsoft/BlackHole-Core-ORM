
namespace BlackHole.CoreSupport
{
    internal interface IDataProvider
    {
        bool SkipQuotes();

        G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry);
        G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction);

        Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry);
        Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction);

        List<G?> MultiInsertScalar<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction);
        Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction);

        bool ExecuteEntry<T>(string commandText, T entry);
        bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction);

        Task<bool> ExecuteEntryAsync<T>(string commandText, T entry);
        Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, BlackHoleTransaction bhTransaction);

        bool JustExecute(string commandText, BlackHoleParameter[]? parameters);
        bool JustExecute(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bhTransaction);

        Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters);
        Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bhTransaction);

        T? QueryFirst<T>(string commandText, BlackHoleParameter[]? parameters);
        T? QueryFirst<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        List<T> Query<T>(string command, BlackHoleParameter[]? parameters);
        List<T> Query<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        Task<T?> QueryFirstAsync<T>(string command, BlackHoleParameter[]? parameters);
        Task<T?> QueryFirstAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);

        Task<List<T>> QueryAsync<T>(string command, BlackHoleParameter[]? parameters);
        Task<List<T>> QueryAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction);
    }
}
