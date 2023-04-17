
namespace BlackHole.Core
{
    public interface IBHConnection
    {
        G? ExecuteScalar<G>(string commandText);
        G? ExecuteScalar<G>(string commandText, BHParameters parameters);
        G? ExecuteScalar<G>(string commandText, object parametersObject);

        G? ExecuteScalar<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction);
        G? ExecuteScalar<G>(string commandText, BHTransaction bHTransaction);
        G? ExecuteScalar<G>(string commandText, object parametersObject, BHTransaction bHTransaction);

        Task<G?> ExecuteScalarAsync<G>(string commandText);
        Task<G?> ExecuteScalarAsync<G>(string commandText, BHParameters parameters);
        Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject);

        Task<G?> ExecuteScalarAsync<G>(string commandText, BHTransaction bHTransaction);
        Task<G?> ExecuteScalarAsync<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction);
        Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject, BHTransaction bHTransaction);

        bool JustExecute(string commandTex);
        bool JustExecute(string commandText, BHParameters parameters);
        bool JustExecute(string commandText, object parametersObject);

        bool JustExecute(string commandText, BHTransaction bHTransaction);
        bool JustExecute(string commandText, BHParameters parameters, BHTransaction bHTransaction);
        bool JustExecute(string commandText, object parametersObject, BHTransaction bHTransaction);

        Task<bool> JustExecuteAsync(string commandText);
        Task<bool> JustExecuteAsync(string commandText, BHParameters parameters);
        Task<bool> JustExecuteAsync(string commandText, object parametersObject);

        Task<bool> JustExecuteAsync(string commandText, BHTransaction bHTransaction);
        Task<bool> JustExecuteAsync(string commandText, BHParameters parameters, BHTransaction bHTransaction);
        Task<bool> JustExecuteAsync(string commandText, object parametersObject, BHTransaction bHTransaction);

        T? QueryFirst<T>(string command);
        T? QueryFirst<T>(string command, BHParameters parameters);
        T? QueryFirst<T>(string command, object parametersObject);

        T? QueryFirst<T>(string commandText, BHTransaction bHTransaction);
        T? QueryFirst<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction);
        T? QueryFirst<T>(string commandText, object parametersObject, BHTransaction bHTransaction);

        List<T> Query<T>(string command);
        List<T> Query<T>(string command, BHParameters parameters);
        List<T> Query<T>(string command, object parametersObject);

        List<T> Query<T>(string commandText, BHTransaction bHTransaction);
        List<T> Query<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction);
        List<T> Query<T>(string commandText, object parametersObject, BHTransaction bHTransaction);

        Task<T?> QueryFirstAsync<T>(string command);
        Task<T?> QueryFirstAsync<T>(string command, BHParameters parameters);
        Task<T?> QueryFirstAsync<T>(string command, object parametersObject);

        Task<T?> QueryFirstAsync<T>(string commandText, BHTransaction bHTransaction);
        Task<T?> QueryFirstAsync<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction);
        Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject, BHTransaction bHTransaction);

        Task<List<T>> QueryAsync<T>(string command);
        Task<List<T>> QueryAsync<T>(string command, BHParameters parameters);
        Task<List<T>> QueryAsync<T>(string command, object parametersObject);

        Task<List<T>> QueryAsync<T>(string commandText, BHTransaction bHTransaction);
        Task<List<T>> QueryAsync<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction);
        Task<List<T>> QueryAsync<T>(string commandText, object parametersObject, BHTransaction bHTransaction);
    }
}
