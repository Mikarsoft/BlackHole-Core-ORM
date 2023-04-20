
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
    public interface IBHConnection
    {
        /// <summary>
        /// <para>Classic Execute Scalar</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText);

        /// <summary>
        /// <para> Classic Execute Scalar with BHParameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, BHParameters parameters);

        /// <summary>
        /// <para> Classic Execute Scalar with Object as Parameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, object parametersObject);

        /// <summary>
        /// <para>Transaction. Classic Execute Scalar with BHParameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction);

        /// <summary>
        /// <para>Transaction. Classic Execute Scalar</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, BHTransaction bHTransaction);

        /// <summary>
        /// <para>Transaction. Classic Execute Scalar with Object as Parameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, object parametersObject, BHTransaction bHTransaction);

        /// <summary>
        /// <para>Asyncronous. Classic Execute Scalar</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText);

        /// <summary>
        /// <para>Asyncronous. Classic Execute Scalar with BHParameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, BHParameters parameters);

        /// <summary>
        /// <para>Asyncronous. Classic Execute Scalar with Object as Parameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject);

        /// <summary>
        /// <para>Asyncronous. Transaction. Classic Execute Scalar</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, BHTransaction bHTransaction);

        /// <summary>
        /// <para>Asyncronous. Transaction. Classic Execute Scalar with BHParameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction);

        /// <summary>
        /// <para>Asyncronous. Transaction. Classic Execute Scalar with Object as Parameters</para>
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject, BHTransaction bHTransaction);

        /// <summary>
        /// Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandTex">Command Text</param>
        /// <returns>Success Boolean</returns>
        bool JustExecute(string commandTex);

        /// <summary>
        /// Classic Execute with BHParameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>Success Boolean</returns>
        bool JustExecute(string commandText, BHParameters parameters);

        /// <summary>
        /// Classic Execute with Object as Parameters
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>Success Boolean</returns>
        bool JustExecute(string commandText, object parametersObject);

        /// <summary>
        /// Transaction. Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        bool JustExecute(string commandText, BHTransaction bHTransaction);

        /// <summary>
        /// Transaction. Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        bool JustExecute(string commandText, BHParameters parameters, BHTransaction bHTransaction);

        /// <summary>
        /// Transaction. Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        bool JustExecute(string commandText, object parametersObject, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. Classic Execute without output.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <returns>Success Boolean</returns>
        Task<bool> JustExecuteAsync(string commandText);

        /// <summary>
        /// Asyncronous. Classic Execute with BHParameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>Success Boolean</returns>
        Task<bool> JustExecuteAsync(string commandText, BHParameters parameters);

        /// <summary>
        /// Asyncronous. Classic Execute with Object as Parameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>Success Boolean</returns>
        Task<bool> JustExecuteAsync(string commandText, object parametersObject);

        /// <summary>
        /// Asyncronous. Transaction. Classic Execute without output. 
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        Task<bool> JustExecuteAsync(string commandText, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. Transaction. Classic Execute with BHParameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        Task<bool> JustExecuteAsync(string commandText, BHParameters parameters, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. Transaction. Classic Execute with Object as Parameters.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success Boolean</returns>
        Task<bool> JustExecuteAsync(string commandText, object parametersObject, BHTransaction bHTransaction);

        /// <summary>
        /// A Query that returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText);

        /// <summary>
        /// A Query that takes BHParameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, BHParameters parameters);

        /// <summary>
        /// A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, object parametersObject);

        /// <summary>
        /// Transaction. A Query that returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, BHTransaction bHTransaction);

        /// <summary>
        /// Transaction. A Query that takes BHParameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction);

        /// <summary>
        /// Transaction. A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, object parametersObject, BHTransaction bHTransaction);

        /// <summary>
        /// A Query that returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText);

        /// <summary>
        /// A Query that takes BHParameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, BHParameters parameters);

        /// <summary>
        /// A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, object parametersObject);

        /// <summary>
        /// Transaction. A Query that returns all Lines of the Result as List. 
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, BHTransaction bHTransaction);

        /// <summary>
        /// Transaction. A Query that takes BHParameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction);

        /// <summary>
        /// Transaction. A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, object parametersObject, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. A Query that returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText);

        /// <summary>
        /// Asyncronous. A Query that takes BHParameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, BHParameters parameters);

        /// <summary>
        /// Asyncronous. A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject);

        /// <summary>
        /// Asyncronous. Transaction. A Query that returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. Transaction. A Query that takes BHParameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. Transaction. A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. A Query that returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText);

        /// <summary>
        /// Asyncronous. A Query that takes BHParameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, BHParameters parameters);

        /// <summary>
        /// Asyncronous. A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, object parametersObject);

        /// <summary>
        /// Asyncronous. Transaction. A Query that returns all Lines of the Result as List. 
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. Transaction. A Query that takes BHParameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction);

        /// <summary>
        /// Asyncronous. Transaction. A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para>Tip: For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, object parametersObject, BHTransaction bHTransaction);
    }
}
