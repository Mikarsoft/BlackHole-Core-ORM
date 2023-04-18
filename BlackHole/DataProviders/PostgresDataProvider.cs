﻿using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.Logger;
using Npgsql;
using System.Reflection;

namespace BlackHole.DataProviders
{
    internal class PostgresDataProvider : IDataProvider
    {
        #region Constructor
        private readonly string _connectionString;
        internal readonly string insertedOutput = "";
        internal readonly bool skipQuotes = false;
        private readonly ILoggerService _loggerService;
        private readonly bool useGenerator = false;
        private string TableName = string.Empty;

        internal PostgresDataProvider(string connectionString, BlackHoleIdTypes idType, string tableName)
        {
            _connectionString = connectionString;
            _loggerService = new LoggerService();
            insertedOutput = $@"returning ""{tableName}"".""Id""";
            TableName = tableName;

            if (idType != BlackHoleIdTypes.StringId)
            {
                useGenerator = false;
            }
            else
            {
                useGenerator = true;
            }
        }
        #endregion

        #region Internal Processes
        private G? ExecuteEntryScalar<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default(G);
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    object? Result =Command.ExecuteScalar();
                    connection.Close();

                    if(Result != null)
                    {
                        Id = (G?)Result;
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default(G);
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs($"InsertAsync_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        private G? ExecuteEntryScalar<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);

                object? Result = Command.ExecuteScalar();

                if(Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{TableName}", commandText, ex.Message, ex.ToString())).Start();
            }
            return default(G);
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);

                object? Result = await Command.ExecuteScalarAsync();

                if(Result != null)
                {
                    return (G?)Result;
                }
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"InsertAsync_{TableName}", commandText, ex.Message, ex.ToString())).Start();
            }
            return default(G);
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
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                if (ExecuteEntry($@"{commandStart},""Id"") {commandEnd},@Id);", entry))
                {
                    return Id;
                }
                else
                {
                    return default(G);
                }
            }
            else
            {
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd}) {insertedOutput};", entry);
            }
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                if (ExecuteEntry($@"{commandStart},""Id"") {commandEnd},@Id);", entry, bhTransaction))
                {
                    return Id;
                }
                else
                {
                    return default(G);
                }
            }
            else
            {
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd}) {insertedOutput};", entry, bhTransaction);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                if (await ExecuteEntryAsync($@"{commandStart},""Id"") {commandEnd},@Id);", entry))
                {
                    return Id;
                }
                else
                {
                    return default(G);
                }
            }
            else
            {
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd}) {insertedOutput};", entry);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                if (await ExecuteEntryAsync($@"{commandStart},""Id"") {commandEnd},@Id);", entry, bhTransaction))
                {
                    return Id;
                }
                else
                {
                    return default(G);
                }
            }
            else
            {
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd}) {insertedOutput};", entry, bhTransaction);
            }
        }

        public async Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction)
        {
            List<G?> Ids = new List<G?>();

            if (useGenerator)
            {
                string commandText = $@"{commandStart},""Id"") {commandEnd},@Id);";

                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                    if (await ExecuteEntryAsync(commandText, entry, bhTransaction))
                    {
                        Ids.Add(Id);
                    }
                    else
                    {
                        Ids.Add(default(G));
                    }
                }
            }
            else
            {
                string commandText = $"{commandStart}) {commandEnd}) {insertedOutput};";

                foreach (T entry in entries)
                {
                    Ids.Add(await ExecuteEntryScalarAsync<T, G>(commandText, entry, bhTransaction));
                }
            }

            return Ids;
        }

        public List<G?> MultiInsertScalar<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction)
        {
            List<G?> Ids = new List<G?>();

            if (useGenerator)
            {
                string commandText = $@"{commandStart},""Id"") {commandEnd},@Id);";

                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                    if (ExecuteEntry(commandText, entry, bhTransaction))
                    {
                        Ids.Add(Id);
                    }
                    else
                    {
                        Ids.Add(default(G));
                    }
                }
            }
            else
            {
                string commandText = $"{commandStart}) {commandEnd}) {insertedOutput};";

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
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"InsertAsync_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);

                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                NpgsqlConnection? connection = bhTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bhTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);

                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"InsertAsync_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
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
                new Thread(() => _loggerService.CreateErrorLogs($"Execute_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters)
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
                new Thread(() => _loggerService.CreateErrorLogs($"Execute_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
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
                new Thread(() => _loggerService.CreateErrorLogs($"ExecuteAsync_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> JustExecuteAsync(string commandText, List<BlackHoleParameter>? parameters)
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
                new Thread(() => _loggerService.CreateErrorLogs($"ExecuteAsync_{TableName}", commandText, ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters)
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
                new Thread(() => _loggerService.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new List<T>();

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
                new Thread(() => _loggerService.CreateErrorLogs($"Select_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default(T);

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs($"SelectFirstAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    NpgsqlCommand Command = new NpgsqlCommand(commandText, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
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
                new Thread(() => _loggerService.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
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
                new Thread(() => _loggerService.CreateErrorLogs($"Select_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);
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
                new Thread(() => _loggerService.CreateErrorLogs($"SelectFirstAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new List<T>();

                NpgsqlConnection? connection = bHTransaction.connection as NpgsqlConnection;
                NpgsqlTransaction? transaction = bHTransaction._transaction as NpgsqlTransaction;
                NpgsqlCommand Command = new NpgsqlCommand(commandText, connection, transaction);
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
                new Thread(() => _loggerService.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", commandText, ex.Message, ex.ToString())).Start();
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
                object? obj = Activator.CreateInstance(type);

                if (properties.Length == 0 && reader.FieldCount > 0)
                {
                    object? value = reader.GetValue(0);

                    if (value != null)
                    {
                        return (T?)value;
                    }

                    return default;
                }

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        string propertyName = reader.GetName(i);
                        PropertyInfo? property = properties.Where(x => string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                        if (property != null)
                        {
                            obj?.GetType()?.GetProperty(property.Name)?.SetValue(obj, reader.GetValue(i));
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

        private void ArrayToParameters(List<BlackHoleParameter>? bhParameters, NpgsqlParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;
                    parameters.Add(new NpgsqlParameter(param.Name, value));
                }
            }
        }

        private void ObjectToParameters<T>(T item, NpgsqlParameterCollection parameters)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(item);
                parameters.Add(new NpgsqlParameter(@property.Name, value));
            }
        }

        private G? GenerateId<G>()
        {
            string ToHash = Guid.NewGuid().ToString() + DateTime.Now.ToString();

            object? value = ToHash.GenerateSHA1();

            return (G?)value;
        }
        #endregion
    }
}
