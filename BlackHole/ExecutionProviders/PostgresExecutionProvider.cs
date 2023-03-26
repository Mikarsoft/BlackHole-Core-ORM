﻿using BlackHole.CoreSupport;
using BlackHole.Logger;
using Npgsql;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.ExecutionProviders
{
    internal class PostgresExecutionProvider : IExecutionProvider
    {
        #region Constructor
        private readonly ILoggerService _loggerService;
        private readonly string _connectionString;

        internal PostgresExecutionProvider(string connectionString)
        {
            _connectionString = connectionString;
            _loggerService = new LoggerService();
        }
        #endregion

        #region ExecutionMethods
        public G? ExecuteScalar<G>(string commandText, BlackHoleParameter[]? parameters)
        {
            try
            {
                G? Id = default(G);
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    Id = (G?)Command.ExecuteScalar();
                    connection.Close();
                }
                return Id;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Scalar", ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        public G? ExecuteScalar<G>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);

                ArrayToParameters(parameters, Command.Parameters);

                return (G?)Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Scalar", ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, BlackHoleParameter[]? parameters)
        {
            try
            {
                G? Id = default(G);
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    Id = (G?)await Command.ExecuteScalarAsync();
                    await connection.CloseAsync();
                }
                return Id;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Scalar", ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);

                ArrayToParameters(parameters, Command.Parameters);

                return (G?)await Command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Scalar", ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        public bool JustExecute(string commandText, BlackHoleParameter[]? parameters)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);

                    ArrayToParameters(parameters, Command.Parameters);

                    Command.ExecuteNonQuery();
                    connection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Execute", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool JustExecute(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);

                ArrayToParameters(parameters, Command.Parameters);

                Command.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Execute", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);

                    ArrayToParameters(parameters, Command.Parameters);

                    await Command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);

                ArrayToParameters(parameters, Command.Parameters);

                await Command.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, BlackHoleParameter[]? parameters)
        {
            try
            {
                T? result = default(T);

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public T? QueryFirst<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);
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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public List<T> Query<T>(string command, BlackHoleParameter[]? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new NpgsqlCommand(command, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public List<T> Query<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new List<T>();

                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);
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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string command, BlackHoleParameter[]? parameters)
        {
            try
            {
                T? result = default(T);

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new NpgsqlCommand(command, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);

                if (parameters != null)
                {
                    Command.Parameters.AddRange(parameters);
                }

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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public async Task<List<T>> QueryAsync<T>(string command, BlackHoleParameter[]? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new NpgsqlCommand(command, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new List<T>();

                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);

                if (parameters != null)
                {
                    Command.Parameters.AddRange(parameters);
                }

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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
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
                PropertyInfo[] properties = type.GetProperties();
                object? obj = Activator.CreateInstance(type);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        string propertyName = reader.GetName(i);

                        if (properties.Any(m => string.Equals(m.Name, propertyName, StringComparison.OrdinalIgnoreCase)))
                        {
                            obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, reader.GetValue(i));
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

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        string propertyName = reader.GetName(i);

                        if (properties.Any(m => string.Equals(m.Name, propertyName, StringComparison.OrdinalIgnoreCase)))
                        {
                            obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, reader.GetValue(i));
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

        private void ArrayToParameters(BlackHoleParameter[]? bhParameters, NpgsqlParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;
                    parameters.Add(new NpgsqlParameter(@param.Name, value));
                }
            }
        }
        #endregion
    }
}
