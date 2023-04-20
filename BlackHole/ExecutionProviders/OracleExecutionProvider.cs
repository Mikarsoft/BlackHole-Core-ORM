using BlackHole.CoreSupport;
using BlackHole.Logger;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.ExecutionProviders
{
    internal class OracleExecutionProvider : IExecutionProvider
    {
        #region Constructor
        private readonly ILoggerService _loggerService;
        private readonly string _connectionString;

        internal OracleExecutionProvider(string connectionString)
        {
            _connectionString = connectionString;
            _loggerService = new LoggerService();
        }
        #endregion

        #region ExecutionMethods
        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default(G);
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                    Command.BindByName = true;
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
                new Thread(() => _loggerService.CreateErrorLogs("Scalar", commandText, ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                Command.BindByName = true;
                Command.Transaction = transaction;

                ArrayToParameters(parameters, Command.Parameters);

                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Scalar", commandText, ex.Message, ex.ToString())).Start();
            }
            return default(G);
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default(G);
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                    Command.BindByName = true;
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
                new Thread(() => _loggerService.CreateErrorLogs($"ScalarAsync", commandText, ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                Command.Transaction = transaction;
                Command.BindByName = true;
                ArrayToParameters(parameters, Command.Parameters);

                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("ScalarAsync", commandText, ex.Message, ex.ToString())).Start();
            }
            return default(G);
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                    Command.BindByName = true;
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
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                Command.Transaction = transaction;
                Command.BindByName = true;
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
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                    Command.BindByName = true;
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
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                Command.Transaction = transaction;
                Command.BindByName = true;
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
                T? result = default(T);

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText.Replace("@",":"), connection);
                    Command.BindByName = true;
                    ArrayToParameters(parameters, Command.Parameters);

                    using (OracleDataReader DataReader = Command.ExecuteReader())
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryFrist_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                Command.Transaction = transaction;
                Command.BindByName = true;
                ArrayToParameters(parameters, Command.Parameters);

                using (OracleDataReader DataReader = Command.ExecuteReader())
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryFrist_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                    Command.BindByName = true;
                    ArrayToParameters(parameters, Command.Parameters);

                    using (OracleDataReader DataReader = Command.ExecuteReader())
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
                List<T> result = new List<T>();

                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                Command.Transaction = transaction;
                Command.BindByName = true;
                ArrayToParameters(parameters, Command.Parameters);

                using (OracleDataReader DataReader = Command.ExecuteReader())
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
                T? result = default(T);

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                    Command.BindByName = true;
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryFristAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                Command.Transaction = transaction;
                Command.BindByName = true;
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryFristAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                    Command.BindByName = true;
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new List<T>();

                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText.Replace("@", ":"), connection);
                Command.Transaction = transaction;
                Command.BindByName = true;
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
                new Thread(() => _loggerService.CreateErrorLogs($"QueryAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }
        #endregion

        #region ObjectMapping

        private T? MapObject<T>(OracleDataReader reader)
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
                        object? GValue = GuidParser(reader.GetString(0));
                        return (T?)GValue;
                    }

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
                        string propertyName = reader.GetName(i);

                        PropertyInfo? property = properties.Where(x => string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                        if (property != null)
                        {
                            if (property.PropertyType == typeof(Guid))
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, GuidParser(reader.GetString(i)));
                            }
                            else if (property.PropertyType == typeof(bool))
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, reader.GetBoolean(i));
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
                        object? GValue = GuidParser(reader.GetString(0));
                        return (T?)GValue;
                    }

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
                        string propertyName = reader.GetName(i);
                        PropertyInfo? property = properties.Where(x => string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                        if (property != null)
                        {
                            if (property.PropertyType == typeof(Guid))
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, GuidParser(reader.GetString(i)));
                            }
                            else if(property.PropertyType == typeof(bool))
                            {
                                obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, reader.GetBoolean(i));
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

        private Guid GuidParser(string guid)
        {
            try
            {
                return Guid.Parse(guid);
            }
            catch
            {
                return Guid.Empty;
            }
        }

        private void ArrayToParameters(List<BlackHoleParameter>? bhParameters, OracleParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;

                    if (value?.GetType() == typeof(Guid))
                    {
                        parameters.Add(new OracleParameter(param.Name, value.ToString()));
                    }
                    else if (value?.GetType() == typeof(bool))
                    {
                        parameters.Add(new OracleParameter(param.Name, (bool)value ? 1 : 0));
                    }
                    else
                    {
                        parameters.Add(new OracleParameter(param.Name, value));
                    }
                }
            }
        }
        #endregion
    }
}
