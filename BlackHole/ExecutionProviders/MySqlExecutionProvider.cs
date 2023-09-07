using BlackHole.CoreSupport;
using BlackHole.Logger;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.ExecutionProviders
{
    internal class MySqlExecutionProvider : IExecutionProvider
    {
        #region Constructor
        private readonly string _connectionString;
        internal readonly bool skipQuotes = true;

        internal MySqlExecutionProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool SkipQuotes()
        {
            return skipQuotes;
        }
        #endregion

        #region Execution Methods
        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default;
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    object? Result = Command.ExecuteScalar();
                    connection.Close();

                    if (Result != null)
                    {
                        Id = (G?)Convert.ChangeType(Result, typeof(G));
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("Scalar", ex.Message, ex.ToString()));
                return default;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = Command.ExecuteScalar();
                
                if(Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("Scalar", ex.Message, ex.ToString()));
            }

            return default;
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default;
                using (MySqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    object? Result = await Command.ExecuteScalarAsync();
                    await connection.CloseAsync();

                    if (Result != null)
                    {
                        Id = (G?)Convert.ChangeType(Result, typeof(G));
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("ScalarAsync", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = await Command.ExecuteScalarAsync();

                if(Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("ScalarAsync", ex.Message, ex.ToString()));
            }
            return default;
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                object? Id = default;
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    Id = Command.ExecuteScalar();
                    connection.Close();
                }
                return Id;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("RawScalar", ex.Message, ex.ToString()));
                return default;
            }
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                return Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("RawScalar", ex.Message, ex.ToString()));
            }
            return default;
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                object? Id = default;
                using (MySqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    Id = await Command.ExecuteScalarAsync();
                    await connection.CloseAsync();
                }
                return Id;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("RawScalarAsync", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                return await Command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("RawScalarAsync", ex.Message, ex.ToString()));
            }
            return default;
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("NonQuery", ex.Message, ex.ToString()));
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("NonQuery", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (MySqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    await Command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("NonQueryAsync", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("NonQueryAsync", ex.Message, ex.ToString()));
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (MySqlDataReader DataReader = Command.ExecuteReader())
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFrist_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default;
                MySqlConnection? connection = bHTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bHTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (MySqlDataReader DataReader = Command.ExecuteReader())
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFrist_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (MySqlDataReader DataReader = Command.ExecuteReader())
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Query_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new();
                MySqlConnection? connection = bHTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bHTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (MySqlDataReader DataReader = Command.ExecuteReader())
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Query_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;
                using (MySqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (DbDataReader DataReader = await Command.ExecuteReaderAsync())
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFristAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default;
                MySqlConnection? connection = bHTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bHTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (DbDataReader DataReader = await Command.ExecuteReaderAsync())
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFristAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (MySqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (DbDataReader DataReader = await Command.ExecuteReaderAsync())
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new();
                MySqlConnection? connection = bHTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bHTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (DbDataReader DataReader = await Command.ExecuteReaderAsync())
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }
        #endregion

        #region Object Mapping

        private T? MapObject<T>(MySqlDataReader reader)
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
                    if (typeof(T) == typeof(Guid))
                    {
                        object? GValue = reader.GetGuid(0);
                        return (T?)GValue;
                    }

                    object? value = reader.GetValue(0);

                    if (value != null)
                    {
                        return (T?)Convert.ChangeType(value, typeof(T));
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
                            if (property.PropertyType == typeof(Guid))
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, reader.GetGuid(i));
                            }
                            else
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, Convert.ChangeType(reader.GetValue(i), property.PropertyType));
                            }
                        }
                    }
                }
                return (T?)obj;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => ex.Message.CreateErrorLogs($"Object Mapping {typeof(T).Name}", "MapperError", ex.ToString()));
                return default;
            }
        }

        private T? MapObject<T>(DbDataReader reader)
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
                    if (typeof(T) == typeof(Guid))
                    {
                        object? GValue = reader.GetGuid(0);
                        return (T?)GValue;
                    }

                    object? value = reader.GetValue(0);

                    if (value != null)
                    {
                        return (T?)Convert.ChangeType(value, typeof(T));
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
                            if (property.PropertyType == typeof(Guid))
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, reader.GetGuid(i));
                            }
                            else
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, Convert.ChangeType(reader.GetValue(i), property.PropertyType));
                            }
                        }
                    }
                }
                return (T?)obj;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => ex.Message.CreateErrorLogs($"Object Mapping {typeof(T).Name}", "MapperError", ex.ToString()));
                return default;
            }
        }

        private void ArrayToParameters(List<BlackHoleParameter>? bhParameters, MySqlParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;

                    if(value != null)
                    {
                        if (value?.GetType() == typeof(Guid))
                        {
                            parameters.Add(new MySqlParameter(@param.Name, value.ToString()));
                        }
                        else
                        {
                            parameters.Add(new MySqlParameter(@param.Name, value));
                        }
                    }
                    else
                    {
                        parameters.Add(new MySqlParameter(@param.Name, DBNull.Value));
                    }
                }
            }
        }
        #endregion
    }
}
