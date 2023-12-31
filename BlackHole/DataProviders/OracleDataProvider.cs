﻿using BlackHole.Engine;
using BlackHole.Enums;
using BlackHole.Logger;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.DataProviders
{
    internal class OracleDataProvider : IDataProvider
    {
        #region Constructor
        private readonly string _connectionString;
        internal readonly string insertedOutput = @"returning ""Id"" into :Id";
        internal readonly bool skipQuotes = false;
        private readonly BlackHoleIdTypes _idType;
        private readonly bool useGenerator = false;
        private readonly string TableName = string.Empty;

        internal OracleDataProvider(string connectionString, BlackHoleIdTypes idType, string tableName)
        {
            _connectionString = connectionString;
            _idType = idType;
            TableName = tableName;

            if (idType != BlackHoleIdTypes.IntId)
            {
                useGenerator = true;
            }
            else
            {
                useGenerator = false;
            }
        }

        internal OracleDataProvider(string connectionString)
        {
            _connectionString = connectionString;
            TableName = string.Empty;
            useGenerator = false;
        }
        #endregion

        #region Internal Processes
        private G? ExecuteEntryScalar<T, G>(string commandText, T entry)
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
                    ObjectToParameters(entry, Command.Parameters);
                    Command.ExecuteScalar();
                    int param = Command.Parameters.IndexOf("Id");

                    if (param > -1)
                    {
                        Id = (G?)Command.Parameters[param].Value;
                    }
                    connection.Close();
                }
                return Id;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{TableName}", ex.Message, ex.ToString()));
                return default;
            }
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry)
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
                    ObjectToParameters(entry, Command.Parameters);
                    await Command.ExecuteScalarAsync();
                    int param = Command.Parameters.IndexOf("Id");

                    if (param > -1)
                    {
                        Id = (G?)Command.Parameters[param].Value;
                    }
                    await connection.CloseAsync();
                }
                return Id;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
            }
        }

        private G? ExecuteEntryScalar<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction)
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
                ObjectToParameters(entry, Command.Parameters);
                Command.ExecuteScalar();
                int param = Command.Parameters.IndexOf("Id");

                if (param > -1)
                {
                    return (G?)Command.Parameters[param].Value;
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{TableName}", ex.Message, ex.ToString()));
            }
            return default;
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction)
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
                ObjectToParameters(entry, Command.Parameters);
                await Command.ExecuteScalarAsync();
                int param = Command.Parameters.IndexOf("Id");

                if (param > -1)
                {
                    return (G?)Command.Parameters[param].Value;
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{TableName}", ex.Message, ex.ToString()));
            }
            return default;
        }
        #endregion

        #region Helper Methods
        public bool SkipQuotes()
        {
            return skipQuotes;
        }
        #endregion

        #region Execution Methods
        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (ExecuteEntry($@"{commandStart},""Id"") {commandEnd}, @Id)", entry))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd}) {insertedOutput}", entry);
            }
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (ExecuteEntry($@"{commandStart},""Id"") {commandEnd}, @Id)", entry, bhTransaction))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd}) {insertedOutput}", entry, bhTransaction);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (await ExecuteEntryAsync($@"{commandStart}, ""Id"") {commandEnd}, @Id)", entry))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd}) {insertedOutput}", entry);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                if (await ExecuteEntryAsync($@"{commandStart},""Id"") {commandEnd}, @Id)", entry, bhTransaction))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd}) {insertedOutput}", entry, bhTransaction);
            }
        }

        public async Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction)
        {
            List<G?> Ids = new();
            if (useGenerator)
            {
                string commandText = $@"{commandStart},""Id"") {commandEnd}, @Id)";
                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                    if (await ExecuteEntryAsync(commandText, entry, bhTransaction))
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
                string commandText = $"{commandStart}) {commandEnd}) {insertedOutput}";
                foreach (T entry in entries)
                {
                    Ids.Add(await ExecuteEntryScalarAsync<T, G>(commandText, entry, bhTransaction));
                }
            }
            return Ids;
        }

        public List<G?> MultiInsertScalar<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction)
        {
            List<G?> Ids = new();
            if (useGenerator)
            {
                string commandText = $@"{commandStart},""Id"") {commandEnd}, @Id)";
                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    typeof(T).GetProperty("Id")?.SetValue(entry, Id);
                    if (ExecuteEntry(commandText, entry, bhTransaction))
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
                string commandText = $"{commandStart}) {commandEnd}) {insertedOutput}";
                foreach (T entry in entries)
                {
                    Ids.Add(ExecuteEntryScalar<T, G>(commandText, entry, bhTransaction));
                }
            }
            return Ids;
        }

        public bool ExecuteEntry<T>(string commandText, T entry)
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
                    ObjectToParameters(entry, Command.Parameters);
                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{TableName}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry)
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
                    ObjectToParameters(entry, Command.Parameters);
                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{TableName}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
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
                ObjectToParameters(entry, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{TableName}", ex.Message, ex.ToString()));
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
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
                ObjectToParameters(entry, Command.Parameters);
                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"InsertAsync_{TableName}", ex.Message, ex.ToString()));
                return false;
            }
        }

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
                    if (paramIndex > -1)
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Execute_{TableName}", ex.Message, ex.ToString()));
                return false;
            }
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Execute_{TableName}", ex.Message, ex.ToString()));
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"ExecuteAsync_{TableName}", ex.Message, ex.ToString()));
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"ExecuteAsync_{TableName}", ex.Message, ex.ToString()));
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString()));
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString()));
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return default;
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
                await Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString()));
                return new List<T>();
            }
        }
        #endregion

        #region Object Mapping
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
                                type.GetType()?.GetProperty(property.Name)?.SetValue(obj, GuidParser(reader.GetString(i)));
                            }
                            else if (property.PropertyType == typeof(bool))
                            {
                                type.GetType()?.GetProperty(property.Name)?.SetValue(obj, reader.GetBoolean(i));
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
                    else
                    {
                        parameters.Add(new OracleParameter(param.Name, DBNull.Value));
                    }
                }
            }
        }

        private void ObjectToParameters<T>(T item, OracleParameterCollection parameters)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(item);

                if(value != null)
                {
                    Type Vtype = value.GetType();
                    if (Vtype == typeof(Guid))
                    {
                        parameters.Add(new OracleParameter(property.Name, value.ToString()));
                    }
                    else if (Vtype == typeof(bool))
                    {
                        parameters.Add(new OracleParameter(property.Name, (bool)value ? 1 : 0));
                    }
                    else
                    {
                        parameters.Add(new OracleParameter(property.Name, value));
                    }
                }
                else
                {
                    parameters.Add(new OracleParameter(property.Name, DBNull.Value));
                }
            }
        }

        private G? GenerateId<G>()
        {
            object? value = default(G);

            if(typeof(G) == typeof(Guid))
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
        #endregion
    }
}
