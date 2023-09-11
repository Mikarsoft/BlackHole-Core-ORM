﻿using BlackHole.CoreSupport;
using BlackHole.Logger;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.ExecutionProviders
{
    internal class OracleExecutionProvider : IExecutionProvider
    {
        #region Constructor
        private readonly string _connectionString;
        internal readonly bool skipQuotes = false;

        internal OracleExecutionProvider(string connectionString)
        {
            _connectionString = connectionString;
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
                using (OracleConnection connection = new(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("Scalar", ex.Message, ex.ToString()));
                return default;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    BindByName = true,
                    Transaction = transaction
                };
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Scalar", ex.Message, ex.ToString()));
            }
            return default;
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default;
                using (OracleConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"ScalarAsync", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    Transaction = transaction,
                    BindByName = true
                };
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("ScalarAsync", ex.Message, ex.ToString()));
            }
            return default;
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                object? Id = default;
                using (OracleConnection connection = new(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
                    ArrayToParameters(parameters, Command.Parameters);
                    Command.Parameters.Add(new OracleParameter("OracleReturningValue", 1));
                    Command.ExecuteScalar();
                    int paramIndex = Command.Parameters.IndexOf("OracleReturningValue");
                    if (paramIndex > -1)
                    {
                        Id = Command.Parameters[paramIndex].Value;
                    }
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
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    BindByName = true,
                    Transaction = transaction
                };
                ArrayToParameters(parameters, Command.Parameters);
                Command.Parameters.Add(new OracleParameter("OracleReturningValue", 1));
                Command.ExecuteScalar();
                int paramIndex = Command.Parameters.IndexOf("OracleReturningValue");
                if (paramIndex > -1)
                {
                    return (int)Command.Parameters[paramIndex].Value;
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"RawScalar", ex.Message, ex.ToString()));
            }
            return default;
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                object? Id = default;
                using (OracleConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
                    ArrayToParameters(parameters, Command.Parameters);
                    Command.Parameters.Add(new OracleParameter("OracleReturningValue", 1));
                    await Command.ExecuteScalarAsync();
                    int paramIndex = Command.Parameters.IndexOf("OracleReturningValue");
                    if(paramIndex > -1)
                    {
                        Id = (int)Command.Parameters[paramIndex].Value;
                    }
                    await connection.CloseAsync();
                }
                return Id;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"RawScalarAsync", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    Transaction = transaction,
                    BindByName = true
                };
                ArrayToParameters(parameters, Command.Parameters);
                Command.Parameters.Add(new OracleParameter("OracleReturningValue", 1));
                await Command.ExecuteScalarAsync();
                int paramIndex = Command.Parameters.IndexOf("OracleReturningValue");
                if (paramIndex > -1)
                {
                    return (int)Command.Parameters[paramIndex].Value;
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("RawScalarAsync", ex.Message, ex.ToString()));
            }
            return default;
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (OracleConnection connection = new(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
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
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    Transaction = transaction,
                    BindByName = true
                };
                ArrayToParameters(parameters, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("NonQuery", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (OracleConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
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
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    Transaction = transaction,
                    BindByName = true
                };
                ArrayToParameters(parameters, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("NonQueryAsync", ex.Message, ex.ToString()));
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;
                using (OracleConnection connection = new(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFrist_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default;
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    Transaction = transaction,
                    BindByName = true
                };
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
                bHTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFrist_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (OracleConnection connection = new(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Query_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new();
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    Transaction = transaction,
                    BindByName = true
                };
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
                bHTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Query_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;
                using (OracleConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
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
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    Transaction = transaction,
                    BindByName = true
                };
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
                bHTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFristAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (OracleConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                    {
                        BindByName = true
                    };
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
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new(commandText.Replace("@", ":"), connection)
                {
                    Transaction = transaction,
                    BindByName = true
                };
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
                bHTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
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
                        PropertyInfo? property = properties.FirstOrDefault(x => string.Equals(x.Name, reader.GetName(i), StringComparison.OrdinalIgnoreCase));
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(Guid))
                            {
                                type.GetProperty(property.Name)?.SetValue(obj, GuidParser(reader.GetString(i)));
                            }
                            else if (property.PropertyType == typeof(bool))
                            {
                                type.GetProperty(property.Name)?.SetValue(obj, reader.GetBoolean(i));
                            }
                            else
                            {
                                type.GetProperty(property.Name)?.SetValue(obj, Convert.ChangeType(reader.GetValue(i), property.PropertyType));
                            }
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
                        PropertyInfo? property = properties.FirstOrDefault(x => string.Equals(x.Name, reader.GetName(i), StringComparison.OrdinalIgnoreCase));

                        if (property != null)
                        {
                            if (property.PropertyType == typeof(Guid))
                            {
                                type.GetProperty(property.Name)?.SetValue(obj, GuidParser(reader.GetString(i)));
                            }
                            else if(property.PropertyType == typeof(bool))
                            {
                                type.GetProperty(property.Name)?.SetValue(obj, reader.GetBoolean(i));
                            }
                            else
                            {
                                type.GetProperty(property.Name)?.SetValue(obj, Convert.ChangeType(reader.GetValue(i), property.PropertyType));
                            }
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

                    if(value != null)
                    {
                        Type Vtype = value.GetType();
                        if (Vtype == typeof(Guid))
                        {
                            parameters.Add(new OracleParameter(param.Name, value.ToString()));
                        }
                        else if (Vtype == typeof(bool))
                        {
                            parameters.Add(new OracleParameter(param.Name, (bool)value ? 1 : 0));
                        }
                        else
                        {
                            parameters.Add(new OracleParameter(param.Name, value));
                        }
                    }
                    else
                    {
                        parameters.Add(new OracleParameter(param.Name, DBNull.Value));
                    }
                }
            }
        }
        #endregion
    }
}
