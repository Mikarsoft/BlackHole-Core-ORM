using BlackHole.CoreSupport;
using System.Reflection;

namespace BlackHole.Core
{
    /// <summary>
    /// An Interface that gives all
    /// the required methods to perform custom sql commands
    /// <para>It's already registered in the ServiceCollection and it can be used to 
    /// your services with Dependency Injection</para>
    /// <para>The connection is automatically generated and disposed after 
    /// each execution</para>
    /// </summary>
    public class BHConnection : IBHConnection
    {
        private readonly IExecutionProvider _executionProvider;
        private readonly IBHDataProviderSelector _dataProviderSelector;

        /// <summary>
        /// An Interface that gives all
        /// the required methods to perform custom sql commands
        /// <para>It's already registered in the ServiceCollection and it can be used to 
        /// your services with Dependency Injection</para>
        /// <para>The connection is automatically generated and disposed after 
        /// each execution</para>
        /// </summary>
        public BHConnection()
        {
            _dataProviderSelector = new BHDataProviderSelector();
            _executionProvider = _dataProviderSelector.GetExecutionProvider();
        }

        /// <summary>
        /// <para>Classic Execute Scalar</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, null);
        }

        /// <summary>
        /// <para> Classic Execute Scalar with BHParameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, BHParameters parameters)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// <para> Classic Execute Scalar with Object as Parameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, object parametersObject)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// <para>Transaction. Classic Execute Scalar with BHParameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// <para>Transaction. Classic Execute Scalar</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// <para>Transaction. Classic Execute Scalar with Object as Parameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// <para>Asyncronous. Classic Execute Scalar</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Value of the Result</returns>
        public async Task<G?> ExecuteScalarAsync<G>(string commandText)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, null);
        }

        /// <summary>
        /// <para>Asyncronous. Classic Execute Scalar with BHParameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Value of the Result</returns>
        public async Task<G?> ExecuteScalarAsync<G>(string commandText, BHParameters parameters)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// <para>Asyncronous. Classic Execute Scalar with Object as Parameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Value of the Result</returns>
        public async Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// <para>Asyncronous. Transaction. Classic Execute Scalar</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public async Task<G?> ExecuteScalarAsync<G>(string commandText, BHTransaction bHTransaction)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// <para>Asyncronous. Transaction. Classic Execute Scalar with BHParameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public async Task<G?> ExecuteScalarAsync<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// <para>Asyncronous. Transaction. Classic Execute Scalar with Object as Parameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public async Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return await _executionProvider.ExecuteScalarAsync<G>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandTex">Command Text</param>
        /// <returns>Success Boolean</returns>
        public bool JustExecute(string commandText)
        {
            return _executionProvider.JustExecute(commandText, null);
        }

        /// <summary>
        /// Classic Execute with BHParameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>Success Boolean</returns>
        public bool JustExecute(string commandText, BHParameters parameters)
        {
            return _executionProvider.JustExecute(commandText, parameters.Parameters);
        }

        /// <summary>
        /// Classic Execute with Object as Parameters
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>Success Boolean</returns>
        public bool JustExecute(string commandText, object parametersObject)
        {
            return _executionProvider.JustExecute(commandText,MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// Transaction. Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        public bool JustExecute(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// Transaction. Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        public bool JustExecute(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// Transaction. Classic Execute with Object as Parameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        public bool JustExecute(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText ,MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <returns>Success Boolean</returns>
        public async Task<bool> JustExecuteAsync(string commandText)
        {
            return await _executionProvider.JustExecuteAsync(commandText, null);
        }

        /// <summary>
        /// Asyncronous. Classic Execute with BHParameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>Success Boolean</returns>
        public async Task<bool> JustExecuteAsync(string commandText, BHParameters parameters)
        {
            return await _executionProvider.JustExecuteAsync(commandText, parameters.Parameters);
        }

        /// <summary>
        /// Asyncronous. Classic Execute with Object as Parameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>Success Boolean</returns>
        public async Task<bool> JustExecuteAsync(string commandText, object parametersObject)
        {
            return await _executionProvider.JustExecuteAsync(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// Asyncronous. Transaction. Classic Execute without output. 
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        public async Task<bool> JustExecuteAsync(string commandText, BHTransaction bHTransaction)
        {
            return await _executionProvider.JustExecuteAsync(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. Transaction. Classic Execute with BHParameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        public async Task<bool> JustExecuteAsync(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return await _executionProvider.JustExecuteAsync(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. Transaction. Classic Execute with Object as Parameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        public async Task<bool> JustExecuteAsync(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return await _executionProvider.JustExecuteAsync(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// A Query that returns returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText)
        {
            return _executionProvider.Query<T>(commandText, null);
        }

        /// <summary>
        /// A Query that takes BHParameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, BHParameters parameters)
        {
            return _executionProvider.Query<T>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, object parametersObject)
        {
            return _executionProvider.Query<T>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// Transaction. A Query that returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// Transaction. A Query that takes BHParameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// Transaction. A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. A Query that returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>List of Lines</returns>
        public async Task<List<T>> QueryAsync<T>(string commandText)
        {
            return await _executionProvider.QueryAsync<T>(commandText, null);
        }

        /// <summary>
        /// Asyncronous. A Query that takes BHParameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>List of Lines</returns>
        public async Task<List<T>> QueryAsync<T>(string commandText, BHParameters parameters)
        {
            return await _executionProvider.QueryAsync<T>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// Asyncronous. A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>List of Lines</returns>
        public async Task<List<T>> QueryAsync<T>(string commandText, object parametersObject)
        {
            return await _executionProvider.QueryAsync<T>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// Asyncronous. Transaction. A Query that returns all Lines of the Result as List. 
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        public async Task<List<T>> QueryAsync<T>(string commandText, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryAsync<T>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. Transaction. A Query that takes BHParameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        public async Task<List<T>> QueryAsync<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryAsync<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. Transaction. A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        public async Task<List<T>> QueryAsync<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryAsync<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// A Query that returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText)
        {
            return _executionProvider.QueryFirst<T>(commandText, null);
        }

        /// <summary>
        /// A Query that takes BHParameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, BHParameters parameters)
        {
            return _executionProvider.QueryFirst<T>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, object parametersObject)
        {
            return _executionProvider.QueryFirst<T>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// Transaction. A Query that returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// Transaction. A Query that takes BHParameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// Transaction. A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. A Query that returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        public async Task<T?> QueryFirstAsync<T>(string commandText)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, null);
        }

        /// <summary>
        /// Asyncronous. A Query that takes BHParameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        public async Task<T?> QueryFirstAsync<T>(string commandText, BHParameters parameters)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// Asyncronous. A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        public async Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// Asyncronous. Transaction. A Query that returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public async Task<T?> QueryFirstAsync<T>(string commandText, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. Transaction. A Query that takes BHParameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public async Task<T?> QueryFirstAsync<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// Asyncronous. Transaction. A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public async Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return await _executionProvider.QueryFirstAsync<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        private List<BlackHoleParameter> MapObjectToParameters(object parametersObject)
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
