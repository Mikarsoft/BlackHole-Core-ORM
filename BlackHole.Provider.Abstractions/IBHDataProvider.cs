using System.Data;

namespace Mikarsoft.BlackHoleCore.Connector
{
    public interface IBHDataProvider
    {
        IDbConnection GetConnection();

        G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput);
        G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex);

        Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput);
        Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex);

        List<G?> MultiInsertScalar<T, G>(string commandStart, string commandEnd, List<T> entries, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex);
        Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex);

        G? ExecuteScalar<G>(string commandText, List<BlackHoleInnerParameter>? parameters);
        G? ExecuteScalar<G>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex);

        Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleInnerParameter>? parameters);
        Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex);

        object? ExecuteRawScalar(string commandText, List<BlackHoleInnerParameter>? parameters);
        object? ExecuteRawScalar(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex);

        Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleInnerParameter>? parameters);
        Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex);

        bool ExecuteEntry<T>(string commandText, T entry);
        bool ExecuteEntry<T>(string commandText, T entry, IBHInnerTransaction bhTransaction, int connectionIndex);

        Task<bool> ExecuteEntryAsync<T>(string commandText, T entry);
        Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, IBHInnerTransaction bhTransaction, int connectionIndex);

        bool JustExecute(string commandText, List<BlackHoleInnerParameter>? parameters);
        bool JustExecute(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bhTransaction, int connectionIndex);

        Task<bool> JustExecuteAsync(string commandText, List<BlackHoleInnerParameter>? parameters);
        Task<bool> JustExecuteAsync(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bhTransaction, int connectionIndex);

        T? QueryFirst<T>(string commandText, List<BlackHoleInnerParameter>? parameters);
        T? QueryFirst<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex);

        List<T> Query<T>(string commandText, List<BlackHoleInnerParameter>? parameters);
        List<T> Query<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex);

        Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters);
        Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex);

        Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters);
        Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex);
    }
}
