using System.Data;

namespace BlackHole.Engine
{
    internal interface IDataProvider
    {
        IDbConnection GetConnection();

        G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput);
        G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex);

        Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput);
        Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex);

        List<G?> MultiInsertScalar<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex);
        Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex);

        G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters);
        G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, int connectionIndex);

        Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters);
        Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, int connectionIndex);

        object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters);
        object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, int connectionIndex);

        Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters);
        Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, int connectionIndex);

        bool ExecuteEntry<T>(string commandText, T entry);
        bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction, int connectionIndex);

        Task<bool> ExecuteEntryAsync<T>(string commandText, T entry);
        Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, BlackHoleTransaction bhTransaction, int connectionIndex);

        bool JustExecute(string commandText, List<BlackHoleParameter>? parameters);
        bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex);

        Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters);
        Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex);

        T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters);
        T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, int connectionIndex);

        List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters);
        List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, int connectionIndex);

        Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters);
        Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, int connectionIndex);

        Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters);
        Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, int connectionIndex);
    }
}
