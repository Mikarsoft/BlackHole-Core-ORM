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
        private readonly ILoggerService _loggerService;
        private readonly string _connectionString;

        internal MySqlExecutionProvider(string connectionString)
        {
            _connectionString = connectionString;
            _loggerService = new LoggerService();
        }
        #endregion

        #region Execution Methods
        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default(G);
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
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
                _loggerService.CreateErrorLogs("Scalar", ex.Message, ex.ToString());
                return default(G);
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                object? Result = Command.ExecuteScalar();
                
                if(Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"Scalar", ex.Message, ex.ToString());
            }

            return default(G);
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default(G);
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
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
                _loggerService.CreateErrorLogs($"Scalar", ex.Message, ex.ToString());
                return default(G);
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                object? Result = await Command.ExecuteScalarAsync();

                if(Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("Scalar", ex.Message, ex.ToString());
            }

            return default(G);
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    Command.ExecuteNonQuery();
                    connection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("Execute", ex.Message, ex.ToString());
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);

                ArrayToParameters(parameters, Command.Parameters);

                Command.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("Execute", ex.Message, ex.ToString());
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    await Command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString());
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                await Command.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString());
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default(T);

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
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
                _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString());
                return default(T);
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                MySqlConnection? connection = bHTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bHTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
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
                _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString());
                return default(T);
            }
        }

        public List<T> Query<T>(string command, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new MySqlCommand(command, connection);
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
                _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString());
                return new List<T>();
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new List<T>();

                MySqlConnection? connection = bHTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bHTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
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
                _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString());
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string command, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default(T);

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new MySqlCommand(command, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (DbDataReader DataReader = await Command.ExecuteReaderAsync())
                    {
                        while (await DataReader.ReadAsync())
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
                _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString());
                return default(T);
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                MySqlConnection? connection = bHTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bHTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (DbDataReader DataReader = await Command.ExecuteReaderAsync())
                {
                    while (await DataReader.ReadAsync())
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
                _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString());
                return default(T);
            }
        }

        public async Task<List<T>> QueryAsync<T>(string command, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new MySqlCommand(command, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (DbDataReader DataReader = await Command.ExecuteReaderAsync())
                    {
                        while (await DataReader.ReadAsync())
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
                _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString());
                return new List<T>();
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new List<T>();

                MySqlConnection? connection = bHTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bHTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (DbDataReader DataReader = await Command.ExecuteReaderAsync())
                {
                    while (await DataReader.ReadAsync())
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
                _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString());
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
                PropertyInfo[] properties = type.GetProperties();
                object? obj = Activator.CreateInstance(type);

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

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        string propertyName = reader.GetName(i);

                        if (properties.Any(m => string.Equals(m.Name, propertyName, StringComparison.OrdinalIgnoreCase)))
                        {
                            PropertyInfo property = properties.Where(x => x.Name == propertyName).First();

                            if (property.PropertyType == typeof(Guid))
                            {
                                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, reader.GetGuid(i));
                            }
                            else
                            {
                                object? propValue = Convert.ChangeType(reader.GetValue(i), property.PropertyType);
                                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, propValue);
                            }
                        }
                    }
                }
                return (T?)obj;
            }
            catch (Exception ex)
            {
                throw new Exception($"Object Mapping:{ex.Message}");
            }
        }

        private T? MapObject<T>(DbDataReader reader)
        {
            try
            {
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                object? obj = Activator.CreateInstance(type);

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

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        string propertyName = reader.GetName(i);

                        if (properties.Any(m => string.Equals(m.Name, propertyName, StringComparison.OrdinalIgnoreCase)))
                        {
                            PropertyInfo property = properties.Where(x => x.Name == propertyName).First();

                            if (property.PropertyType == typeof(Guid))
                            {
                                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, reader.GetGuid(i));
                            }
                            else
                            {
                                object? propValue = Convert.ChangeType(reader.GetValue(i), property.PropertyType);
                                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, propValue);
                            }
                        }
                    }
                }
                return (T?)obj;
            }
            catch (Exception ex)
            {
                throw new Exception($"Object Mapping:{ex.Message}");
            }
        }

        private void ArrayToParameters(List<BlackHoleParameter>? bhParameters, MySqlParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;

                    if (value?.GetType() == typeof(Guid))
                    {
                        parameters.Add(new MySqlParameter(@param.Name, value.ToString()));
                    }
                    else
                    {
                        parameters.Add(new MySqlParameter(@param.Name, value));
                    }
                }
            }
        }
        #endregion
    }
}
