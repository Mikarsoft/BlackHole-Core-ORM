using BlackHole.Entities;
using System.Reflection;
using System.Linq.Expressions;
using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    /// <summary>
    /// Makes all the communication between the Datbase Table and The Specified Entity
    /// </summary>
    /// <typeparam name="T">Black Hole Entity with Integer Id</typeparam>
    /// <typeparam name="G">The type of Entity's Id</typeparam>
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

        private IBHDataProviderSelector _dataProviderSelector;
        private IDataProvider _dataProvider;

        /// <summary>
        /// Create a Data Provider that Automatically Communicates with the Database Using the Black Hole Entity you pass in.
        /// </summary>
        public BHDataProvider()
        {
            Type _type = typeof(T);
            string name = _type.Name;

            var attributes = _type.GetCustomAttributes(true);
            var attribute = attributes.SingleOrDefault(x => x.GetType() == typeof(UseActivator));

            if (attribute != null)
            {
                withActivator = true;
            }

            _dataProviderSelector = new BHDataProviderSelector();
            _dataProvider = _dataProviderSelector.GetDataProvider(typeof(G),name);
            isMyShit = _dataProvider.SkipQuotes();

            ThisTable = $"{_dataProviderSelector.GetDatabaseSchema()}{MyShit(name)}";
            ThisId = MyShit("Id");
            ThisInactive = MyShit("Inactive");

            IList<PropertyInfo> props = new List<PropertyInfo>(_type.GetProperties());

            foreach (PropertyInfo prop in props)
            {

                if (prop.Name != "Inactive")
                {
                    if (prop.Name != "Id")
                    {
                        string property = MyShit(prop.Name);
                        PropertyNames += $", {property}";
                        PropertyParams += $", @{prop.Name}";
                        UpdateParams += $",{property} = @{prop.Name}";
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

        List<T> IBHDataProvider<T, G>.GetAllEntries()
        {
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0";
            }

            return _dataProvider.Query<T>(SubCommand, null);
        }

        List<T> IBHDataProvider<T, G>.GetAllEntries(BHTransaction bhTransaction)
        {
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0";
            }

            return _dataProvider.Query<T>(SubCommand, null, bhTransaction.transaction);
        }

        List<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>() where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));

            string SubCommand = $"select {colsAndParams} from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0";
            }

            return _dataProvider.Query<Dto>(SubCommand, null);
        }

        List<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>(BHTransaction bhTransaction) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));

            string SubCommand = $"select {colsAndParams} from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0";
            }

            return _dataProvider.Query<Dto>(SubCommand, null, bhTransaction.transaction);
        }

        List<T> IBHDataProvider<T, G>.GetAllInactiveEntries()
        {
            List<T> entries = new List<T>();

            if (withActivator)
            {
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 1";
                entries = _dataProvider.Query<T>(SubCommand, null);
            }

            return entries;
        }

        List<T> IBHDataProvider<T, G>.GetAllInactiveEntries(BHTransaction bhTransaction)
        {
            List<T> entries = new List<T>();

            if (withActivator)
            {
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 1";
                entries = _dataProvider.Query<T>(SubCommand, null, bhTransaction.transaction);
            }

            return entries;
        }

        T? IBHDataProvider<T, G>.GetEntryById(G Id)
        {
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id";

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0";
            }

            return _dataProvider.QueryFirst<T>(SubCommand, parameters.Parameters);
        }

        T? IBHDataProvider<T, G>.GetEntryById(G Id, BHTransaction bhTransaction)
        {
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id";

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0";
            }

            return _dataProvider.QueryFirst<T>(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        Dto? IBHDataProvider<T, G>.GetEntryById<Dto>(G Id) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId} = @Id";

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0";
            }

            return _dataProvider.QueryFirst<Dto>(SubCommand, parameters.Parameters);
        }

        Dto? IBHDataProvider<T, G>.GetEntryById<Dto>(G Id, BHTransaction bhTransaction) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId} = @Id";

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0";
            }

            return _dataProvider.QueryFirst<Dto>(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null , 0);
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.QueryFirst<T>(SubCommand, sql.Parameters);
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.QueryFirst<T>(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.QueryFirst<Dto>(SubCommand, sql.Parameters);
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.QueryFirst<Dto>(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        List<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.Query<T>(SubCommand, sql.Parameters);
        }

        List<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.Query<T>(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        List<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.Query<Dto>(SubCommand, sql.Parameters);
        }

        List<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.Query<Dto>(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        G? IBHDataProvider<T, G>.InsertEntry(T entry)
        {
            string commandStart = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}";
            string commandEnd = $"values ({PropertyParams}, 0";
            return _dataProvider.InsertScalar<T, G>(commandStart,commandEnd, entry);
        }

        G? IBHDataProvider<T, G>.InsertEntry(T entry, BHTransaction bhTransaction)
        {
            string commandStart = $"insert into {ThisTable} ({PropertyNames}, {ThisInactive}";
            string commandEnd = $"values ({PropertyParams}, 0";
            return _dataProvider.InsertScalar<T, G>(commandStart, commandEnd, entry, bhTransaction.transaction);
        }

        List<G?> IBHDataProvider<T, G>.InsertEntries(List<T> entries)
        {
            string commandStart = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}";
            string commandEnd = $"values ({PropertyParams}, 0";
            List<G?> Ids = new List<G?>();

            using(BlackHoleTransaction bhTransaction = new BlackHoleTransaction())
            {
                Ids = _dataProvider.MultiInsertScalar<T, G>(commandStart, commandEnd, entries, bhTransaction);
            }

            return Ids;
        }

        List<G?> IBHDataProvider<T, G>.InsertEntries(List<T> entries, BHTransaction bhTransaction)
        {
            string commandStart = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}";
            string commandEnd = $"values ({PropertyParams}, 0";
            return _dataProvider.MultiInsertScalar<T, G>(commandStart, commandEnd, entries, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById(T entry)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id";
            return _dataProvider.ExecuteEntry(updateCommand, entry);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById(T entry, BHTransaction bhTransaction)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id";
            return _dataProvider.ExecuteEntry(updateCommand, entry, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById<Columns>(T entry) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id";
            return _dataProvider.ExecuteEntry(updateCommand, entry);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById<Columns>(T entry, BHTransaction bhTransaction) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id";
            return _dataProvider.ExecuteEntry(updateCommand, entry, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById(List<T> entries)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id";
            return UpdateMany(entries, updateCommand);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById(List<T> entries, BHTransaction bhTransaction)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id";
            return UpdateMany(entries, updateCommand, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById<Columns>(List<T> entries) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id";
            return UpdateMany(entries, updateCommand);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById<Columns>(List<T> entries, BHTransaction bhTransaction) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id";
            return UpdateMany(entries, updateCommand, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);

            string updateCommand = $"update {ThisTable} set {UpdateParams} where {sql.Columns}";

            if (withActivator)
            {
                updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.JustExecute(updateCommand, sql.Parameters);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {sql.Columns}";

            if (withActivator)
            {
                updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.JustExecute(updateCommand, sql.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);

            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}";

            if (withActivator)
            {
                updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.JustExecute(updateCommand, sql.Parameters);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction bhTransaction) where Columns : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);

            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}";

            if (withActivator)
            {
                updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return _dataProvider.JustExecute(updateCommand, sql.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.DeleteAllEntries()
        {
            string SubCommand = $"Delete from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {ThisInactive} = 0";
            }

            return _dataProvider.JustExecute(SubCommand, null);
        }

        bool IBHDataProvider<T, G>.DeleteAllEntries(BHTransaction bhTransaction)
        {
            string SubCommand = $"Delete from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {ThisInactive} = 0";
            }

            return _dataProvider.JustExecute(SubCommand, null, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.DeleteEntryById(G Id)
        {
            string SubCommand = $"delete from {ThisTable} where {ThisId} = @Id";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {ThisId} = @Id";
            }

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            return _dataProvider.JustExecute(SubCommand, parameters.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteEntryById(G Id, BHTransaction bhTransaction)
        {
            string SubCommand = $"delete from {ThisTable} where {ThisId} = @Id";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {ThisId} = @Id";
            }

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            return _dataProvider.JustExecute(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.DeleteInactiveEntryById(G Id)
        {
            string SubCommand = $"delete from {ThisTable} where {ThisId}= @Id and {ThisInactive} = 1";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);
            return _dataProvider.JustExecute(SubCommand, parameters.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteInactiveEntryById(G Id, BHTransaction bhTransaction)
        {
            string SubCommand = $"delete from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 1";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);
            return _dataProvider.JustExecute(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.ReactivateEntryById(G Id, BHTransaction bhTransaction)
        {
            string SubCommand = $"update {ThisTable} set {ThisInactive} = 0 where {ThisId} = @Id and {ThisInactive} = 1";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);
            return _dataProvider.JustExecute(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.ReactivateEntryById(G Id)
        {
            string SubCommand = $"update {ThisTable} set {ThisInactive} = 0 where {ThisId} = @Id and {ThisInactive} = 1";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);
            return _dataProvider.JustExecute(SubCommand, parameters.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"delete from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {sql.Columns}";
            }

            return _dataProvider.JustExecute(SubCommand, sql.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"delete from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive}=1 where {sql.Columns}";
            }

            return _dataProvider.JustExecute(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllEntriesAsync()
        {
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0";
            }

            return await _dataProvider.QueryAsync<T>(SubCommand,null);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllEntriesAsync(BHTransaction bhTransaction)
        {
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0";
            }

            return await _dataProvider.QueryAsync<T>(SubCommand, null ,bhTransaction.transaction);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>() where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));

            string SubCommand = $"select {colsAndParams} from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0";
            }

            return await _dataProvider.QueryAsync<Dto>(SubCommand, null);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>(BHTransaction bhTransaction) where Dto : class
        {
            List<Dto> entries = new List<Dto>();

            string colsAndParams = CompareDtoToEntity(typeof(Dto));

            string SubCommand = $"select {colsAndParams} from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0";
            }

            return await _dataProvider.QueryAsync<Dto>(SubCommand, null, bhTransaction.transaction);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync()
        {
            if (withActivator)
            {
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 1";
                return await _dataProvider.QueryAsync<T>(SubCommand, null);
            }

            return new List<T>();
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync(BHTransaction bhTransaction)
        {
            if (withActivator)
            {
                string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 1";
                return await _dataProvider.QueryAsync<T>(SubCommand, null, bhTransaction.transaction);
            }

            return new List<T>();
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryByIdAsync(G Id)
        {
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0";
            }

            return await _dataProvider.QueryFirstAsync<T>(SubCommand, parameters.Parameters);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0";
            }

            return await _dataProvider.QueryFirstAsync<T>(SubCommand, parameters.Parameters,bhTransaction.transaction);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryByIdAsync<Dto>(G Id) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId} = @Id";

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0";
            }

            return await _dataProvider.QueryFirstAsync<Dto>(SubCommand, parameters.Parameters);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryByIdAsync<Dto>(G Id, BHTransaction bhTransaction) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId} = @Id";

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0";
            }

            return await _dataProvider.QueryFirstAsync<Dto>(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.QueryFirstAsync<T>(SubCommand, sql.Parameters);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.QueryFirstAsync<T>(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.QueryFirstAsync<Dto>(SubCommand, sql.Parameters);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.QueryFirstAsync<Dto>(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.QueryAsync<T>(SubCommand, sql.Parameters);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.QueryAsync<T>(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.QueryAsync<Dto>(SubCommand, sql.Parameters);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            string colsAndParams = CompareDtoToEntity(typeof(Dto));
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);

            string SubCommand = $"select {colsAndParams} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"select {colsAndParams} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.QueryAsync<Dto>(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        async Task<G?> IBHDataProvider<T, G>.InsertEntryAsync(T entry)
        {
            string commandStart = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}";
            string commandEnd = $"values ({PropertyParams}, 0";
            return await _dataProvider.InsertScalarAsync<T, G>(commandStart, commandEnd, entry);
        }

        async Task<G?> IBHDataProvider<T, G>.InsertEntryAsync(T entry, BHTransaction bhTransaction)
        {
            string commandStart = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}";
            string commandEnd = $"values ({PropertyParams}, 0";
            return await _dataProvider.InsertScalarAsync<T, G>(commandStart, commandEnd, entry, bhTransaction.transaction);
        }

        async Task<List<G?>> IBHDataProvider<T, G>.InsertEntriesAsync(List<T> entries)
        {
            string commandStart = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}";
            string commandEnd = $"values ({PropertyParams}, 0";
            List<G?> Ids = new List<G?>();

            using(BlackHoleTransaction bhTransaction = new BlackHoleTransaction())
            {
                Ids = await _dataProvider.MultiInsertScalarAsync<T, G>(commandStart, commandEnd, entries, bhTransaction);
            }

            return Ids;
        }

        async Task<List<G?>> IBHDataProvider<T, G>.InsertEntriesAsync(List<T> entries, BHTransaction bhTransaction)
        {
            string commandStart = $"insert into {ThisTable} ({PropertyNames},{ThisInactive}";
            string commandEnd = $"values ({PropertyParams}, 0";
            return await _dataProvider.MultiInsertScalarAsync<T, G>(commandStart, commandEnd, entries, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync(T entry)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id";
            return await _dataProvider.ExecuteEntryAsync(updateCommand, entry);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync(T entry, BHTransaction bhTransaction)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id";
            return await _dataProvider.ExecuteEntryAsync(updateCommand, entry, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync<Columns>(T entry) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id";
            return await _dataProvider.ExecuteEntryAsync(updateCommand, entry);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync<Columns>(T entry, BHTransaction bhTransaction) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id";
            return await _dataProvider.ExecuteEntryAsync(updateCommand, entry, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync(List<T> entries)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id";
            return await UpdateManyAsync(entries, updateCommand);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync(List<T> entries, BHTransaction bhTransaction)
        {
            string updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id";
            return await UpdateManyAsync(entries, updateCommand, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync<Columns>(List<T> entries) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id";
            return await UpdateManyAsync(entries, updateCommand);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync<Columns>(List<T> entries, BHTransaction bhTransaction) where Columns : class
        {
            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id";
            return await UpdateManyAsync(entries, updateCommand, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);

            string updateCommand = $"update {ThisTable} set {UpdateParams} where {sql.Columns}";

            if (withActivator)
            {
                updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.JustExecuteAsync(updateCommand, sql.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);

            string updateCommand = $"update {ThisTable} set {UpdateParams} where {sql.Columns}";

            if (withActivator)
            {
                updateCommand = $"update {ThisTable} set {UpdateParams} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.JustExecuteAsync(updateCommand, sql.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);

            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}";

            if (withActivator)
            {
                updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.JustExecuteAsync(updateCommand, sql.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction bhTransaction) where Columns : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);

            string updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}";

            if (withActivator)
            {
                updateCommand = $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive} = 0 and {sql.Columns}";
            }

            return await _dataProvider.JustExecuteAsync(updateCommand, sql.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteAllEntriesAsync()
        {
            string SubCommand = $"Delete from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {ThisInactive} = 0";
            }

            return await _dataProvider.JustExecuteAsync(SubCommand, null);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteAllEntriesAsync(BHTransaction bhTransaction)
        {
            string SubCommand = $"Delete from {ThisTable}";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {ThisInactive} = 0";
            }

            return await _dataProvider.JustExecuteAsync(SubCommand, null, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntryByIdAsync(G Id)
        {
            string SubCommand = $"delete from {ThisTable} where {ThisId} = @Id";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {ThisId} = @Id";
            }

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            return await _dataProvider.JustExecuteAsync(SubCommand, parameters.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            string SubCommand = $"delete from {ThisTable} where {ThisId} = @Id";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {ThisId} = @Id";
            }

            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);

            return await _dataProvider.JustExecuteAsync(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteInactiveEntryByIdAsync(G Id)
        {
            string SubCommand = $"delete from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 1";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);
            return await _dataProvider.JustExecuteAsync(SubCommand, parameters.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteInactiveEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            string SubCommand = $"delete from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 1";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);
            return await _dataProvider.JustExecuteAsync(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.ReactivateEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            string SubCommand = $"update {ThisTable} set {ThisInactive} = 0 where {ThisId} = @Id and {ThisInactive} = 1";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);
            return await _dataProvider.JustExecuteAsync(SubCommand, parameters.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.ReactivateEntryByIdAsync(G Id)
        {
            string SubCommand = $"update {ThisTable} set {ThisInactive} = 0 where {ThisId} = @Id and {ThisInactive} = 1";
            BHParameters parameters = new BHParameters();
            parameters.Add("Id", Id);
            return await _dataProvider.JustExecuteAsync(SubCommand, parameters.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"delete from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {sql.Columns}";
            }

            return await _dataProvider.JustExecuteAsync(SubCommand, sql.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string SubCommand = $"delete from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                SubCommand = $"Update {ThisTable} set {ThisInactive} = 1 where {sql.Columns}";
            }

            return await _dataProvider.JustExecuteAsync(SubCommand, sql.Parameters, bhTransaction.transaction);
        }

        G? IBHDataProvider<T, G>.GetIdWhere(Expression<Func<T, bool>> predicate)
        {
            return GetIdFromPredicate(predicate);
        }

        G? IBHDataProvider<T, G>.GetIdWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            return GetIdFromPredicate(predicate, bhTransaction);
        }

        List<G> IBHDataProvider<T, G>.GetIdsWhere(Expression<Func<T, bool>> predicate)
        {
            return GetIdsFromPredicate(predicate);
        }

        List<G> IBHDataProvider<T, G>.GetIdsWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            return GetIdsFromPredicate(predicate, bhTransaction);
        }

        async Task<G?> IBHDataProvider<T, G>.GetIdAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            return await GetIdFromPredicateAsync(predicate);
        }

        async Task<G?> IBHDataProvider<T, G>.GetIdAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            return await GetIdFromPredicateAsync(predicate, bhTransaction);
        }

        async Task<List<G>> IBHDataProvider<T, G>.GetIdsAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            return await GetIdsFromPredicateAsync(predicate);
        }

        async Task<List<G>> IBHDataProvider<T, G>.GetIdsAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            return await GetIdsFromPredicateAsync(predicate, bhTransaction);
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.InnerJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return CreateFirstJoin<TOther, Dto>(key, otherKey, "inner");
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.OuterJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return CreateFirstJoin<TOther, Dto>(key, otherKey, "full outer");
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

            firstJoin.Joins = $" {joinType} join {_dataProviderSelector.GetDatabaseSchema()}{MyShit(typeof(TOther).Name)} {parameterOther} on {parameterOther}.{MyShit(propNameOther)} = {parameter}.{MyShit(propName)}";
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

        private bool UpdateMany(List<T> entries, string updateCommand)
        {
            BlackHoleTransaction bhTransaction = new BlackHoleTransaction();
            
            foreach (T entry in entries)
            {
                _dataProvider.ExecuteEntry(updateCommand, entry, bhTransaction);
            }

            bool result = bhTransaction.Commit();
            bhTransaction.Dispose();

            return result;
        }

        private bool UpdateMany(List<T> entries, string updateCommand, BlackHoleTransaction bhTransaction)
        {
            bool result = true;

            foreach (T entry in entries)
            {
                if(!_dataProvider.ExecuteEntry(updateCommand, entry, bhTransaction))
                {
                    result = false;
                }
            }

            return result;
        }

        private async Task<bool> UpdateManyAsync(List<T> entries, string updateCommand)
        {
            BlackHoleTransaction bhTransaction = new BlackHoleTransaction();

            foreach (T entry in entries)
            {
                await _dataProvider.ExecuteEntryAsync(updateCommand, entry, bhTransaction);
            }

            bool result = bhTransaction.Commit();
            bhTransaction.Dispose();

            return result;
        }

        private async Task<bool> UpdateManyAsync(List<T> entries, string updateCommand, BlackHoleTransaction bhTransaction)
        {
            bool result = true;

            foreach (T entry in entries)
            {
                if (!await _dataProvider.ExecuteEntryAsync(updateCommand, entry, bhTransaction))
                {
                    result = false;
                }
            }

            return result;
        }

        private G? GetIdFromPredicate(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
            }

            return _dataProvider.QueryFirst<G>(selectCommand, sql.Parameters);
        }

        private G? GetIdFromPredicate(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
            }

            return _dataProvider.QueryFirst<G>(selectCommand, sql.Parameters, bhTransaction.transaction);
        }

        private async Task<G?> GetIdFromPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
            }

            return await _dataProvider.QueryFirstAsync<G>(selectCommand, sql.Parameters);
        }

        private async Task<G?> GetIdFromPredicateAsync(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
            }

            return await _dataProvider.QueryFirstAsync<G>(selectCommand, sql.Parameters, bhTransaction.transaction);
        }

        private List<G> GetIdsFromPredicate(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
            }

            return _dataProvider.Query<G>(selectCommand, sql.Parameters);
        }

        private List<G> GetIdsFromPredicate(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
            }

            return _dataProvider.Query<G>(selectCommand, sql.Parameters, bhTransaction.transaction);
        }

        private async Task<List<G>> GetIdsFromPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
            }

            return await _dataProvider.QueryAsync<G>(selectCommand, sql.Parameters);
        }

        private async Task<List<G>> GetIdsFromPredicateAsync(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(isMyShit, string.Empty, null, 0);
            string selectCommand = $"select {ThisId} from {ThisTable} where {sql.Columns}";

            if (withActivator)
            {
                selectCommand = $"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}";
            }

            return await _dataProvider.QueryAsync<G>(selectCommand, sql.Parameters, bhTransaction.transaction);
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

