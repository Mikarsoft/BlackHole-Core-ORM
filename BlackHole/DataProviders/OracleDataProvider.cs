using BlackHole.CoreSupport;
using BlackHole.Enums;
using BlackHole.Logger;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.DataProviders
{
    internal class OracleDataProvider : IDataProvider
    {
        private readonly string _connectionString;
        internal readonly string insertedOutput = "SELECT LAST_INSERT_ID();";
        internal readonly bool skipQuotes = true;
        private readonly BlackHoleIdTypes _idType;
        private readonly ILoggerService _loggerService;
        private readonly bool useGenerator = false;

        internal OracleDataProvider(string connectionString, BlackHoleIdTypes idType)
        {
            _connectionString = connectionString;
            _idType = idType;
            _loggerService = new LoggerService();

            if (idType != BlackHoleIdTypes.IntId)
            {
                useGenerator = true;
            }
            else
            {
                useGenerator = false;
            }
        }

        #region Internal Processes
        private G? ExecuteEntryScalar<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default(G);
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    Id = (G?)Command.ExecuteScalar();
                    connection.Close();
                }
                return Id;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry)
        {
            try
            {
                G? Id = default(G);
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    Id = (G?)await Command.ExecuteScalarAsync();
                    await connection.CloseAsync();
                }
                return Id;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"InsertAsync_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        private G? ExecuteEntryScalar<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                ObjectToParameters(entry, Command.Parameters);

                return (G?)Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString())).Start();
                return default(G);
            }
        }

        private async Task<G?> ExecuteEntryScalarAsync<T, G>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                ObjectToParameters(entry, Command.Parameters);

                return (G?)await Command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return default(G);
            }
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

                if (ExecuteEntry($"{commandStart},Id){commandEnd},'{Id}');", entry))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}){commandEnd});{insertedOutput}", entry);
            }
        }

        public G? InsertScalar<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();

                if (ExecuteEntry($"{commandStart},Id) {commandEnd},'{Id}');", entry, bhTransaction))
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
                return ExecuteEntryScalar<T, G>($"{commandStart}){commandEnd});{insertedOutput}", entry, bhTransaction);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();

                if (await ExecuteEntryAsync($"{commandStart},Id){commandEnd},'{Id}');", entry))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}){commandEnd});{insertedOutput}", entry);
            }
        }

        public async Task<G?> InsertScalarAsync<T, G>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            if (useGenerator)
            {
                G? Id = GenerateId<G>();

                if (await ExecuteEntryAsync($"{commandStart},Id){commandEnd},'{Id}');", entry, bhTransaction))
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
                return await ExecuteEntryScalarAsync<T, G>($"{commandStart}){commandEnd});{insertedOutput}", entry, bhTransaction);
            }
        }

        public async Task<List<G?>> MultiInsertScalarAsync<T, G>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction)
        {
            List<G?> Ids = new List<G?>();

            if (useGenerator)
            {
                string commandText = "";
                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    commandText = $"{commandStart},Id){commandEnd},'{Id}');";

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
                string commandText = $"{commandStart}){commandEnd});{insertedOutput}";

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
                string commandText = "";
                foreach (T entry in entries)
                {
                    G? Id = GenerateId<G>();
                    commandText = $"{commandStart},Id){commandEnd},'{Id}');";

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
                string commandText = $"{commandStart}){commandEnd});{insertedOutput}";

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
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);

                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                ObjectToParameters(entry, Command.Parameters);

                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public async Task<bool> ExecuteEntryAsync<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                ObjectToParameters(entry, Command.Parameters);

                await Command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool JustExecute(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                ArrayToParameters(parameters, Command.Parameters);

                Command.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return false;
            }
        }

        public bool JustExecute(string commandText, BlackHoleParameter[]? parameters)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    ArrayToParameters(parameters, Command.Parameters);

                    Command.ExecuteNonQuery();
                    connection.Close();
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
                OracleConnection? connection = bhTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bhTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

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

        public async Task<bool> JustExecuteAsync(string commandText, BlackHoleParameter[]? parameters)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(commandText, connection);

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


        public T? QueryFirst<T>(string commandText, BlackHoleParameter[]? parameters)
        {
            try
            {
                T? result = default(T);

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs("Insert", ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public List<T> Query<T>(string command, BlackHoleParameter[]? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(command, connection);
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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string command, BlackHoleParameter[]? parameters)
        {
            try
            {
                T? result = default(T);

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(command, connection);
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

        public async Task<List<T>> QueryAsync<T>(string command, BlackHoleParameter[]? parameters)
        {
            try
            {
                List<T> result = new List<T>();

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(command, connection);
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

        public T? QueryFirst<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return default(T);
            }
        }

        public List<T> Query<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new List<T>();

                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

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
                new Thread(() => _loggerService.CreateErrorLogs("Select", ex.Message, ex.ToString())).Start();
                return new List<T>();
            }
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default(T);

                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

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

        public async Task<List<T>> QueryAsync<T>(string commandText, BlackHoleParameter[]? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new List<T>();

                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

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

        #region Object Mapping
        private T? MapObject<T>(OracleDataReader reader)
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
                            PropertyInfo property = properties.Where(x => x.Name == propertyName).First();

                            if (property.PropertyType == typeof(Guid))
                            {
                                Guid result = Guid.Empty;
                                Guid.TryParse(reader.GetString(i), out result);
                                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, result);
                            }
                            else
                            {
                                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, reader.GetValue(i));
                            }
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
                            PropertyInfo property = properties.Where(x => x.Name == propertyName).First();

                            if (property.PropertyType == typeof(Guid))
                            {
                                Guid result = Guid.Empty;
                                Guid.TryParse(reader.GetString(i), out result);
                                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, result);
                            }
                            else
                            {
                                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, reader.GetValue(i));
                            }
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

        private void ArrayToParameters(BlackHoleParameter[]? bhParameters, OracleParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;

                    if (value?.GetType() == typeof(Guid))
                    {
                        parameters.Add(new OracleParameter(@param.Name, value.ToString()));
                    }
                    else
                    {
                        parameters.Add(new OracleParameter(@param.Name, value));
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

                if (value?.GetType() == typeof(Guid))
                {
                    parameters.Add(new OracleParameter(@property.Name, value.ToString()));
                }
                else
                {
                    parameters.Add(new OracleParameter(@property.Name, value));
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
