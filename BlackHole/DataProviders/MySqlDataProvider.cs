using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.Logger;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.DataProviders
{
    internal class MySqlDataProvider : IDataProvider
    {
        #region Constractor
        private readonly string _connectionString;
        internal readonly string insertedOutput = "SELECT LAST_INSERT_ID();";
        internal readonly bool skipQuotes = true;
        private readonly BlackHoleIdTypes _idType;
        private readonly ILoggerService _loggerService;
        private readonly bool useGenerator = false;
        private string TableName = string.Empty;

        internal MySqlDataProvider(string connectionString, BlackHoleIdTypes idType, string tableName)
        {
            _connectionString = connectionString;
            _idType = idType;
            _loggerService = new LoggerService();
            TableName=tableName;

            if(idType != BlackHoleIdTypes.IntId)
            {
                useGenerator = true;
            }
            else
            {
                useGenerator = false;
            }
        }
        #endregion

        #region Internal Processes
        private G? ExecuteEntryScalar<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default(G);
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{TableName}",ex.Message,ex.ToString())).Start();
                return default(G);
            }
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default(G);
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs($"InsertAsync_{TableName}", ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        private G? ExecuteEntryScalar<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);

                object? Result = Command.ExecuteScalar();

                if(Result!= null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{TableName}", ex.Message, ex.ToString())).Start();
            }

            return default(G);
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);

                object? Result = await Command.ExecuteScalarAsync();

                if (Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"InsertAsync_{TableName}", ex.Message, ex.ToString())).Start();
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

                if (ExecuteEntry($"{commandStart},Id) {commandEnd},@Id);", entry))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd});{insertedOutput}", entry);
            }
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                if (ExecuteEntry($"{commandStart},Id) {commandEnd},@Id);", entry, bhTransaction))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}) {commandEnd});{insertedOutput}", entry, bhTransaction);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                if (await ExecuteEntryAsync($"{commandStart},Id) {commandEnd},@Id);", entry))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd});{insertedOutput}", entry);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();
                entry?.GetType().GetProperty("Id")?.SetValue(entry, Id);

                if (await ExecuteEntryAsync($"{commandStart},Id) {commandEnd},@Id);", entry, bhTransaction))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}) {commandEnd});{insertedOutput}", entry, bhTransaction);
            }
        }

        public async Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction)
        {
            List<G?> Ids = new List<G?>();

            if (useGenerator)
            {
                string commandText = $"{commandStart},Id) {commandEnd},@Id);";

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
                string commandText = $"{commandStart}) {commandEnd});{insertedOutput}";

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
                string commandText = $"{commandStart},Id) {commandEnd},@Id);";

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
                string commandText = $"{commandStart}) {commandEnd});{insertedOutput}";

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
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{TableName}", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    MySqlCommand Command = new MySqlCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return true;
            }
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"InserAsync_{TableName}", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);

                Command.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{TableName}", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                MySqlConnection? connection = bhTransaction.connection as MySqlConnection;
                MySqlTransaction? transaction = bhTransaction._transaction as MySqlTransaction;
                MySqlCommand Command = new MySqlCommand(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);

                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"InsertAsync_{TableName}", ex.Message, ex.ToString())).Start();
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Execute_{TableName}", ex.Message, ex.ToString())).Start();
                return false;
            }
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Execute_{TableName}", ex.Message, ex.ToString())).Start();
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"ExecuteAsync_{TableName}", ex.Message, ex.ToString())).Start();
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"ExecuteAsync_{TableName}", ex.Message, ex.ToString())).Start();
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
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
                new Thread(() => _loggerService.CreateErrorLogs($"SelectFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
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
                new Thread(() => _loggerService.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
                return new List<T>();
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
                return default(T);
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
                return new List<T>();
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
                new Thread(() => _loggerService.CreateErrorLogs($"SelectFirstAsync_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
                return default(T);
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
                new Thread(()=>_loggerService.CreateErrorLogs($"SelectAsync_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Object Mapping {typeof(T).Name}",ex.Message,ex.ToString())).Start();
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
                object? obj = Activator.CreateInstance(type);

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
            catch(Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Object Mapping {typeof(T).Name}", ex.Message, ex.ToString())).Start();
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

        private void ObjectToParameters<T>(T item, MySqlParameterCollection parameters)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach(PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(item);

                if (value?.GetType() == typeof(Guid))
                {
                    parameters.Add(new MySqlParameter(@property.Name, value.ToString()));
                }
                else
                {
                    parameters.Add(new MySqlParameter(@property.Name, value));
                }
            }
        }

        private G? GenerateId<G>()
        {
            object? value = default(G);

            switch (_idType)
            {
                case BlackHoleIdTypes.GuidId:
                    value = Guid.NewGuid();
                    break;
                case BlackHoleIdTypes.StringId:
                    string ToHash = Guid.NewGuid().ToString() + DateTime.Now.ToString();
                    value = ToHash.GenerateSHA1();
                    break;
            }

            return (G?)value;
        }
        #endregion
    }
}
