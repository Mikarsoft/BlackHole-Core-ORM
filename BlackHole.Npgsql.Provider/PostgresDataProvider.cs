﻿using Mikarsoft.BlackHoleCore.Connector;
using Npgsql;
using System.Data;
using System.Reflection;

namespace Mikarsoft.BlackHoleCore.Npgsql
{
    public class PostgresDataProvider : IBHDataProvider
    {
        #region Ctor

        private readonly IBlackHoleLogger _logger;

        private readonly string _connectionString;

        internal PostgresDataProvider(string connectionString, IBlackHoleLogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        #endregion

        #region Internal Processes
        private G? ExecuteEntryScalar<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default;
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new(commandText, connection);
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
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Insert_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default;
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new(commandText, connection);
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
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        private G? ExecuteEntryScalar<T, G>(string commandText, T entry, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                bHTransaction.SetError(true);
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Insert_{typeof(T).Name}", ex.Message, ex.ToString()));
            }
            return default;
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                bHTransaction.SetError(true);
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
            }
            return default;
        }
        #endregion

        #region Execution Methods

        public IDbConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (ExecuteEntry($@"{commandStart},""Id"") {commandEnd},@Id);", entry))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd}) {insertedOutput};", entry);
            }
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (ExecuteEntry($@"{commandStart},""Id"") {commandEnd},@Id);", entry, bhTransaction, connectionIndex))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd}) {insertedOutput};", entry, bhTransaction, connectionIndex);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (await ExecuteEntryAsync($@"{commandStart},""Id"") {commandEnd},@Id);", entry))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd}) {insertedOutput};", entry);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (await ExecuteEntryAsync($@"{commandStart},""Id"") {commandEnd},@Id);", entry, bhTransaction, connectionIndex))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd}) {insertedOutput};", entry, bhTransaction, connectionIndex);
            }
        }

        public async Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, IBHInnerTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            List<G?> Ids = new();
            if (useGenerator)
            {
                string commandText = $@"{commandStart},""Id"") {commandEnd},@Id);";
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
                string commandText = $"{commandStart}) {commandEnd}) {insertedOutput};";
                foreach (T entry in entries)
                {
                    Ids.Add(await ExecuteEntryScalarAsync<T, G>(commandText, entry, bhTransaction, connectionIndex));
                }
            }
            return Ids;
        }

        public List<G?> MultiInsertScalar<T, G>(string commandStart, string commandEnd, List<T> entries, IBHInnerTransaction bhTransaction,
            bool useGenerator, string insertedOutput, int connectionIndex)
        {
            List<G?> Ids = new();
            if (useGenerator)
            {
                string commandText = $@"{commandStart},""Id"") {commandEnd},@Id);";
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
                string commandText = $"{commandStart}) {commandEnd}) {insertedOutput};";
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
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Insert_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry)
        {
            try
            {
                using (NpgsqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public bool ExecuteEntry<T>(string commandText, T entry, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                bHTransaction.SetError(true);
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Insert_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                bHTransaction.SetError(true);
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Scalar_{typeof(G).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
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
                bHTransaction.SetError(true);
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Scalar_{typeof(G).Name}", ex.Message, ex.ToString()));
            }
            return default;
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"ScalarAsync_{typeof(G).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
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
                bHTransaction.SetError(true);
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"ScalarAsync_{typeof(G).Name}", ex.Message, ex.ToString()));
            }
            return default;
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, "Pg_RawScalar", ex.Message, ex.ToString()));
                return default;
            }
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                return Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                bHTransaction.SetError(true);
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, "Pg_RawScalar", ex.Message, ex.ToString()));
            }
            return default;
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, "Pg_RawScalarAsync", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                return await Command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                bHTransaction.SetError(true);
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, "Pg_RawScalarAsync", ex.Message, ex.ToString()));
            }
            return default;
        }

        public bool JustExecute(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                bHTransaction.SetError(true);
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Pg_Execute", ex.Message, ex.ToString()));
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Pg_Execute", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
                NpgsqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.SetError(true);
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, "Pg_ExecuteAsync", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, "Pg_ExecuteAsync", ex.Message, ex.ToString()));
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Select_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"SelectFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters)
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
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                T? result = default;
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
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
                bHTransaction.SetError(true);
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                List<T> result = new();
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
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
                bHTransaction.SetError(true);
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"Select_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                T? result = default;
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
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
                bHTransaction.SetError(true);
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"SelectFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleInnerParameter>? parameters, IBHInnerTransaction bHTransaction, int connectionIndex)
        {
            try
            {
                List<T> result = new();
                NpgsqlConnection? connection = bHTransaction.GetConnection(connectionIndex) as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction.GetTransaction(connectionIndex) as NpgsqlTransaction;
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
                bHTransaction.SetError(true);
                await Task.Factory.StartNew(() => _logger.CreateErrorLogs(commandText, $"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }
        #endregion

        #region Object Mapping
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
                Task.Factory.StartNew(() => _logger.CreateErrorLogs(ex.Message, $"Object_Mapping_{typeof(T).Name}", "MapperError", ex.ToString()));
                return default;
            }
        }

        private void ArrayToParameters(List<BlackHoleInnerParameter>? bhParameters, NpgsqlParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleInnerParameter param in bhParameters)
                {
                    object? value = param.Value;

                    if (value != null)
                    {
                        parameters.Add(new NpgsqlParameter(param.Name, value));
                    }
                    else
                    {
                        parameters.Add(new NpgsqlParameter(param.Name, DBNull.Value));
                    }
                }
            }
        }

        private void ObjectToParameters<T>(T item, NpgsqlParameterCollection parameters)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(item);

                if (value != null)
                {
                    parameters.Add(new NpgsqlParameter(@property.Name, value));
                }
                else
                {
                    parameters.Add(new NpgsqlParameter(@property.Name, DBNull.Value));
                }
            }
        }

        private G? GenerateId<G>()
        {
            string ToHash = Guid.NewGuid().ToString() + DateTime.Now.ToString();

            object? value = _logger.GenerateSHA1(ToHash);

            return (G?)value;
        }
        #endregion
    }
}
