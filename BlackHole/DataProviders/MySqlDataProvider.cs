﻿using BlackHole.Engine;
using BlackHole.Logger;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.DataProviders
{
    internal class MySqlDataProvider : IDataProvider
    {
        #region Ctor

        private readonly string _connectionString;

        internal MySqlDataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        #endregion

        #region Internal Processes
        private G? ExecuteEntryScalar<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default;
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    object? Result = Command.ExecuteScalar();
                    connection.Close();

                    if (Result != null)
                    {
                        Id = (G?)Convert.ChangeType(Result, typeof(G));
                    }
                }
                return Id;
            }
            catch(Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message,ex.ToString()));
                return default;
            }
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default;
                using (MySqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    object? Result = await Command.ExecuteScalarAsync();
                    await connection.CloseAsync();

                    if (Result != null)
                    {
                        Id = (G?)Convert.ChangeType(Result, typeof(G));
                    }
                }
                return Id;
            }
            catch(Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{typeof(T)}", ex.Message, ex.ToString()));
                return default;
            }
        }

        private G? ExecuteEntryScalar<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                object? Result = Command.ExecuteScalar();

                if(Result!= null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch(Exception ex)
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
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch(Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
            }

            return default;
        }
        #endregion

        #region Execution Methods

        IDbConnection IDataProvider.GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        G? IDataProvider.InsertScalar<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput) where G : default
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);
                if (ExecuteEntry($"{commandStart}, Id) {commandEnd}, @Id);", entry))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd});{insertedOutput}", entry);
            }
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry,
            BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);
                if (ExecuteEntry($"{commandStart}, Id) {commandEnd}, @Id);", entry, bhTransaction, connectionIndex))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd}); {insertedOutput}", entry, bhTransaction, connectionIndex);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, bool useGenerator, string insertedOutput)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);
                if (await ExecuteEntryAsync($"{commandStart}, Id) {commandEnd}, @Id);", entry))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd});{insertedOutput}", entry);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry,
            BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);
                if (await ExecuteEntryAsync($"{commandStart}, Id) {commandEnd}, @Id);", entry, bhTransaction, connectionIndex))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd});{insertedOutput}", entry, bhTransaction, connectionIndex);
            }
        }

        public async Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries,
            BlackHoleTransaction bhTransaction, bool useGenerator, string insertedOutput, int connectionIndex)
        {
            List<G?> Ids = new();
            if (useGenerator)
            {
                string commandText = $"{commandStart}, Id) {commandEnd}, @Id);";
                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

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
                string commandText = $"{commandStart}) {commandEnd}); {insertedOutput}";
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
                string commandText = $"{commandStart}, Id) {commandEnd}, @Id);";
                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

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
                string commandText = $"{commandStart}) {commandEnd}); {insertedOutput}";

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
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch(Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry)
        {
            try
            {
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return true;
            }
            catch(Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InserAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
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
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch(Exception ex)
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Scalar_{typeof(G).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"ScalarAsync_{typeof(G).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
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
                using (MySqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    Id = Command.ExecuteScalar();
                    connection.Close();
                }
                return IntOrLong(Id);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("MySql_RawScalar", ex.Message, ex.ToString()));
                return default;
            }
        }

        public object? ExecuteRawScalar(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                return IntOrLong(Command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("MySql_RawScalar", ex.Message, ex.ToString()));
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
                return IntOrLong(Id);
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("MySql_RawScalarAsync", ex.Message, ex.ToString()));
                return default;
            }
        }

        public async Task<object?> ExecuteRawScalarAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                return IntOrLong(await Command.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("MySql_RawScalarAsync", ex.Message, ex.ToString()));
            }
            return default;
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("MySql_Execute", ex.Message, ex.ToString()));
                return false;
            }
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
            catch(Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("MySql_Execute", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
                MySqlCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch(Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("MySql_ExecuteAsync", ex.Message, ex.ToString()));
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
            catch(Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs("MySql_ExecuteAsync", ex.Message, ex.ToString()));
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
            catch(Exception ex)
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
            catch(Exception ex)
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
            catch(Exception ex)
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction, int connectionIndex)
        {
            try
            {
                T? result = default;
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
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
            catch(Exception ex)
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
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
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
            catch(Exception ex)
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
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
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
            catch(Exception ex)
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
                MySqlConnection? connection = bhTransaction.GetConnection(connectionIndex) as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction.GetTransaction(connectionIndex) as MySqlTransaction;
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
            catch(Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(()=> commandText.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
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
                                type.GetProperty(property.Name)?.SetValue(obj, reader.GetGuid(i));
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
            catch(Exception ex)
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
                    if(typeof(T)== typeof(Guid))
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
                                type.GetType()?.GetProperty(property.Name)?.SetValue(obj, reader.GetGuid(i));
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
            catch(Exception ex)
            {
                Task.Factory.StartNew(() => ex.Message.CreateErrorLogs($"Object_Mapping_{typeof(T).Name}", "MapperError", ex.ToString()));
                return default;
            }
        }

        private void ArrayToParameters(List<BlackHoleParameter>? bhParameters , MySqlParameterCollection parameters)
        {
            if(bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;

                    if(value != null)
                    {
                        if (value.GetType() == typeof(Guid))
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

        private void ObjectToParameters<T>(T item, MySqlParameterCollection parameters)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach(PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(item);

                if(value != null)
                {
                    if (value?.GetType() == typeof(Guid))
                    {
                        parameters.Add(new MySqlParameter(@property.Name, value.ToString()));
                    }
                    else
                    {
                        parameters.Add(new MySqlParameter(@property.Name, value));
                    }
                }
                else
                {
                    parameters.Add(new MySqlParameter(@property.Name, DBNull.Value));
                }
            }
        }

        private G? GenerateId<G>()
        {
            object? value = default(G);

            if (typeof(G) == typeof(Guid))
            {
                value = Guid.NewGuid();
                return (G?)value;
            }

            if (typeof(G) == typeof(string))
            {
                string ToHash = Guid.NewGuid().ToString() + DateTime.Now.ToString();
                value = ToHash.GenerateSHA1();
                return (G?)value;
            }

            return (G?)value;
        }

        private object? IntOrLong(object? scalarResult)
        {
            try
            {
                return Convert.ToInt32(scalarResult);
            }
            catch { }

            try
            {
                return Convert.ToInt64(scalarResult);
            }
            catch { return 0; }
        }
        #endregion
    }
}
