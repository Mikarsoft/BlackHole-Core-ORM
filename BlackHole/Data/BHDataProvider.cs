using BlackHole.Interfaces;
using BlackHole.Entities;
using System.Reflection;
using System.Data;
using Dapper;
using BlackHole.Services;
using System.Linq.Expressions;
using BlackHole.Attributes.EntityAttributes;
using BlackHole.Enums;

namespace BlackHole.Data
{
    /// <summary>
    /// Makes all the communication between the Datbase Table and The Specified Entity
    /// </summary>
    /// <typeparam name="T">Black Hole Entity with Integer Id</typeparam>
    public class BHDataProvider<T, G> : IBHDataProvider<T, G> where T : BlackHoleEntity<G>
    {
        private bool withActivator { get; }
        private string ThisTable { get; } = string.Empty;
        private List<string> Columns { get; } = new List<string>();
        private string PropertyNames { get; } = string.Empty;
        private string PropertyParams { get; } = string.Empty;
        private string UpdateParams { get; } = string.Empty;
        private string ThisId { get; } = string.Empty;
        private string ThisInactive { get; } = string.Empty;
        private bool isMyShit { get; }
        private bool requiredId { get; }
        private string OutputIdMiddle { get; set; }
        private string OutputIdEnding { get; set; }
        private string OriginalTableName { get; set; }

        private BHIdTypes idType { get; }

        private IBHDatabaseSelector _multiDatabaseSelector;
        private ILoggerService _loggerService;

        /// <summary>
        /// Create a Data Provider that Automatically Communicates with the Database Using the Black Hole Entity you pass in.
        /// </summary>
        public BHDataProvider()
        {
            _multiDatabaseSelector = new BHDatabaseSelector();
            _loggerService = new LoggerService();
            isMyShit = _multiDatabaseSelector.GetMyShit();

            Type _type = typeof(T);
            OriginalTableName = _type.Name;
            var attributes = _type.GetCustomAttributes(true);
            var attribute = attributes.SingleOrDefault(x => x.GetType() == typeof(UseActivator));

            if (attribute != null)
            {
                withActivator = true;
            }

            string name = _type.Name;

            ThisTable = MyShit(name);
            ThisId = MyShit("Id");
            ThisInactive = MyShit("Inactive");

            idType = _multiDatabaseSelector.GetIdType(typeof(G));
            requiredId = _multiDatabaseSelector.RequiredIdGeneration(idType);

            string[] insertOutputs = _multiDatabaseSelector.IdOutput(ThisTable, ThisId, false);
            OutputIdMiddle = insertOutputs[0];
            OutputIdEnding = insertOutputs[1];

            IList<PropertyInfo> props = new List<PropertyInfo>(_type.GetProperties());

            foreach (PropertyInfo prop in props)
            {

                if (prop.Name != "Inactive")
                {
                    if (prop.Name != "Id")
                    {
                        string property = MyShit(prop.Name);
                        PropertyNames += $",{property}";
                        PropertyParams += $",@{prop.Name}";
                        UpdateParams += $",{property}=@{prop.Name}";
                    }

                    Columns.Add(prop.Name);
                }
            }

            PropertyNames += " ";
            PropertyParams += " ";
            UpdateParams += " ";
            PropertyNames = PropertyNames.Remove(0, 1);
            PropertyParams = PropertyParams.Remove(0, 1);
            UpdateParams = UpdateParams.Remove(0, 1);
        }

        IList<T> IBHDataProvider<T, G>.GetAllEntries()
        {
            List<T> entries = new List<T>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable}";

                    if (withActivator)
                    {
                        SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0";
                    }

                    var entr = connection.Query<T>(SubCommand);
                    entries = entr.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        IList<T> IBHDataProvider<T, G>.GetAllEntries(BHTransaction bhTransaction)
        {
            List<T> entries = new List<T>();

            try
            {             
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable}";

                if (withActivator)
                {
                    SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0";
                }

                var entr = bhTransaction.connection.Query<T>(SubCommand, bhTransaction._transaction);
                entries = entr.ToList();
                
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        IList<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>() where Dto : class
        {
            List<Dto> entries = new List<Dto>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));

                    string SubCommand = $"select {colsAndParams} from {ThisTable}";

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0";
                    }

                    var entr = connection.Query<Dto>(SubCommand);
                    entries = entr.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        IList<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>(BHTransaction bhTransaction) where Dto : class
        {
            List<Dto> entries = new List<Dto>();

            try
            {  
                string colsAndParams = CompareDtoToEntity(typeof(Dto));
                string SubCommand = $"select {colsAndParams} from {ThisTable}";

                if (withActivator)
                {
                    SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0";
                }

                var entr = bhTransaction.connection.Query<Dto>(SubCommand,bhTransaction._transaction);
                entries = entr.ToList();
                
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        IList<T> IBHDataProvider<T, G>.GetAllInactiveEntries()
        {
            IList<T> entries = new List<T>();

            if (withActivator)
            {
                try
                {
                    using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                    {
                        string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=1";
                        var entr = connection.Query<T>(SubCommand);
                        entries = entr.ToList();
                    }
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                }
            }

            return entries;
        }

        IList<T> IBHDataProvider<T, G>.GetAllInactiveEntries(BHTransaction bhTransaction)
        {
            IList<T> entries = new List<T>();

            if (withActivator)
            {
                try
                {
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=1";
                    var entr = bhTransaction.connection.Query<T>(SubCommand,bhTransaction._transaction);
                    entries = entr.ToList(); 
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                }
            }

            return entries;
        }

        T? IBHDataProvider<T, G>.GetEntryById(G Id)
        {
            T? entry = default(T);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId}=@Id";
                    var Parameter = new { Id = Id };

                    if (withActivator)
                    {
                        SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId}=@Id and {ThisInactive}=0";
                    }

                    entry = connection.QueryFirstOrDefault<T>(SubCommand, Parameter);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        T? IBHDataProvider<T, G>.GetEntryById(G Id, BHTransaction bhTransaction)
        {
            T? entry = default(T);

            try
            {              
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId}=@Id";
                var Parameter = new { Id = Id };

                if (withActivator)
                {
                    SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId}=@Id and {ThisInactive}=0";
                }

                entry = bhTransaction.connection.QueryFirstOrDefault<T>(SubCommand, Parameter, bhTransaction._transaction);       
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        Dto? IBHDataProvider<T, G>.GetEntryById<Dto>(G Id) where Dto : class
        {
            Dto? entry = default(Dto);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));

                    string SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId}=@Id";
                    var Parameter = new { Id = Id };

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId}=@Id and {ThisInactive}=0";
                    }

                    entry = connection.QueryFirstOrDefault<Dto>(SubCommand, Parameter);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        Dto? IBHDataProvider<T, G>.GetEntryById<Dto>(G Id, BHTransaction bhTransaction) where Dto : class
        {
            Dto? entry = default(Dto);

            try
            {     
                string colsAndParams = CompareDtoToEntity(typeof(Dto));

                string SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId}=@Id";
                var Parameter = new { Id = Id };

                if (withActivator)
                {
                    SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId}=@Id and {ThisInactive}=0";
                }

                entry = bhTransaction.connection.QueryFirstOrDefault<Dto>(SubCommand, Parameter,bhTransaction._transaction);      
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            T? entry = default(T);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    entry = connection.QueryFirstOrDefault<T>(SubCommand, sql.Parameters);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            T? entry = default(T);

            try
            {  
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                entry = bhTransaction.connection.QueryFirstOrDefault<T>(SubCommand, sql.Parameters, bhTransaction._transaction); 
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            Dto? entry = default(Dto);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    entry = connection.QueryFirstOrDefault<Dto>(SubCommand, sql.Parameters);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            Dto? entry = default(Dto);

            try
            {    
                string colsAndParams = CompareDtoToEntity(typeof(Dto));
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                entry = bhTransaction.connection.QueryFirstOrDefault<Dto>(SubCommand, sql.Parameters, bhTransaction._transaction);  
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        IList<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            IList<T> entries = new List<T>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    var entr = connection.Query<T>(SubCommand, sql.Parameters);
                    entries = entr.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        IList<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            IList<T> entries = new List<T>();

            try
            {     
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                var entr = bhTransaction.connection.Query<T>(SubCommand, sql.Parameters,bhTransaction._transaction);
                entries = entr.ToList(); 
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        IList<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            IList<Dto> entries = new List<Dto>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    var entr = connection.Query<Dto>(SubCommand, sql.Parameters);
                    entries = entr.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        IList<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            IList<Dto> entries = new List<Dto>();

            try
            {            
                string colsAndParams = CompareDtoToEntity(typeof(Dto));
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                var entr = bhTransaction.connection.Query<Dto>(SubCommand, sql.Parameters,bhTransaction._transaction);
                entries = entr.ToList();    
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        G? IBHDataProvider<T, G>.InsertEntry(T entry)
        {
            G? Id = default(G);
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    if (requiredId)
                    {
                        Id = GenerateId();
                        insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                        connection.Execute(insertCommand, entry);
                    }
                    else
                    {
                        Id = connection.ExecuteScalar<G>(insertCommand, entry);
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        G? IBHDataProvider<T, G>.InsertEntry(T entry, BHTransaction bhTransaction)
        {
            G? Id = default(G);
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";

            try
            {             
                if (requiredId)
                {
                    Id = GenerateId();
                    insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                    bhTransaction.connection.Execute(insertCommand, entry,bhTransaction._transaction);
                }
                else
                {
                    Id = bhTransaction.connection.ExecuteScalar<G>(insertCommand, entry, bhTransaction._transaction);
                } 
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        List<G?> IBHDataProvider<T, G>.InsertEntries(List<T> entries)
        {
            List<G?> Ids = new List<G?>();
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    connection.Open();
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (T entry in entries)
                        {
                            if (requiredId)
                            {
                                G? Id = GenerateId();
                                insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                                connection.Execute(insertCommand, entry);
                                Ids.Add(Id);
                            }
                            else
                            {
                                Ids.Add(connection.ExecuteScalar<G>(insertCommand, entry, transaction));
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Ids;
        }

        List<G?> IBHDataProvider<T, G>.InsertEntries(List<T> entries, BHTransaction bhTransaction)
        {
            List<G?> Ids = new List<G?>();
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";
            try
            {            
                foreach (T entry in entries)
                {
                    if (requiredId)
                    {
                        G? Id = GenerateId();
                        insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                        bhTransaction.connection.Execute(insertCommand, entry, bhTransaction._transaction);
                        Ids.Add(Id);
                    }
                    else
                    {
                        Ids.Add(bhTransaction.connection.ExecuteScalar<G>(insertCommand, entry, bhTransaction._transaction));
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Ids;
        }

        async Task<IList<T>> IBHDataProvider<T, G>.GetAllEntriesAsync()
        {
            List<T> entries = new List<T>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable}";

                    if (withActivator)
                    {
                        SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0";
                    }

                    var entr = await connection.QueryAsync<T>(SubCommand);
                    entries = entr.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        async Task<IList<T>> IBHDataProvider<T, G>.GetAllEntriesAsync(BHTransaction bhTransaction)
        {
            List<T> entries = new List<T>();

            try
            {     
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable}";

                if (withActivator)
                {
                    SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0";
                }

                var entr = await bhTransaction.connection.QueryAsync<T>(SubCommand, bhTransaction._transaction);
                entries = entr.ToList();   
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        async Task<IList<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>() where Dto : class
        {
            List<Dto> entries = new List<Dto>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));

                    string SubCommand = $"select {colsAndParams} from {ThisTable}";

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0";
                    }

                    var entr = await connection.QueryAsync<Dto>(SubCommand);
                    entries = entr.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        async Task<IList<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>(BHTransaction bhTransaction) where Dto : class
        {
            List<Dto> entries = new List<Dto>();

            try
            {
                string colsAndParams = CompareDtoToEntity(typeof(Dto));

                string SubCommand = $"select {colsAndParams} from {ThisTable}";

                if (withActivator)
                {
                    SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0";
                }

                var entr = await bhTransaction.connection.QueryAsync<Dto>(SubCommand,bhTransaction._transaction);
                entries = entr.ToList();
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        async Task<IList<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync()
        {
            List<T> entries = new List<T>();

            if (withActivator)
            {
                try
                {
                    using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                    {
                        string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=1";
                        var entr = await connection.QueryAsync<T>(SubCommand);
                        entries = entr.ToList();
                    }
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                }
            }

            return entries;
        }

        async Task<IList<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync(BHTransaction bhTransaction)
        {
            List<T> entries = new List<T>();

            if (withActivator)
            {
                try
                { 
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=1";
                    var entr = await bhTransaction.connection.QueryAsync<T>(SubCommand,bhTransaction._transaction);
                    entries = entr.ToList();        
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                }
            }

            return entries;
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryByIdAsync(G Id)
        {
            T? entry = default(T);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId}=@Id";
                    var Parameter = new { Id = Id };

                    if (withActivator)
                    {
                        SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId}=@Id and {ThisInactive}=0";
                    }

                    entry = await connection.QueryFirstOrDefaultAsync<T>(SubCommand, Parameter);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            T? entry = default(T);

            try
            {               
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId}=@Id";
                var Parameter = new { Id = Id };

                if (withActivator)
                {
                    SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId}=@Id and {ThisInactive}=0";
                }

                entry = await bhTransaction.connection.QueryFirstOrDefaultAsync<T>(SubCommand, Parameter,bhTransaction._transaction);   
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryByIdAsync<Dto>(G Id) where Dto : class
        {
            Dto? entry = default(Dto);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));

                    string SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId}=@Id";
                    var Parameter = new { Id = Id };

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId}=@Id and {ThisInactive}=0";
                    }

                    entry = await connection.QueryFirstOrDefaultAsync<Dto>(SubCommand, Parameter);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryByIdAsync<Dto>(G Id, BHTransaction bhTransaction) where Dto : class
        {
            Dto? entry = default(Dto);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));

                    string SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId}=@Id";
                    var Parameter = new { Id = Id };

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId}=@Id and {ThisInactive}=0";
                    }

                    entry = await connection.QueryFirstOrDefaultAsync<Dto>(SubCommand, Parameter);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            T? entry = default(T);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    entry = await connection.QueryFirstOrDefaultAsync<T>(SubCommand, sql.Parameters);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            T? entry = default(T);

            try
            { 
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                entry = await bhTransaction.connection.QueryFirstOrDefaultAsync<T>(SubCommand, sql.Parameters,bhTransaction._transaction);  
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            Dto? entry = default(Dto);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    entry = await connection.QueryFirstOrDefaultAsync<Dto>(SubCommand, sql.Parameters);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            Dto? entry = default(Dto);

            try
            {              
                string colsAndParams = CompareDtoToEntity(typeof(Dto));
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                entry = await bhTransaction.connection.QueryFirstOrDefaultAsync<Dto>(SubCommand, sql.Parameters,bhTransaction._transaction);
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entry;
        }

        async Task<IList<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            IList<T> entries = new List<T>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    var entr = await connection.QueryAsync<T>(SubCommand, sql.Parameters);
                    entries = entr.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        async Task<IList<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            IList<T> entries = new List<T>();

            try
            {            
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                var entr = await bhTransaction.connection.QueryAsync<T>(SubCommand, sql.Parameters,bhTransaction._transaction);
                entries = entr.ToList();            
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        async Task<IList<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            IList<Dto> entries = new List<Dto>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string colsAndParams = CompareDtoToEntity(typeof(Dto));
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    var entr = await connection.QueryAsync<Dto>(SubCommand, sql.Parameters);
                    entries = entr.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        async Task<IList<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            IList<Dto> entries = new List<Dto>();

            try
            {
                string colsAndParams = CompareDtoToEntity(typeof(Dto));
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                var entr = await bhTransaction.connection.QueryAsync<Dto>(SubCommand, sql.Parameters,bhTransaction._transaction);
                entries = entr.ToList();     
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return entries;
        }

        async Task<G?> IBHDataProvider<T, G>.InsertEntryAsync(T entry)
        {
            G? Id = default(G);
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    if (requiredId)
                    {
                        Id = GenerateId();
                        insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                        await connection.ExecuteAsync(insertCommand, entry);
                    }
                    else
                    {
                        Id = await connection.ExecuteScalarAsync<G>(insertCommand, entry);
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        async Task<G?> IBHDataProvider<T, G>.InsertEntryAsync(T entry, BHTransaction bhTransaction)
        {
            G? Id = default(G);
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    if (requiredId)
                    {
                        Id = GenerateId();
                        insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                        await connection.ExecuteAsync(insertCommand, entry);
                    }
                    else
                    {
                        Id = await connection.ExecuteScalarAsync<G>(insertCommand, entry);
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        async Task<List<G?>> IBHDataProvider<T, G>.InsertEntriesAsync(List<T> entries)
        {
            List<G?> Ids = new List<G?>();
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    connection.Open();
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (T entry in entries)
                        {
                            if (requiredId)
                            {
                                G? Id = GenerateId();
                                insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                                await connection.ExecuteAsync(insertCommand, entry, transaction);
                                Ids.Add(Id);
                            }
                            else
                            {
                                Ids.Add(await connection.ExecuteScalarAsync<G>(insertCommand, entry, transaction));
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Ids;
        }

        async Task<List<G?>> IBHDataProvider<T, G>.InsertEntriesAsync(List<T> entries, BHTransaction bhTransaction)
        {
            List<G?> Ids = new List<G?>();
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";
            try
            {
                foreach (T entry in entries)
                {
                    if (requiredId)
                    {
                        G? Id = GenerateId();
                        insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                        await bhTransaction.connection.ExecuteAsync(insertCommand, entry, bhTransaction._transaction);
                        Ids.Add(Id);
                    }
                    else
                    {
                        Ids.Add(await bhTransaction.connection.ExecuteScalarAsync<G>(insertCommand, entry, bhTransaction._transaction));
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Ids;
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryById(T entry)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId}=@Id";
            return await Update(entry, updateCommand);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryById(T entry, BHTransaction bhTransaction)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId}=@Id";
            return await Update(entry, updateCommand, bhTransaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryById<Columns>(T entry) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id";
            return await Update(entry, updateCommand);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryById<Columns>(T entry, BHTransaction bhTransaction) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id";
            return await Update(entry, updateCommand, bhTransaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesById(List<T> entries)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId}=@Id";
            return await UpdateMany(entries, updateCommand);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesById(List<T> entries, BHTransaction bhTransaction)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId}=@Id";
            return await UpdateMany(entries, updateCommand, bhTransaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesById<Columns>(List<T> entries) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id";
            return await UpdateMany(entries, updateCommand);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesById<Columns>(List<T> entries, BHTransaction bhTransaction) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id";
            return await UpdateMany(entries, updateCommand, bhTransaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            bool success = false;
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string updateCommand = $"update {ThisTable} set {UpdateParams} where {sql.Columns}";
                    ColumnsAndParameters additionalSql = AdditionalParameters(sql, entry);

                    if (withActivator)
                    {
                        updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    await connection.ExecuteAsync(updateCommand, additionalSql.Parameters);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction bhTransaction)
        {
            bool success = false;
            try
            {
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string updateCommand = $"update {ThisTable} set {UpdateParams} where {sql.Columns}";
                ColumnsAndParameters additionalSql = AdditionalParameters(sql, entry);

                if (withActivator)
                {
                    updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisInactive}=0 and {sql.Columns}";
                }

                await bhTransaction.connection.ExecuteAsync(updateCommand, additionalSql.Parameters,bhTransaction._transaction);
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            bool success = false;
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}";
                    ColumnsAndParameters additionalSql = AdditionalParameters(sql, entry);

                    if (withActivator)
                    {
                        updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    await connection.ExecuteAsync(updateCommand, additionalSql.Parameters);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry,BHTransaction bhTransaction) where Columns : class
        {
            bool success = false;
            try
            {
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}";
                ColumnsAndParameters additionalSql = AdditionalParameters(sql, entry);

                if (withActivator)
                {
                    updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive}=0 and {sql.Columns}";
                }

                await bhTransaction.connection.ExecuteAsync(updateCommand, additionalSql.Parameters,bhTransaction._transaction);
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteAllEntries()
        {
            bool success = false;
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string SubCommand = $"Delete from {ThisTable}";

                    if (withActivator)
                    {
                        SubCommand = $"Update {ThisTable} set {ThisInactive}=1 where {ThisInactive}=0";
                    }

                    await connection.ExecuteAsync(SubCommand);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteAllEntries(BHTransaction bhTransaction)
        {
            bool success = false;
            try
            {               
                string SubCommand = $"Delete from {ThisTable}";

                if (withActivator)
                {
                    SubCommand = $"Update {ThisTable} set {ThisInactive}=1 where {ThisInactive}=0";
                }

                await bhTransaction.connection.ExecuteAsync(SubCommand,bhTransaction._transaction);
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntryById(G id)
        {
            bool success = false;
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    string SubCommand = $"delete from {ThisTable} where {ThisId}=@Id";

                    if (withActivator)
                    {
                        SubCommand = $"Update {ThisTable} set {ThisInactive}=1 where {ThisId}=@Id";
                    }

                    var Parameter = new { Id = id };
                    await connection.ExecuteAsync(SubCommand, Parameter);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntryById(G id, BHTransaction bhTransaction)
        {
            bool success = false;
            try
            {
                string SubCommand = $"delete from {ThisTable} where {ThisId}=@Id";

                if (withActivator)
                {
                    SubCommand = $"Update {ThisTable} set {ThisInactive}=1 where {ThisId}=@Id";
                }

                var Parameter = new { Id = id };
                await bhTransaction.connection.ExecuteAsync(SubCommand, Parameter,bhTransaction._transaction);
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteInactiveEntryById(G id)
        {
            bool success = false;
            if (withActivator)
            {
                try
                {
                    using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                    {
                        string SubCommand = $"delete from {ThisTable} where {ThisId}=@Id and {ThisInactive}=1";
                        var Parameter = new { Id = id };
                        await connection.ExecuteAsync(SubCommand, Parameter);
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                    success = false;
                }
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteInactiveEntryById(G id, BHTransaction bhTransaction)
        {
            bool success = false;
            if (withActivator)
            {
                try
                {
                    string SubCommand = $"delete from {ThisTable} where {ThisId}=@Id and {ThisInactive}=1";
                    var Parameter = new { Id = id };
                    await bhTransaction.connection.ExecuteAsync(SubCommand, Parameter,bhTransaction._transaction);
                    
                    success = true;
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                    success = false;
                }
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.ReactivateEntryById(G id, BHTransaction bhTransaction)
        {
            bool success = false;
            if (withActivator)
            {
                try
                {
                    string SubCommand = $"update {ThisTable} set {ThisInactive}=0 where {ThisId}=@Id and {ThisInactive}=1";
                    var Parameter = new { Id = id };
                    await bhTransaction.connection.ExecuteAsync(SubCommand, Parameter,bhTransaction._transaction);
                    
                    success = true;
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                    success = false;
                }
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.ReactivateEntryById(G id)
        {
            bool success = false;
            if (withActivator)
            {
                try
                {
                    using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                    {
                        string SubCommand = $"update {ThisTable} set {ThisInactive}=0 where {ThisId}=@Id and {ThisInactive}=1";
                        var Parameter = new { Id = id };
                        await connection.ExecuteAsync(SubCommand, Parameter);
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                    success = false;
                }
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            bool success = false;
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string SubCommand = $"delete from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        SubCommand = $"Update {ThisTable} set {ThisInactive}=1 where {sql.Columns}";
                    }

                    await connection.ExecuteAsync(SubCommand, sql.Parameters);
                }
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            bool success = false;
            try
            {    
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string SubCommand = $"delete from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    SubCommand = $"Update {ThisTable} set {ThisInactive}=1 where {sql.Columns}";
                }

                await bhTransaction.connection.ExecuteAsync(SubCommand, sql.Parameters,bhTransaction._transaction);
                
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        async Task<G?> IBHDataProvider<T, G>.GetIdWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetIdFromPredicate(predicate);
        }

        async Task<G?> IBHDataProvider<T, G>.GetIdWhereAsync(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            return await GetIdFromPredicate(predicate,bhTransaction);
        }

        async Task<List<G>> IBHDataProvider<T, G>.GetIdsWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetIdsFromPredicate(predicate);
        }

        async Task<List<G>> IBHDataProvider<T, G>.GetIdsWhereAsync(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            return await GetIdsFromPredicate(predicate, bhTransaction);
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.InnerJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return CreateFirstJoin<TOther, Dto>(key, otherKey, "inner");
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.OuterJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return CreateFirstJoin<TOther, Dto>(key, otherKey, "outer");
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.LeftJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return CreateFirstJoin<TOther, Dto>(key, otherKey, "left");
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.RightJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return CreateFirstJoin<TOther, Dto>(key, otherKey, "right");
        }

        private JoinsData<Dto, T, TOther> CreateFirstJoin<TOther, Dto>(LambdaExpression key, LambdaExpression otherKey, string joinType)
        {
            string? parameter = key.Parameters[0].Name;
            MemberExpression? member = key.Body as MemberExpression;
            string? propName = member?.Member.Name;
            string? parameterOther = otherKey.Parameters[0].Name;
            MemberExpression? memberOther = otherKey.Body as MemberExpression;
            string? propNameOther = memberOther?.Member.Name;

            JoinsData<Dto, T, TOther> firstJoin = new JoinsData<Dto, T, TOther>();
            firstJoin.isMyShit = isMyShit;
            firstJoin.BaseTable = typeof(T);
            firstJoin.TablesToLetters.Add(new TableLetters { Table = typeof(T), Letter = parameter });
            firstJoin.Letters.Add(parameter);
            firstJoin.Ignore = false;

            if (parameterOther == parameter)
            {
                parameterOther += firstJoin.HelperIndex.ToString();
                firstJoin.HelperIndex++;
            }

            firstJoin.TablesToLetters.Add(new TableLetters { Table = typeof(TOther), Letter = parameterOther });
            firstJoin.Letters.Add(parameterOther);

            firstJoin.Joins = $" {joinType} join {MyShit(typeof(TOther).Name)} {parameterOther} on {parameterOther}.{MyShit(propNameOther)} = {parameter}.{MyShit(propName)}";
            firstJoin.OccupiedDtoProps = BindPropertiesToDto(typeof(TOther), typeof(Dto), parameter, parameterOther);
            return firstJoin;
        }

        private List<PropertyOccupation> BindPropertiesToDto(Type otherTable, Type dto, string? paramA, string? paramB)
        {
            List<PropertyOccupation> result = new List<PropertyOccupation>();
            List<string> OtherPropNames = new List<string>();

            foreach (PropertyInfo otherProp in otherTable.GetProperties())
            {
                OtherPropNames.Add(otherProp.Name);
            }

            foreach (PropertyInfo property in dto.GetProperties())
            {
                PropertyOccupation occupation = new PropertyOccupation();

                if (Columns.Contains(property.Name))
                {
                    Type? TpropType = typeof(T).GetProperty(property.Name)?.PropertyType;

                    if (TpropType == property.PropertyType)
                    {
                        occupation = new PropertyOccupation
                        {
                            PropName = property.Name,
                            PropType = property.PropertyType,
                            Occupied = true,
                            TableLetter = paramA,
                            TableProperty = property.Name,
                            TablePropertyType = TpropType,
                            WithCast = 0
                        };
                    }
                }

                if (OtherPropNames.Contains(property.Name) && !occupation.Occupied)
                {
                    Type? TOtherPropType = otherTable.GetProperty(property.Name)?.PropertyType;

                    if (TOtherPropType == property.PropertyType)
                    {
                        occupation = new PropertyOccupation
                        {
                            PropName = property.Name,
                            PropType = property.PropertyType,
                            Occupied = true,
                            TableLetter = paramB,
                            TableProperty = property.Name,
                            TablePropertyType = TOtherPropType,
                            WithCast = 0
                        };
                    }
                }

                if (!occupation.Occupied)
                {
                    occupation = new PropertyOccupation
                    {
                        PropName = property.Name,
                        PropType = property.PropertyType,
                        Occupied = false,
                        WithCast = 0
                    };
                }

                result.Add(occupation);
            }

            return result;
        }

        private async Task<bool> Update(T entry, string updateCommand)
        {
            bool success = false;
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    await connection.ExecuteAsync(updateCommand, entry);
                }
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        private async Task<bool> Update(T entry, string updateCommand, BHTransaction bhTransaction)
        {
            bool success = false;
            try
            {
                await bhTransaction.connection.ExecuteAsync(updateCommand, entry, bhTransaction._transaction);
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        private async Task<bool> UpdateMany(List<T> entries, string updateCommand)
        {
            bool success = false;
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    connection.Open();
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (T entry in entries)
                        {
                            await connection.ExecuteAsync(updateCommand, entry,transaction);
                        }
                        transaction.Commit();
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        private async Task<bool> UpdateMany(List<T> entries, string updateCommand,BHTransaction bhTransaction)
        {
            bool success = false;
            try
            {
                foreach (T entry in entries)
                {
                    await bhTransaction.connection.ExecuteAsync(updateCommand, entry, bhTransaction._transaction);
                }
                success = true;
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                success = false;
            }
            return success;
        }

        private async Task<G?> GetIdFromPredicate(Expression<Func<T, bool>> predicate)
        {
            G? Id = default(G);

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    Id = await connection.QueryFirstOrDefaultAsync<G>(selectCommand, sql.Parameters);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private async Task<G?> GetIdFromPredicate(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            G? Id = default(G);

            try
            {
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                Id = await bhTransaction.connection.QueryFirstOrDefaultAsync<G>(selectCommand, sql.Parameters,bhTransaction._transaction); 
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private async Task<List<G>> GetIdsFromPredicate(Expression<Func<T, bool>> predicate)
        {
            List<G> Id = new List<G>();

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                    ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                    string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

                    if (withActivator)
                    {
                        selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                    }

                    var Ids = await connection.QueryAsync<G>(selectCommand, sql.Parameters);
                    Id = Ids.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private async Task<List<G>> GetIdsFromPredicate(Expression<Func<T, bool>> predicate,BHTransaction bhTransaction)
        {
            List<G> Id = new List<G>();

            try
            { 
                List<ExpressionsData> expressionTree = SplitMembers(predicate.Body);
                ColumnsAndParameters sql = ExpressionTreeToSql(expressionTree);
                string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

                if (withActivator)
                {
                    selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
                }

                var Ids = await bhTransaction.connection.QueryAsync<G>(selectCommand, sql.Parameters,bhTransaction._transaction);
                Id = Ids.ToList();
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private string MyShit(string? propName)
        {
            string result = propName ?? string.Empty;

            if (!isMyShit)
            {
                result = $@"""{propName}""";
            }

            return result;
        }

        private List<ExpressionsData> SplitMembers(Expression expression)
        {
            List<ExpressionsData> expressionTree = new List<ExpressionsData>();

            BinaryExpression? operation = expression as BinaryExpression;
            BinaryExpression? currentOperation = operation;
            MemberExpression? leftMember = currentOperation?.Left as MemberExpression;
            MemberExpression? rightMember = currentOperation?.Right as MemberExpression;

            int currentIndx = 0;
            bool startTranslate = false;

            if (operation != null)
            {
                startTranslate = true;

                expressionTree.Add(new ExpressionsData()
                {
                    operation = operation,
                    leftMember = operation?.Left as MemberExpression,
                    rightMember = operation?.Right as MemberExpression,
                    expressionType = operation != null ? operation.NodeType : ExpressionType.Default,
                    rightChecked = false,
                    leftChecked = false,
                    memberValue = null
                });
            }

            while (startTranslate)
            {
                if (expressionTree[currentIndx].operation != null)
                {
                    if (expressionTree[currentIndx].expressionType == ExpressionType.AndAlso || expressionTree[currentIndx].expressionType == ExpressionType.OrElse)
                    {
                        bool addTotree = false;

                        if (!expressionTree[currentIndx].leftChecked)
                        {
                            currentOperation = expressionTree[currentIndx].operation?.Left as BinaryExpression;
                            expressionTree[currentIndx].leftChecked = true;
                            addTotree = true;
                        }
                        else if (!expressionTree[currentIndx].rightChecked && expressionTree[currentIndx].leftChecked)
                        {
                            currentOperation = expressionTree[currentIndx].operation?.Right as BinaryExpression;
                            expressionTree[currentIndx].rightChecked = true;
                            addTotree = true;
                        }
                        else
                        {
                            currentIndx -= 1;
                        }

                        if (addTotree)
                        {
                            expressionTree.Add(new ExpressionsData()
                            {
                                operation = currentOperation,
                                leftMember = currentOperation?.Left as MemberExpression,
                                rightMember = currentOperation?.Right as MemberExpression,
                                expressionType = currentOperation != null ? currentOperation.NodeType : ExpressionType.Default,
                                rightChecked = false,
                                leftChecked = false,
                                memberValue = null,
                                parentIndex = currentIndx
                            });

                            currentIndx = expressionTree.Count - 1;
                        }
                    }
                    else
                    {
                        if (!expressionTree[currentIndx].leftChecked || !expressionTree[currentIndx].rightChecked)
                        {
                            rightMember = currentOperation?.Right as MemberExpression;
                            ConstantExpression? rightConstant = currentOperation?.Right as ConstantExpression;
                            BinaryExpression? rightBinary = currentOperation?.Right as BinaryExpression;

                            expressionTree[currentIndx].leftChecked = true;
                            expressionTree[currentIndx].rightChecked = true;

                            object? value = null;

                            if (rightMember != null)
                            {
                                value = Expression.Lambda(rightMember).Compile().DynamicInvoke();
                            }

                            if (rightConstant != null)
                            {
                                value = rightConstant?.Value;
                            }

                            if (rightBinary != null)
                            {
                                value = Expression.Lambda(rightBinary).Compile().DynamicInvoke();
                            }

                            expressionTree[currentIndx].memberValue = value;
                        }

                        currentIndx -= 1;
                    }
                }

                if (currentIndx < 0)
                {
                    startTranslate = false;
                }
            }

            return expressionTree;
        }

        private ColumnsAndParameters ExpressionTreeToSql(List<ExpressionsData> data)
        {
            string result = "";
            DynamicParameters parameters = new DynamicParameters();
            List<ExpressionsData> children = data.Where(x => x.memberValue != null).ToList();
            string[] translations = new string[children.Count];
            int index = 0;
            foreach (ExpressionsData child in children)
            {
                ExpressionsData parent = data[child.parentIndex];
                if (parent.leftChecked)
                {
                    ColumnAndParameter childParams = TranslateExpression(child, index);

                    if (childParams.ParamName != string.Empty)
                    {
                        parameters.Add(@childParams.ParamName, childParams.Value);
                    }

                    parent.sqlCommand = $"{childParams.Column}";
                    parent.leftChecked = false;
                    index++;
                }
                else
                {
                    ColumnAndParameter parentCols = TranslateExpression(parent, index);

                    if (parentCols.ParamName != string.Empty)
                    {
                        parameters.Add(@parentCols.ParamName, parentCols.Value);
                    }

                    index++;

                    ColumnAndParameter childCols = TranslateExpression(child, index);

                    if (childCols.ParamName != string.Empty)
                    {
                        parameters.Add(@childCols.ParamName, childCols.Value);
                    }

                    parent.sqlCommand = $"({parent.sqlCommand} {parentCols.Column} {childCols.Column})";

                    index++;
                }
            }

            List<ExpressionsData> parents = data.Where(x => x.memberValue == null).ToList();

            if (parents.Count > 1)
            {
                parents.RemoveAt(0);
                int parentsCount = parents.Count;

                for (int i = 0; i < parentsCount; i++)
                {
                    ExpressionsData parent = data[parents[parentsCount - 1 - i].parentIndex];

                    if (parent.leftChecked)
                    {
                        parent.sqlCommand = parents[parentsCount - 1 - i].sqlCommand;
                        parent.leftChecked = false;
                    }
                    else
                    {
                        ColumnAndParameter parentParams = TranslateExpression(parent, index);
                        if (parentParams.ParamName != string.Empty)
                        {
                            parameters.Add(@parentParams.ParamName, parentParams.Value);
                        }
                        parent.sqlCommand = $"({parent.sqlCommand} {parentParams.Column} {parents[parentsCount - 1 - i].sqlCommand})";
                        index++;
                    }
                }
            }

            result = data[0].sqlCommand;

            return new ColumnsAndParameters { Columns = result, Parameters = parameters, Count = index };
        }

        private ColumnAndParameter TranslateExpression(ExpressionsData expression, int index)
        {
            string? column = string.Empty;
            string? parameter = string.Empty;
            object? value = new object();
            string[]? variable = new string[2];

            switch (expression.expressionType)
            {
                case ExpressionType.AndAlso:
                    column = " and ";
                    break;
                case ExpressionType.OrElse:
                    column = " or ";
                    break;
                case ExpressionType.Equal:
                    value = expression?.memberValue;
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{MyShit(variable?[1])} = @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{MyShit(variable?[1])} is @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{MyShit(variable?[1])} >= @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThanOrEqual:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{MyShit(variable?[1])} <= @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.LessThan:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{MyShit(variable?[1])} < @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.GreaterThan:
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{MyShit(variable?[1])} > @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
                    break;
                case ExpressionType.NotEqual:
                    value = expression?.memberValue;
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{MyShit(variable?[1])} != @{variable?[1]}{index}";
                    if (value == null)
                    {
                        column = $"{MyShit(variable?[1])} is not @{variable?[1]}{index}";
                    }
                    parameter = $"{variable?[1]}{index}";
                    break;
            }

            return new ColumnAndParameter { Column = column, ParamName = parameter, Value = value };
        }

        private ColumnsAndParameters AdditionalParameters(ColumnsAndParameters colsAndParams, object item)
        {
            Type type = item.GetType();
            PropertyInfo[] props = type.GetProperties();

            foreach (var prop in props)
            {
                colsAndParams.Parameters.Add(@prop.Name, prop.GetValue(item));
            }

            return colsAndParams;
        }

        private string CompareDtoToEntity(Type dto)
        {
            string columns = string.Empty;

            foreach (PropertyInfo property in dto.GetProperties())
            {
                if (Columns.Contains(property.Name)
                    && typeof(T).GetProperty(property.Name)?.PropertyType == property.PropertyType)
                {
                    columns += $",{MyShit(property.Name)}";
                }
            }

            columns += " ";
            columns = columns.Remove(0, 1);

            return columns;
        }

        private string CompareColumnsToEntity(Type dto)
        {
            string columns = string.Empty;

            foreach (PropertyInfo property in dto.GetProperties())
            {
                if (property.Name != "Id" && Columns.Contains(property.Name)
                    && typeof(T).GetProperty(property.Name)?.PropertyType == property.PropertyType)
                {
                    columns += $",{MyShit(property.Name)}=@{property.Name}";
                }
            }

            columns += " ";
            columns = columns.Remove(0, 1);

            return columns;
        }

        private G? GenerateId()
        {
            object? value = null;

            switch (idType)
            {
                case BHIdTypes.GuidId:
                    value = Guid.NewGuid();
                    break;
                case BHIdTypes.StringId:
                    string ToHash = Guid.NewGuid().ToString()+DateTime.Now.ToString();
                    value = ToHash.GenerateSHA1();
                    break;
            }

            return (G?)value;
        }
    }
}

