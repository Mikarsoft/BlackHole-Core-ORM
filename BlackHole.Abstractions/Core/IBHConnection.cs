
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
        /// 
        /// </summary>
        /// <returns></returns>
        IBHTransaction BeginIBHTransaction();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBHParameters CreateBHParameters();

        /// <summary>
        /// <para>Classic Execute Scalar</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText) where G : IComparable<G>;

        /// <summary>
        /// <para> Classic Execute Scalar with IBHParameters</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, IBHParameters parameters) where G : IComparable<G>;

        /// <summary>
        /// <para> Classic Execute Scalar with Object as Parameters</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, object parametersObject)where G : IComparable<G>;

        /// <summary>
        /// <para><b>Transaction. </b>Classic Execute Scalar with IBHParameters</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where G : IComparable<G>;

        /// <summary>
        /// <para><b>Transaction. </b>Classic Execute Scalar</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, IBHTransaction bHTransaction) where G : IComparable<G>;

        /// <summary>
        /// <para><b>Transaction. </b>Classic Execute Scalar with Object as Parameters</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        G? ExecuteScalar<G>(string commandText, object parametersObject, IBHTransaction bHTransaction)where G : IComparable<G>;

        /// <summary>
        /// <para><b>Asyncronous.</b> Classic Execute Scalar</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText) where G : IComparable<G>;

        /// <summary>
        /// <para><b>Asyncronous.</b> Classic Execute Scalar with IBHParameters</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, IBHParameters parameters) where G : IComparable<G>;

        /// <summary>
        /// <para><b>Asyncronous.</b> Classic Execute Scalar with Object as Parameters</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject) where G : IComparable<G>;

        /// <summary>
        /// <para><b>Asyncronous.</b> <b>Transaction. </b>Classic Execute Scalar</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, IBHTransaction bHTransaction) where G : IComparable<G>;

        /// <summary>
        /// <para><b>Asyncronous.</b> <b>Transaction. </b>Classic Execute Scalar with IBHParameters</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where G : IComparable<G>;

        /// <summary>
        /// <para><b>Asyncronous.</b> <b>Transaction. </b>Classic Execute Scalar with Object as Parameters</para>
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="G">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        Task<G?> ExecuteScalarAsync<G>(string commandText, object parametersObject, IBHTransaction bHTransaction) where G : IComparable<G>;

        /// <summary>
        /// Classic Execute without output.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandTex">Command Text</param>
        /// <returns>Success</returns>
        bool JustExecute(string commandTex);

        /// <summary>
        /// Classic Execute with IBHParameters.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <returns>Success</returns>
        bool JustExecute(string commandText, IBHParameters parameters);

        /// <summary>
        /// Classic Execute with Object as Parameters
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>Success</returns>
        bool JustExecute(string commandText, object parametersObject);

        /// <summary>
        /// <b>Transaction. </b>Classic Execute without output.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        bool JustExecute(string commandText, IBHTransaction bHTransaction);

        /// <summary>
        /// <b>Transaction. </b>Classic Execute without output.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        bool JustExecute(string commandText, IBHParameters parameters, IBHTransaction bHTransaction);

        /// <summary>
        /// <b>Transaction. </b>Classic Execute without output.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        bool JustExecute(string commandText, object parametersObject, IBHTransaction bHTransaction);

        /// <summary>
        /// <b>Asyncronous.</b> Classic Execute without output.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <returns>Success</returns>
        Task<bool> JustExecuteAsync(string commandText);

        /// <summary>
        /// <b>Asyncronous.</b> Classic Execute with IBHParameters.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <returns>Success</returns>
        Task<bool> JustExecuteAsync(string commandText, IBHParameters parameters);

        /// <summary>
        /// <b>Asyncronous.</b> Classic Execute with Object as Parameters.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>Success</returns>
        Task<bool> JustExecuteAsync(string commandText, object parametersObject);

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>Classic Execute without output. 
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        Task<bool> JustExecuteAsync(string commandText, IBHTransaction bHTransaction);

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>Classic Execute with IBHParameters.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        Task<bool> JustExecuteAsync(string commandText, IBHParameters parameters, IBHTransaction bHTransaction);

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>Classic Execute with Object as Parameters.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        Task<bool> JustExecuteAsync(string commandText, object parametersObject, IBHTransaction bHTransaction);

        /// <summary>
        /// A Query that returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText) where T : class;

        /// <summary>
        /// A Query that takes IBHParameters and returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, IBHParameters parameters) where T : class;

        /// <summary>
        /// A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, object parametersObject) where T : class;

        /// <summary>
        /// <b>Transaction. </b>A Query that returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Transaction. </b>A Query that takes IBHParameters and returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Transaction. </b>A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        T? QueryFirst<T>(string commandText, object parametersObject, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// A Query that returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText);

        /// <summary>
        /// A Query that takes IBHParameters and returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, IBHParameters parameters) where T : class;

        /// <summary>
        /// A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, object parametersObject) where T : class;

        /// <summary>
        /// <b>Transaction. </b>A Query that returns all Lines of the Result as List. 
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Transaction. </b>A Query that takes IBHParameters and returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Transaction. </b>A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        List<T> Query<T>(string commandText, object parametersObject, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> A Query that returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> A Query that takes IBHParameters and returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, IBHParameters parameters) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>A Query that returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>A Query that takes IBHParameters and returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>A Query that takes an Object as parameters and returns only the first Line of the result.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        Task<T?> QueryFirstAsync<T>(string commandText, object parametersObject, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> A Query that returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText);

        /// <summary>
        /// <b>Asyncronous.</b> A Query that takes IBHParameters and returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, IBHParameters parameters) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, object parametersObject) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>A Query that returns all Lines of the Result as List. 
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>A Query that takes IBHParameters and returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">IBHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, IBHParameters parameters, IBHTransaction bHTransaction) where T : class;

        /// <summary>
        /// <b>Asyncronous.</b> <b>Transaction. </b>A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// <para><b>Tip:</b> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="T">Output List Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">IBHTransaction Class, contains connection and transaction</param>
        /// <returns>List of Lines</returns>
        Task<List<T>> QueryAsync<T>(string commandText, object parametersObject, IBHTransaction bHTransaction) where T : class;
    }
}
