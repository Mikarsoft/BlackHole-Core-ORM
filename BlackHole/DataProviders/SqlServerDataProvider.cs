using BlackHole.Engine;
using BlackHole.Logger;
using System.Reflection;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BlackHole.DataProviders
{
    internal class SqlServerDataProvider : IDataProvider
    {
        #region Ctor
        private readonly string _connectionString;
        internal readonly string PK = "Id";

        internal SqlServerDataProvider(string connectionString, bool isQuotedDb)
        {
            _connectionString = connectionString;
            if (isQuotedDb) { PK = @"""Id""";}
        }
        #endregion

        #region Internal Processes
        private G? ExecuteEntryScalar<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default;
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    object? Result = Command.ExecuteScalar();
                    connection.Close();

                    if (Result != null)
                    {
                        Id = (G?)Result;
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", commandText, ex.ToString()));
                return default;
            }
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default;
                using (SqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    object? Result = await Command.ExecuteScalarAsync();
                    await connection.CloseAsync();

                    if (Result != null)
                    {
                        Id = (G?)Result;
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        private G? ExecuteEntryScalar<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction)
                {
                    CommandType = System.Data.CommandType.Text
                };
                ObjectToParameters(entry, Command.Parameters);
                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString()));
            }
            return default;
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction)
                {
                    CommandType = System.Data.CommandType.Text
                };
                ObjectToParameters(entry, Command.Parameters);
                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
            }
            return default;
        }
        #endregion

        #region Execution Methods

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (ExecuteEntry($"{commandStart}, {PK}) {commandEnd},@Id);", entry))
                {
                    return Id;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return ExecuteEntryScalar<T, G>($"{commandStart}) {insertedOutput} {commandEnd});", entry);
            }
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry,
            BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (ExecuteEntry($"{commandStart}, {PK}) {commandEnd}, @Id);", entry, bhTransaction, connectionIndex))
                {
                    return Id;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return ExecuteEntryScalar<T, G>($"{commandStart}) {insertedOutput} {commandEnd});", entry, bhTransaction, connectionIndex);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (await ExecuteEntryAsync($"{commandStart}, {PK}) {commandEnd}, @Id);", entry))
                {
                    return Id;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {insertedOutput} {commandEnd});", entry);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry,
            BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (await ExecuteEntryAsync($"{commandStart}, {PK}) {commandEnd}, @Id);", entry, bhTransaction, connectionIndex))
                {
                    return Id;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {insertedOutput} {commandEnd});", entry, bhTransaction, connectionIndex);
            }
        }

        public async Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries,
            BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            List<G?> Ids = new();
            if (useGenerator)
            {
                string commandText = $"{commandStart}, {PK}) {commandEnd}, @Id);";
                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                    if (await ExecuteEntryAsync(commandText, entry, bhTransaction, connectionIndex))
                    {
                        Ids.Add(Id);
                    }
                    else
                    {
                        Ids.Add(default);
                    }
                }
            }
            else
            {
                string commandText = $"{commandStart}) {insertedOutput} {commandEnd});";
                foreach (T entry in entries)
                {
                    Ids.Add(await ExecuteEntryScalarAsync<T, G>(commandText, entry, bhTransaction, connectionIndex));
                }
            }

            return Ids;
        }

        public List<G?> MultiInsertScalar<T, G>(string commandStart, string commandEnd, List<T> entries,
            BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            List<G?> Ids = new();
            if (useGenerator)
            {
                string commandText = $"{commandStart}, {PK}) {commandEnd}, @Id);";
                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                    if (ExecuteEntry(commandText, entry, bhTransaction, connectionIndex))
                    {
                        Ids.Add(Id);
                    }
                    else
                    {
                        Ids.Add(default);
                    }
                }
            }
            else
            {
                string commandText = $"{commandStart}) {insertedOutput} {commandEnd});";
                foreach (T entry in entries)
                {
                    Ids.Add(ExecuteEntryScalar<T, G>(commandText, entry, bhTransaction, connectionIndex));
                }
            }
            return Ids;
        }

        public bool ExecuteEntry<T>(string commandText, T entry)
        {
            try
            {
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry)
        {
            try
            {
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default;
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    object? Result = Command.ExecuteScalar();
                    connection.Close();

                    if (Result != null)
                    {
                        Id = (G?)Result;
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Scalar_{typeof(G).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Scalar_{typeof(G).Name}", ex.Message, ex.ToString()));
            }
            return default;
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default;
                using (SqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    object? Result = await Command.ExecuteScalarAsync();
                    await connection.CloseAsync();

                    if (Result != null)
                    {
                        Id = (G?)Result;
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"ScalarAsync_{typeof(G).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"ScalarAsync_{typeof(G).Name}", ex.Message, ex.ToString()));
            }
            return default;
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                object? Id = default;
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    Id = Command.ExecuteScalar();
                    connection.Close();
                }
                return Id;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("MSQL_RawScalar", ex.Message, ex.ToString()));
                return default;
            }
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                return Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("MSQL_RawScalar", ex.Message, ex.ToString()));
            }
            return default;
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                object? Id = default;
                using (SqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    Id = await Command.ExecuteScalarAsync();
                    await connection.CloseAsync();
                }
                return Id;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("MSQL_RawScalarAsync", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                return await Command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("MSQL_RawScalarAsync", ex.Message, ex.ToString()));
            }
            return default;
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("MSQL_Execute", ex.Message, ex.ToString()));
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("MSQL_Execute", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("MSQL_ExecuteAsync", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (SqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    await Command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("MSQL_ExecuteAsync", ex.Message, ex.ToString()));
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqlDataReader DataReader = Command.ExecuteReader())
                    {
                        while (DataReader.Read())
                        {
                            T? line = MapObject<T>(DataReader);

                            if (line != null)
                            {
                                result = line;
                                break;
                            }
                        }
                    }

                    connection.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqlDataReader DataReader = Command.ExecuteReader())
                    {
                        while (DataReader.Read())
                        {
                            T? line = MapObject<T>(DataReader);

                            if (line != null)
                            {
                                result.Add(line);
                            }
                        }
                    }
                    connection.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;
                using (SqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqlDataReader DataReader = await Command.ExecuteReaderAsync())
                    {
                        while (DataReader.Read())
                        {
                            T? line = MapObject<T>(DataReader);

                            if (line != null)
                            {
                                result = line;
                                break;
                            }
                        }
                    }
                    await connection.CloseAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (SqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqlDataReader DataReader = await Command.ExecuteReaderAsync())
                    {
                        while (DataReader.Read())
                        {
                            T? line = MapObject<T>(DataReader);

                            if (line != null)
                            {
                                result.Add(line);
                            }
                        }
                    }
                    await connection.CloseAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                T? result = default;
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqlDataReader DataReader = Command.ExecuteReader())
                {
                    while (DataReader.Read())
                    {
                        T? line = MapObject<T>(DataReader);

                        if (line != null)
                        {
                            result = line;
                            break;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                List<T> result = new();
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqlDataReader DataReader = Command.ExecuteReader())
                {
                    while (DataReader.Read())
                    {
                        T? line = MapObject<T>(DataReader);

                        if (line != null)
                        {
                            result.Add(line);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                T? result = default;
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqlDataReader DataReader = await Command.ExecuteReaderAsync())
                {
                    while (DataReader.Read())
                    {
                        T? line = MapObject<T>(DataReader);

                        if (line != null)
                        {
                            result = line;
                            break;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                List<T> result = new();
                SqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as SqlConnection;
                SqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as SqlTransaction;
                SqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqlDataReader DataReader = await Command.ExecuteReaderAsync())
                {
                    while (DataReader.Read())
                    {
                        T? line = MapObject<T>(DataReader);

                        if (line != null)
                        {
                            result.Add(line);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }
        #endregion

        #region Object Mapping
        private T? MapObject<T>(SqlDataReader reader)
        {
            try
            {
                Type type = typeof(T);

                if (type == typeof(string))
                {
                    if (reader.FieldCount > 0) return (T?)reader.GetValue(0);
                    else return default;
                }

                PropertyInfo[] properties = type.GetProperties();

                if (properties.Length == 0 && reader.FieldCount > 0)
                {
                    object? value = reader.GetValue(0);

                    if (value != null)
                    {
                        return (T?)value;
                    }

                    return default;
                }

                object? obj = Activator.CreateInstance(type);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        PropertyInfo? property = properties.FirstOrDefault(x => string.Equals(x.Name, reader.GetName(i), StringComparison.OrdinalIgnoreCase));
                        if (property != null)
                        {
                            type.GetProperty(property.Name)?.SetValue(obj, reader.GetValue(i));
                        }
                    }
                }
                return (T?)obj;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => ex.Message.CreateErrorLogs($"Object_Mapping_{typeof(T).Name}", "MapperError", ex.ToString()));
                return default;
            }
        }

        private void ArrayToParameters(List<BlackHoleParameter>? bhParameters, SqlParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    if(param.Value != null)
                    {
                        parameters.Add(new SqlParameter(param.Name, param.Value));
                    }
                    else
                    {
                        parameters.Add(new SqlParameter(param.Name, DBNull.Value));
                    }
                }
            }
        }

        private void ObjectToParameters<T>(T item, SqlParameterCollection parameters)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(item);

                if(value != null)
                {
                    parameters.Add(new SqlParameter(@property.Name, value));
                }
                else
                {
                    parameters.Add(new SqlParameter(@property.Name, DBNull.Value));
                }
            }
        }

        private G? GenerateId<G>()
        {
            string ToHash = Guid.NewGuid().ToString() + DateTime.Now.ToString();

            object? value = ToHash.GenerateSHA1();

            return (G?)value;
        }
        #endregion
    }
}
