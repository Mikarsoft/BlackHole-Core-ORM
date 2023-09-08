﻿using BlackHole.CoreSupport;
using BlackHole.Logger;
using Npgsql;
using System.Reflection;

namespace BlackHole.ExecutionProviders
{
    internal class PostgresExecutionProvider : IExecutionProvider
    {
        #region Constructor
        private readonly string _connectionString;
        internal readonly bool skipQuotes = false;

        internal PostgresExecutionProvider(string connectionString)
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    object? Result = Command.ExecuteScalar();

                    if (Result != null)
                    {
                        Id = (G?)Result;
                    }

                    connection.Close();
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
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (G?)Result;
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    object? Result = await Command.ExecuteScalarAsync();

                    if (Result != null)
                    {
                        Id = (G?)Result;
                    }

                    await connection.CloseAsync();
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
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Result;
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new(commandText, connection);
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
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new(commandText, connection);
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
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new(commandText, connection);
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
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new(commandText, connection);
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
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (NpgsqlDataReader DataReader = Command.ExecuteReader())
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFirst_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default;
                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (NpgsqlDataReader DataReader = Command.ExecuteReader())
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFirst_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (NpgsqlDataReader DataReader = Command.ExecuteReader())
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
                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (NpgsqlDataReader DataReader = Command.ExecuteReader())
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (NpgsqlDataReader DataReader = await Command.ExecuteReaderAsync())
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default;
                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (NpgsqlDataReader DataReader = await Command.ExecuteReaderAsync())
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (NpgsqlDataReader DataReader = await Command.ExecuteReaderAsync())
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
                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (NpgsqlDataReader DataReader = await Command.ExecuteReaderAsync())
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

        #region ObjectMapping

        private T? MapObject<T>(NpgsqlDataReader reader)
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

        private void ArrayToParameters(List<BlackHoleParameter>? bhParameters, NpgsqlParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;

                    if(value != null)
                    {
                        parameters.Add(new NpgsqlParameter(@param.Name, value));
                    }
                    else
                    {
                        parameters.Add(new NpgsqlParameter(@param.Name, DBNull.Value));
                    }
                }
            }
        }
        #endregion
    }
}
