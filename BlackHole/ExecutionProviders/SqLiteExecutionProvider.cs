using BlackHole.CoreSupport;
using BlackHole.Logger;
using Microsoft.Data.Sqlite;
using System.Reflection;

namespace BlackHole.ExecutionProviders
{
    internal class SqLiteExecutionProvider : IExecutionProvider
    {
        #region Constructor
        private readonly ILoggerService _loggerService;
        private readonly string _connectionString;
        internal readonly bool skipQuotes = true;

        internal SqLiteExecutionProvider(string connectionString)
        {
            _connectionString = connectionString;
            _loggerService = new LoggerService();
        }

        public bool SkipQuotes()
        {
            return skipQuotes;
        }
        #endregion

        #region ExecutionMethods
        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default;
                using (SqliteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    object? Result = Command.ExecuteScalar();
                    connection.Close();

                    if(Result != null)
                    {
                        Id = (G?)Convert.ChangeType(Result, typeof(G));
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Scalar", commandText, ex.Message, ex.ToString())).Start();
                return default;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                SqliteConnection? connection = bhTransaction.connection as SqliteConnection;
                SqliteTransaction? transaction = bhTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Scalar", commandText, ex.Message, ex.ToString())).Start();
            }
            return default;
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default;
                using (SqliteConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqliteCommand Command = new(commandText, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs("ScalarAsync", commandText, ex.Message, ex.ToString())).Start();
                return default;
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                SqliteConnection? connection = bhTransaction.connection as SqliteConnection;
                SqliteTransaction? transaction = bhTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("ScalarAsync", commandText, ex.Message, ex.ToString())).Start();
            }
            return default;
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (SqliteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("NonQuery", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                SqliteConnection? connection = bhTransaction.connection as SqliteConnection;
                SqliteTransaction? transaction = bhTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("NonQuery", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (SqliteConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    await Command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("NonQueryAsync", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                SqliteConnection? connection = bhTransaction.connection as SqliteConnection;
                SqliteTransaction? transaction = bhTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("NonQueryAsync", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;

                using (SqliteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqliteDataReader DataReader = Command.ExecuteReader())
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryFirst_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default;

                SqliteConnection? connection = bHTransaction.connection as SqliteConnection;
                SqliteTransaction? transaction = bHTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqliteDataReader DataReader = Command.ExecuteReader())
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryFirst_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();

                using (SqliteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqliteDataReader DataReader = Command.ExecuteReader())
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
                new Thread(() => _loggerService.CreateErrorLogs($"Query_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new();

                SqliteConnection? connection = bHTransaction.connection as SqliteConnection;
                SqliteTransaction? transaction = bHTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqliteDataReader DataReader = Command.ExecuteReader())
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
                new Thread(() => _loggerService.CreateErrorLogs($"Query_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;

                using (SqliteConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqliteDataReader DataReader = await Command.ExecuteReaderAsync())
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryFirstAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default;
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default;

                SqliteConnection? connection = bHTransaction.connection as SqliteConnection;
                SqliteTransaction? transaction = bHTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqliteDataReader DataReader = await Command.ExecuteReaderAsync())
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryFirstAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();

                using (SqliteConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqliteDataReader DataReader = await Command.ExecuteReaderAsync())
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new();

                SqliteConnection? connection = bHTransaction.connection as SqliteConnection;
                SqliteTransaction? transaction = bHTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqliteDataReader DataReader = await Command.ExecuteReaderAsync())
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }
        #endregion

        #region ObjectMapping

        private T? MapObject<T>(SqliteDataReader reader)
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
                        string propertyName = reader.GetName(i);
                        PropertyInfo? property = properties.Where(x => string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                        if (property != null)
                        {
                            if (property.PropertyType == typeof(Guid))
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, reader.GetGuid(i));
                            }
                            else
                            {
                                object? propValue = Convert.ChangeType(reader.GetValue(i), property.PropertyType);
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, propValue);
                            }
                        }
                    }
                }
                return (T?)obj;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Object Mapping {typeof(T).Name}", "", ex.Message, ex.ToString())).Start();
                return default;
            }
        }

        private static void ArrayToParameters(List<BlackHoleParameter>? bhParameters, SqliteParameterCollection parameters)
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
                            parameters.Add(new SqliteParameter(@param.Name, value.ToString()));
                        }
                        else
                        {
                            parameters.Add(new SqliteParameter(@param.Name, value));
                        }
                    }
                    else
                    {
                        parameters.Add(new SqliteParameter(@param.Name, DBNull.Value));
                    }
                }
            }
        }
        #endregion
    }
}
