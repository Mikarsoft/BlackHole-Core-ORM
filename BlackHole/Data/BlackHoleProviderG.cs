using BlackHole.Interfaces;
using BlackHole.Entities;
using System.Reflection;
using System.Data;
using Dapper;
using BlackHole.Services;
using System.Linq.Expressions;
using BlackHole.Attributes.EntityAttributes;

namespace BlackHole.Data
{
    public class BlackHoleProviderG <T> : IBlackHoleProviderG<T> where T : BlackHoleEntityG
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
        private string OutputIdMiddle { get; set; }
        private string OutputIdEnding { get; set; }
        private string OriginalTableName { get; set; }

        private IBHDatabaseSelector _multiDatabaseSelector;
        private ILoggerService _loggerService;

        /// <summary>
        /// Create a Data Provider that Automatically Communicates with the Database Using the BlazarEntity you pass in.
        /// </summary>
        public BlackHoleProviderG()
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

            string[] insertOutputs = _multiDatabaseSelector.IdOutput(ThisTable, ThisId,true);
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

        /// <summary>
        /// Get All Entries from the Table in the Database
        /// </summary>
        /// <returns></returns>
        IList<T> IBlackHoleProviderG<T>.GetAllEntries()
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        IList<Dto> IBlackHoleProviderG<T>.GetAllEntries<Dto>() where Dto : class
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

        /// <summary>
        /// Get All Inactive Entries from the Table in Database
        /// mikarsoft.com
        /// </summary>
        /// <returns></returns>
        IList<T> IBlackHoleProviderG<T>.GetAllInactiveEntries()
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

        /// <summary>
        /// Get an Entry by its Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        T? IBlackHoleProviderG<T>.GetEntryById(Guid Id)
        {
            T? entry = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        Dto? IBlackHoleProviderG<T>.GetEntryById<Dto>(Guid Id) where Dto : class
        {
            Dto? entry = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T? IBlackHoleProviderG<T>.GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            T? entry = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Dto? IBlackHoleProviderG<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            Dto? entry = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<T> IBlackHoleProviderG<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate)
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<Dto> IBlackHoleProviderG<T>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        Guid IBlackHoleProviderG<T>.InsertEntry(T entry)
        {
            if (isMyShit)
            {
                return InsertShit(entry);
            }

            string insertCommandn = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";
            return Insert(entry, insertCommandn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        List<Guid> IBlackHoleProviderG<T>.InsertEntries(List<T> entries)
        {
            List<Guid> Ids = new List<Guid>();
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";

            foreach (T entry in entries)
            {
                if (isMyShit)
                {
                    Ids.Add(InsertShit(entry));
                }
                else
                {
                    Ids.Add(Insert(entry, insertCommand));
                }
            }

            return Ids;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        async Task<IList<T>> IBlackHoleProviderG<T>.GetAllEntriesAsync()
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        async Task<IList<Dto>> IBlackHoleProviderG<T>.GetAllEntriesAsync<Dto>() where Dto : class
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        async Task<IList<T>> IBlackHoleProviderG<T>.GetAllInactiveEntriesAsync()
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        async Task<T?> IBlackHoleProviderG<T>.GetEntryByIdAsync(Guid Id)
        {
            T? entry = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        async Task<Dto?> IBlackHoleProviderG<T>.GetEntryByIdAsync<Dto>(int Id) where Dto : class
        {
            Dto? entry = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        async Task<T?> IBlackHoleProviderG<T>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            T? entry = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        async Task<Dto?> IBlackHoleProviderG<T>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            Dto? entry = null;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        async Task<IList<T>> IBlackHoleProviderG<T>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        async Task<IList<Dto>> IBlackHoleProviderG<T>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        async Task<Guid> IBlackHoleProviderG<T>.InsertEntryAsync(T entry)
        {
            if (isMyShit)
            {
                return await InsertShitAsync(entry);
            }

            string insertCommandn = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";
            return await InsertAsync(entry, insertCommandn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        async Task<List<Guid>> IBlackHoleProviderG<T>.InsertEntriesAsync(List<T> entries)
        {
            List<Guid> Ids = new List<Guid>();
            string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}) {OutputIdMiddle} values ({PropertyParams},0) {OutputIdEnding}";

            foreach (T entry in entries)
            {
                if (isMyShit)
                {
                    Ids.Add(await InsertShitAsync(entry));
                }
                else
                {
                    Ids.Add(await InsertAsync(entry, insertCommand));
                }
            }

            return Ids;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        async Task IBlackHoleProviderG<T>.UpdateEntryById(T entry)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId}=@Id";
            await Update(entry, updateCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Columns"></typeparam>
        /// <param name="entry"></param>
        async Task IBlackHoleProviderG<T>.UpdateEntryById<Columns>(T entry) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id";
            await Update(entry, updateCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        async Task IBlackHoleProviderG<T>.UpdateEntriesById(List<T> entries)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId}=@Id";

            foreach (T entry in entries)
            {
                await Update(entry, updateCommand);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Columns"></typeparam>
        /// <param name="entries"></param>
        async Task IBlackHoleProviderG<T>.UpdateEntriesById<Columns>(List<T> entries) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id";

            foreach (T entry in entries)
            {
                await Update(entry, updateCommand);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="entry"></param>
        async Task IBlackHoleProviderG<T>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
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
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Columns"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="entry"></param>
        async Task IBlackHoleProviderG<T>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
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
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }
        }

        /// <summary>
        /// Delete all Entries of the Table in Database
        /// </summary>
        async Task IBlackHoleProviderG<T>.DeleteAllEntries()
        {
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
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }
        }

        /// <summary>
        /// Delete Entry in the Table with the specific Id number
        /// </summary>
        /// <param name="id"></param>
        async Task IBlackHoleProviderG<T>.DeleteEntryById(Guid id)
        {
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
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        async Task IBlackHoleProviderG<T>.DeleteInactiveEntryById(Guid id)
        {
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
                }
                catch (Exception ex)
                {
                    _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <exception cref="NotImplementedException"></exception>
        async Task IBlackHoleProviderG<T>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate)
        {
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
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        async Task<Guid> IBlackHoleProviderG<T>.GetIdWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetIdFromPredicate(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        async Task<List<Guid>> IBlackHoleProviderG<T>.GetIdsWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetIdsFromPredicate(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        JoinsData<Dto, T, TOther> IBlackHoleProviderG<T>.InnerJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            //Type propertyType = key.Body.Type;

            return CreateFirstJoin<TOther, Dto>(key, otherKey, "inner");
        }

        JoinsData<Dto, T, TOther> IBlackHoleProviderG<T>.OuterJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return CreateFirstJoin<TOther, Dto>(key, otherKey, "outer");
        }

        JoinsData<Dto, T, TOther> IBlackHoleProviderG<T>.LeftJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return CreateFirstJoin<TOther, Dto>(key, otherKey, "left");
        }

        JoinsData<Dto, T, TOther> IBlackHoleProviderG<T>.RightJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
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
                            WithCast = false
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
                            WithCast = false
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
                        WithCast = false
                    };
                }

                result.Add(occupation);
            }

            return result;
        }

        private async Task<Guid> InsertShitAsync(T entry)
        {
            Guid Id = Guid.NewGuid();

            try
            {
                string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    await connection.ExecuteAsync(insertCommand, entry);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private Guid InsertShit(T entry)
        {
            Guid Id = Guid.NewGuid();

            try
            {
                string insertCommand = $"insert into {ThisTable} ({PropertyNames},{ThisInactive},Id) values ({PropertyParams},0,'{Id}');";
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    connection.Execute(insertCommand, entry);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private async Task<Guid> InsertAsync(T entry, string insertCommand)
        {
            Guid Id = Guid.Empty;

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    Id = await connection.QuerySingleAsync<Guid>(insertCommand, entry);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private Guid Insert(T entry, string insertCommand)
        {
            Guid Id = Guid.Empty;

            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    Id = connection.ExecuteScalar<Guid>(insertCommand, entry);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private async Task Update(T entry, string updateCommand)
        {
            try
            {
                using (IDbConnection connection = _multiDatabaseSelector.GetConnection())
                {
                    await connection.ExecuteAsync(updateCommand, entry);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }
        }

        private async Task<Guid> GetIdFromPredicate(Expression<Func<T, bool>> predicate)
        {
            Guid Id = Guid.Empty;

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

                    Id = await connection.QueryFirstOrDefaultAsync<Guid>(selectCommand, sql.Parameters);
                }
            }
            catch (Exception ex)
            {
                _loggerService.CreateErrorLogs($"DataProvider_{OriginalTableName}", ex.Message, ex.ToString());
            }

            return Id;
        }

        private async Task<List<Guid>> GetIdsFromPredicate(Expression<Func<T, bool>> predicate)
        {
            List<Guid> Id = new List<Guid>();

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

                    var Ids = await connection.QueryAsync<Guid>(selectCommand, sql.Parameters);
                    Id = Ids.ToList();
                }
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
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{MyShit(variable?[1])} = @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
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
                    variable = expression?.leftMember?.ToString().Split(".");
                    column = $"{MyShit(variable?[1])} != @{variable?[1]}{index}";
                    parameter = $"{variable?[1]}{index}";
                    value = expression?.memberValue;
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
    }
}

