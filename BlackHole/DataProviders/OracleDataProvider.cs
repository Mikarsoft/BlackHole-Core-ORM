﻿using BlackHole.CoreSupport;
using BlackHole.Enums;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Reflection;

namespace BlackHole.DataProviders
{
    internal class OracleDataProvider
    {
        private readonly string _connectionString;
        internal readonly string[] insertedOutputs = new string[2];
        internal readonly bool skipQuotes = false;

        internal OracleDataProvider(string connectionString, BlackHoleIdTypes idType)
        {
            _connectionString = connectionString;
        }

        public bool IsGeneratorRequired(Type type)
        {
            if (type != typeof(int))
            {
                return true;
            }

            return false;
        }

        public bool SkipQuotes()
        {
            return skipQuotes;
        }

        public string[] GetIdOutputCommand(string tableName, string columnName)
        {
            return insertedOutputs;
        }

        public BlackHoleIdTypes GetIdType(Type type)
        {
            if (type == typeof(int))
            {
                return BlackHoleIdTypes.IntId;
            }

            if (type == typeof(Guid))
            {
                return BlackHoleIdTypes.GuidId;
            }

            return BlackHoleIdTypes.StringId;
        }

        public G? ExecuteScalar<G>(string commandText, object?[]? parameters)
        {
            G? Id = default(G);

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (parameters != null)
                    {
                        Command.Parameters.AddRange(parameters);
                    }

                    Id = (G?)Command.ExecuteScalar();
                }
            }
            catch
            {

            }

            return Id;
        }

        public G? ExecuteScalar<G>(string commandText, object?[]? parameters, BlackHoleTransaction bHTransaction)
        {
            G? Id = default(G);

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (parameters != null)
                {
                    Command.Parameters.AddRange(parameters);
                }

                Id = (G?)Command.ExecuteScalar();
            }
            catch
            {

            }

            return Id;
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, object?[]? parameters)
        {
            G? Id = default(G);

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (parameters != null)
                    {
                        Command.Parameters.AddRange(parameters);
                    }

                    Id = (G?)await Command.ExecuteScalarAsync();
                }
            }
            catch
            {

            }

            return Id;
        }

        public async Task<G?> ExecuteScalarAsync<G>(string commandText, object?[]? parameters, BlackHoleTransaction bHTransaction)
        {
            G? Id = default(G);

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (parameters != null)
                {
                    Command.Parameters.AddRange(parameters);
                }

                Id = (G?)await Command.ExecuteScalarAsync();
            }
            catch
            {

            }

            return Id;
        }

        public G? ExecuteScalar<T, G>(string commandText, T entry)
        {
            G? Id = default(G);

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (entry != null)
                    {
                        Command.Parameters.AddRange(BreakObjectToParameters(entry));
                    }

                    Id = (G?)Command.ExecuteScalar();
                }
            }
            catch
            {

            }

            return Id;
        }

        public G? ExecuteScalar<T, G>(string commandText, T entry, BlackHoleTransaction bHTransaction)
        {
            G? Id = default(G);

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (entry != null)
                {
                    Command.Parameters.AddRange(BreakObjectToParameters(entry));
                }

                Id = (G?)Command.ExecuteScalar();
            }
            catch
            {

            }

            return Id;
        }

        public async Task<G?> ExecuteScalarAsync<T, G>(string commandText, T entry)
        {
            G? Id = default(G);

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (entry != null)
                    {
                        Command.Parameters.AddRange(BreakObjectToParameters(entry));
                    }

                    Id = (G?)await Command.ExecuteScalarAsync();
                }
            }
            catch
            {

            }

            return Id;
        }

        public async Task<G?> ExecuteScalarAsync<T, G>(string commandText, T entry, BlackHoleTransaction bHTransaction)
        {
            G? Id = default(G);

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (entry != null)
                {
                    Command.Parameters.AddRange(BreakObjectToParameters(entry));
                }

                Id = (G?)await Command.ExecuteScalarAsync();
            }
            catch
            {

            }

            return Id;
        }

        public List<G> MultiScalarTransaction<T,G>(string commandText, List<T> entries)
        {
            List<G> result = new List<G>();
            using (BlackHoleTransaction bHTransaction = new BlackHoleTransaction())
            {
                foreach (T entry in entries)
                {
                    if (entry != null)
                    {
                        object?[] parameters = BreakObjectToParameters(entry);

                        G? response = ExecuteScalar<G>(commandText, parameters, bHTransaction);

                        if (response != null)
                        {
                            result.Add(response);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<List<G>> MultiScalarTransactionAsync<T,G>(string commandText, List<T> entries)
        {
            List<G> result = new List<G>();

            using (BlackHoleTransaction bHTransaction = new BlackHoleTransaction())
            {
                foreach (T entry in entries)
                {
                    if (entry != null)
                    {
                        object?[] parameters = BreakObjectToParameters(entry);

                        G? response = await ExecuteScalarAsync<G>(commandText, parameters, bHTransaction);

                        if (response != null)
                        {
                            result.Add(response);
                        }
                    }
                }
            }

            return result;
        }

        public List<G> MultiScalarTransaction<T,G>(string commandText, List<T> entries, BlackHoleTransaction bHTransaction)
        {
            List<G> result = new List<G>();

            foreach (T entry in entries)
            {
                if (entry != null)
                {
                    object?[] parameters = BreakObjectToParameters(entry);

                    G? response = ExecuteScalar<G>(commandText, parameters, bHTransaction);

                    if (response != null)
                    {
                        result.Add(response);
                    }
                }
            }

            return result;
        }

        public async Task<List<G>> MultiScalarTransactionAsync<T,G>(string commandText, List<T> entries, BlackHoleTransaction bHTransaction)
        {
            List<G> result = new List<G>();

            foreach (T entry in entries)
            {
                if (entry != null)
                {
                    object?[] parameters = BreakObjectToParameters(entry);

                    G? response = await ExecuteScalarAsync<G>(commandText, parameters, bHTransaction);

                    if (response != null)
                    {
                        result.Add(response);
                    }
                }
            }

            return result;
        }

        public bool JustExecute(string commandText, object?[]? parameters)
        {
            bool success = false;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (parameters != null)
                    {
                        Command.Parameters.AddRange(parameters);
                    }

                    Command.ExecuteNonQuery();
                    connection.Close();
                }

                success = true;
            }
            catch
            {

            }

            return success;
        }

        public async Task<bool> JustExecuteAsync(string commandText, object?[]? parameters)
        {
            bool success = false;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (parameters != null)
                    {
                        Command.Parameters.AddRange(parameters);
                    }

                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }

                success = true;
            }
            catch
            {

            }

            return success;
        }

        public bool JustExecute(string commandText, object?[]? parameters, BlackHoleTransaction bHTransaction)
        {
            bool success = false;

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (parameters != null)
                {
                    Command.Parameters.AddRange(parameters);
                }

                Command.ExecuteNonQuery();
                success = true;
            }
            catch
            {

            }

            return success;
        }

        public async Task<bool> JustExecuteAsync(string commandText, object?[]? parameters, BlackHoleTransaction bHTransaction)
        {
            bool success = false;

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (parameters != null)
                {
                    Command.Parameters.AddRange(parameters);
                }

                await Command.ExecuteNonQueryAsync();
                success = true;
            }
            catch
            {

            }

            return success;
        }

        public bool JustExecute<T>(string commandText, T entry)
        {
            bool success = false;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (entry != null)
                    {
                        Command.Parameters.AddRange(BreakObjectToParameters(entry));
                    }

                    Command.ExecuteNonQuery();
                    connection.Close();
                }

                success = true;
            }
            catch
            {

            }

            return success;
        }

        public async Task<bool> JustExecuteAsync<T>(string commandText, T entry)
        {
            bool success = false;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (entry != null)
                    {
                        Command.Parameters.AddRange(BreakObjectToParameters(entry));
                    }

                    await Command.ExecuteNonQueryAsync();
                    connection.Close();
                }

                success = true;
            }
            catch
            {

            }

            return success;
        }

        public bool JustExecute<T>(string commandText, T entry, BlackHoleTransaction bHTransaction)
        {
            bool success = false;

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (entry != null)
                {
                    Command.Parameters.AddRange(BreakObjectToParameters(entry));
                }

                Command.ExecuteNonQuery();
                success = true;
            }
            catch
            {

            }

            return success;
        }

        public async Task<bool> JustExecuteAsync<T>(string commandText, T entry, BlackHoleTransaction bHTransaction)
        {
            bool success = false;

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (entry != null)
                {
                    Command.Parameters.AddRange(BreakObjectToParameters(entry));
                }

                await Command.ExecuteNonQueryAsync();
                success = true;
            }
            catch
            {

            }

            return success;
        }

        public T? QueryFirst<T>(string commandText, object?[]? parameters)
        {
            T? result = default(T);

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(commandText, connection);

                    if (parameters != null)
                    {
                        Command.Parameters.AddRange(parameters);
                    }

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
            }
            catch
            {

            }

            return result;
        }

        public List<T> Query<T>(string command, object?[]? parameters)
        {
            List<T> result = new List<T>();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    OracleCommand Command = new OracleCommand(command, connection);

                    if (parameters != null)
                    {
                        Command.Parameters.AddRange(parameters);
                    }

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
            }
            catch
            {

            }

            return result;
        }

        public async Task<T?> QueryFirstAsync<T>(string command, object?[]? parameters)
        {
            T? result = default(T);

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(command, connection);

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
                    await connection.CloseAsync();
                }
            }
            catch
            {

            }

            return result;
        }

        public async Task<List<T>> QueryAsync<T>(string command, object?[]? parameters)
        {
            List<T> result = new List<T>();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    OracleCommand Command = new OracleCommand(command, connection);

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
                    await connection.CloseAsync();
                }
            }
            catch
            {

            }

            return result;
        }

        public T? QueryFirst<T>(string commandText, object?[]? parameters, BlackHoleTransaction bHTransaction)
        {
            T? result = default(T);

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (parameters != null)
                {
                    Command.Parameters.AddRange(parameters);
                }

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
            }
            catch
            {

            }

            return result;
        }

        public List<T> Query<T>(string commandText, object?[]? parameters, BlackHoleTransaction bHTransaction)
        {
            List<T> result = new List<T>();

            try
            {
                OracleConnection? connection = bHTransaction.connection as OracleConnection;
                OracleTransaction? transaction = bHTransaction._transaction as OracleTransaction;
                OracleCommand Command = new OracleCommand(commandText, connection);
                Command.Transaction = transaction;

                if (parameters != null)
                {
                    Command.Parameters.AddRange(parameters);
                }

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
            }
            catch
            {

            }

            return result;
        }

        public async Task<T?> QueryFirstAsync<T>(string commandText, object?[]? parameters, BlackHoleTransaction bHTransaction)
        {
            T? result = default(T);

            try
            {
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
            }
            catch
            {

            }

            return result;
        }

        public async Task<List<T>> QueryAsync<T>(string commandText, object?[]? parameters, BlackHoleTransaction bHTransaction)
        {
            List<T> result = new List<T>();

            try
            {
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
            }
            catch
            {

            }

            return result;
        }

        private T? MapObject<T>(OracleDataReader reader)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            object? obj = Activator.CreateInstance(type);
            try
            {
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
            }
            catch
            {

            }

            return (T?)obj;
        }

        private T? MapObject<T>(DbDataReader reader)
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

        private object?[] BreakObjectToParameters(object item)
        {
            PropertyInfo[] propertyInfos = item.GetType().GetProperties();
            object?[] parameters = new object[propertyInfos.Length];
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                object? value = propertyInfos[i].GetValue(item);

                parameters[i] = value;

                if (value?.GetType() == typeof(Guid))
                {
                    parameters[i] = value.ToString();
                }
            }
            return parameters;
        }
    }
}