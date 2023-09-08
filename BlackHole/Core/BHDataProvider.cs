using BlackHole.Entities;
using System.Reflection;
using System.Linq.Expressions;
using BlackHole.CoreSupport;
using System.Text;

namespace BlackHole.Core
{
    /// <summary>
    /// Makes all the communication between the Datbase Table and The Specified Entity
    /// </summary>
    /// <typeparam name="T">Black Hole Entity with Integer Id</typeparam>
    /// <typeparam name="G">The type of Entity's Id</typeparam>
    public class BHDataProvider<T, G> : IBHDataProvider<T, G> where T : BlackHoleEntity<G>
    {
        private bool WithActivator { get; }
        private string ThisTable { get; }
        private List<string> Columns { get; } = new();
        private string PropertyNames { get; }
        private string PropertyParams { get; }
        private string UpdateParams { get; }
        private string ThisId { get; }
        private string ThisInactive { get; }
        private string ThisSchema { get; }
        private bool IsMyShit { get; }

        private readonly IDataProvider _dataProvider;

        /// <summary>
        /// Create a Data Provider that Automatically Communicates with the Database Using the Black Hole Entity, that you pass in.
        /// </summary>
        public BHDataProvider()
        {
            Type EntityType = typeof(T);

            WithActivator = EntityType.CheckActivator();
            _dataProvider = typeof(G).GetDataProvider(EntityType.Name);
            IsMyShit = _dataProvider.SkipQuotes();
            ThisSchema = BHDataProviderSelector.GetDatabaseSchema();
            ThisTable = $"{ThisSchema}{MyShit(EntityType.Name)}";
            ThisId = MyShit("Id");
            ThisInactive = MyShit("Inactive");

            using(TripleStringBuilder sb = new())
            {
                foreach (PropertyInfo prop in EntityType.GetProperties())
                {
                    if (prop.Name != "Inactive")
                    {
                        if (prop.Name != "Id")
                        {
                            string property = MyShit(prop.Name);
                            sb.PNSb.Append($", {property}");
                            sb.PPSb.Append($", @{prop.Name}");
                            sb.UPSb.Append($",{property} = @{prop.Name}");
                        }
                        Columns.Add(prop.Name);
                    }
                }
                PropertyNames = $"{sb.PNSb.ToString().Remove(0, 1)} ";
                PropertyParams = $"{sb.PPSb.ToString().Remove(0, 1)} ";
                UpdateParams = $"{sb.UPSb.ToString().Remove(0, 1)} ";
            }
        }

        List<T> IBHDataProvider<T, G>.GetAllEntries()
        {
            if (WithActivator)
            {
                return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0", null);
            }
            return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable}", null);
        }

        List<T> IBHDataProvider<T, G>.GetAllEntries(BHTransaction bhTransaction)
        {
            if (WithActivator)
            {
                return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0", null, bhTransaction.transaction);
            }
            return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable}", null, bhTransaction.transaction);
        }

        List<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>() where Dto : class
        {
            if (WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0", null);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable}", null);
        }

        List<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>(BHTransaction bhTransaction) where Dto : class
        {
            if (WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0", null, bhTransaction.transaction);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable}", null, bhTransaction.transaction);
        }

        List<T> IBHDataProvider<T, G>.GetAllInactiveEntries()
        {
            if (WithActivator)
            {
                return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 1", null);
            }
            return new List<T>();
        }

        List<T> IBHDataProvider<T, G>.GetAllInactiveEntries(BHTransaction bhTransaction)
        {
            if (WithActivator)
            {
                return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 1", null, bhTransaction.transaction);
            }
            return new List<T>();
        }

        T? IBHDataProvider<T, G>.GetEntryById(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0", Params.Parameters);
            }
            return _dataProvider.QueryFirst<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id", Params.Parameters);
        }

        T? IBHDataProvider<T, G>.GetEntryById(G Id, BHTransaction bhTransaction)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0", Params.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.QueryFirst<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id", Params.Parameters, bhTransaction.transaction);
        }

        Dto? IBHDataProvider<T, G>.GetEntryById<Dto>(G Id) where Dto : class
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0", Params.Parameters);
            }
            return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisId} = @Id", Params.Parameters);
        }

        Dto? IBHDataProvider<T, G>.GetEntryById<Dto>(G Id, BHTransaction bhTransaction) where Dto : class
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0", Params.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisId} = @Id", Params.Parameters, bhTransaction.transaction);
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null , 0);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.QueryFirst<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        List<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        List<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.Query<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        List<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        List<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        G? IBHDataProvider<T, G>.InsertEntry(T entry)
        {
            return _dataProvider.InsertScalar<T, G>($"insert into {ThisTable} ({PropertyNames},{ThisInactive}", $"values ({PropertyParams}, 0", entry);
        }

        G? IBHDataProvider<T, G>.InsertEntry(T entry, BHTransaction bhTransaction)
        {
            return _dataProvider.InsertScalar<T, G>($"insert into {ThisTable} ({PropertyNames}, {ThisInactive}", $"values ({PropertyParams}, 0", entry, bhTransaction.transaction);
        }

        List<G?> IBHDataProvider<T, G>.InsertEntries(List<T> entries)
        {
            List<G?> Ids = new();
            using(BlackHoleTransaction bhTransaction = new())
            {
                Ids = _dataProvider.MultiInsertScalar<T, G>($"insert into {ThisTable} ({PropertyNames},{ThisInactive}", $"values ({PropertyParams}, 0", entries, bhTransaction);
            }
            return Ids;
        }

        List<G?> IBHDataProvider<T, G>.InsertEntries(List<T> entries, BHTransaction bhTransaction)
        {
            return _dataProvider.MultiInsertScalar<T, G>($"insert into {ThisTable} ({PropertyNames},{ThisInactive}", $"values ({PropertyParams}, 0", entries, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById(T entry)
        {
            return _dataProvider.ExecuteEntry($"update {ThisTable} set {UpdateParams} where {ThisId} = @Id", entry);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById(T entry, BHTransaction bhTransaction)
        {
            return _dataProvider.ExecuteEntry($"update {ThisTable} set {UpdateParams} where {ThisId} = @Id", entry, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById<Columns>(T entry) where Columns : class
        {
            return _dataProvider.ExecuteEntry($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id", entry);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById<Columns>(T entry, BHTransaction bhTransaction) where Columns : class
        {
            return _dataProvider.ExecuteEntry($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id", entry, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById(List<T> entries)
        {
            return UpdateMany(entries, $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id");
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById(List<T> entries, BHTransaction bhTransaction)
        {
            return UpdateMany(entries, $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id", bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById<Columns>(List<T> entries) where Columns : class
        {
            return UpdateMany(entries, $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id");
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById<Columns>(List<T> entries, BHTransaction bhTransaction) where Columns : class
        {
            return UpdateMany(entries, $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId}=@Id", bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {UpdateParams} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.JustExecute($"update {ThisTable} set {UpdateParams} where {sql.Columns}", sql.Parameters);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {UpdateParams} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"update {ThisTable} set {UpdateParams} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.JustExecute($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}", sql.Parameters);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction bhTransaction) where Columns : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.DeleteAllEntries()
        {
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {ThisInactive} = 1 where {ThisInactive} = 0", null);
            }
            return _dataProvider.JustExecute($"delete from {ThisTable}", null);
        }

        bool IBHDataProvider<T, G>.DeleteAllEntries(BHTransaction bhTransaction)
        {
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {ThisInactive} = 1 where {ThisInactive} = 0", null, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"delete from {ThisTable}", null, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.DeleteEntryById(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"Update {ThisTable} set {ThisInactive} = 1 where {ThisId} = @Id", Params.Parameters);
            }
            return _dataProvider.JustExecute($"delete from {ThisTable} where {ThisId} = @Id", Params.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteEntryById(G Id, BHTransaction bhTransaction)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {ThisInactive} = 1 where {ThisId} = @Id", Params.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"delete from {ThisTable} where {ThisId} = @Id", Params.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.DeleteInactiveEntryById(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"delete from {ThisTable} where {ThisId}= @Id and {ThisInactive} = 1", Params.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteInactiveEntryById(G Id, BHTransaction bhTransaction)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"delete from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 1", Params.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.ReactivateEntryById(G Id, BHTransaction bhTransaction)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"update {ThisTable} set {ThisInactive} = 0 where {ThisId} = @Id and {ThisInactive} = 1", Params.Parameters, bhTransaction.transaction);
        }

        bool IBHDataProvider<T, G>.ReactivateEntryById(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"update {ThisTable} set {ThisInactive} = 0 where {ThisId} = @Id and {ThisInactive} = 1", Params.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {ThisInactive} = 1 where {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.JustExecute($"delete from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.JustExecute($"update {ThisTable} set {ThisInactive}=1 where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.JustExecute($"delete from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllEntriesAsync()
        {
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0", null);
            }
            return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable}", null);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllEntriesAsync(BHTransaction bhTransaction)
        {
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0", null, bhTransaction.transaction);
            }
            return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable}", null ,bhTransaction.transaction);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>() where Dto : class
        {
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0", null);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable}", null);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>(BHTransaction bhTransaction) where Dto : class
        {
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0", null, bhTransaction.transaction);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable}", null, bhTransaction.transaction);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync()
        {
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 1", null);
            }
            return new List<T>();
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync(BHTransaction bhTransaction)
        {
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 1", null, bhTransaction.transaction);
            }
            return new List<T>();
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryByIdAsync(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0", Params.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id", Params.Parameters);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0", Params.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisId} = @Id", Params.Parameters,bhTransaction.transaction);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryByIdAsync<Dto>(G Id) where Dto : class
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0", Params.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisId} = @Id", Params.Parameters);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryByIdAsync<Dto>(G Id, BHTransaction bhTransaction) where Dto : class
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 0", Params.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisId} = @Id", Params.Parameters, bhTransaction.transaction);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.QueryAsync<T>($"select {ThisId},{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<G?> IBHDataProvider<T, G>.InsertEntryAsync(T entry)
        {
            return await _dataProvider.InsertScalarAsync<T, G>($"insert into {ThisTable} ({PropertyNames},{ThisInactive}", $"values ({PropertyParams}, 0", entry);
        }

        async Task<G?> IBHDataProvider<T, G>.InsertEntryAsync(T entry, BHTransaction bhTransaction)
        {
            return await _dataProvider.InsertScalarAsync<T, G>($"insert into {ThisTable} ({PropertyNames},{ThisInactive}", $"values ({PropertyParams}, 0", entry, bhTransaction.transaction);
        }

        async Task<List<G?>> IBHDataProvider<T, G>.InsertEntriesAsync(List<T> entries)
        {
            List<G?> Ids = new();
            using(BlackHoleTransaction bhTransaction = new())
            {
                Ids = await _dataProvider.MultiInsertScalarAsync<T, G>($"insert into {ThisTable} ({PropertyNames},{ThisInactive}", $"values ({PropertyParams}, 0", entries, bhTransaction);
            }
            return Ids;
        }

        async Task<List<G?>> IBHDataProvider<T, G>.InsertEntriesAsync(List<T> entries, BHTransaction bhTransaction)
        {
            return await _dataProvider.MultiInsertScalarAsync<T, G>($"insert into {ThisTable} ({PropertyNames},{ThisInactive}", $"values ({PropertyParams}, 0", entries, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync(T entry)
        {
            return await _dataProvider.ExecuteEntryAsync($"update {ThisTable} set {UpdateParams} where {ThisId} = @Id", entry);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync(T entry, BHTransaction bhTransaction)
        {
            return await _dataProvider.ExecuteEntryAsync($"update {ThisTable} set {UpdateParams} where {ThisId} = @Id", entry, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync<Columns>(T entry) where Columns : class
        {
            return await _dataProvider.ExecuteEntryAsync($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id", entry);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync<Columns>(T entry, BHTransaction bhTransaction) where Columns : class
        {
            return await _dataProvider.ExecuteEntryAsync($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id", entry, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync(List<T> entries)
        {
            return await UpdateManyAsync(entries, $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id");
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync(List<T> entries, BHTransaction bhTransaction)
        {
            return await UpdateManyAsync(entries, $"update {ThisTable} set {UpdateParams} where {ThisId} = @Id", bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync<Columns>(List<T> entries) where Columns : class
        {
            return await UpdateManyAsync(entries, $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id");
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync<Columns>(List<T> entries, BHTransaction bhTransaction) where Columns : class
        {
            return await UpdateManyAsync(entries, $"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisId} = @Id", bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {UpdateParams} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {UpdateParams} where {sql.Columns}", sql.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {UpdateParams} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {UpdateParams} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}", sql.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction bhTransaction) where Columns : class
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {ThisInactive} = 0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteAllEntriesAsync()
        {
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {ThisInactive} = 1 where {ThisInactive} = 0", null);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {ThisTable}", null);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteAllEntriesAsync(BHTransaction bhTransaction)
        {
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {ThisInactive} = 1 where {ThisInactive} = 0", null, bhTransaction.transaction);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {ThisTable}", null, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntryByIdAsync(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {ThisInactive} = 1 where {ThisId} = @Id", Params.Parameters);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {ThisTable} where {ThisId} = @Id", Params.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {ThisInactive} = 1 where {ThisId} = @Id", Params.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {ThisTable} where {ThisId} = @Id", Params.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteInactiveEntryByIdAsync(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return await _dataProvider.JustExecuteAsync($"delete from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 1", Params.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteInactiveEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return await _dataProvider.JustExecuteAsync($"delete from {ThisTable} where {ThisId} = @Id and {ThisInactive} = 1", Params.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.ReactivateEntryByIdAsync(G Id, BHTransaction bhTransaction)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {ThisInactive} = 0 where {ThisId} = @Id and {ThisInactive} = 1", Params.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.ReactivateEntryByIdAsync(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {ThisInactive} = 0 where {ThisId} = @Id and {ThisInactive} = 1", Params.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {ThisInactive} = 1 where {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {ThisTable} set {ThisInactive} = 1 where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
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
            return Columns.CreateFirstJoin<T,TOther, Dto>(key, otherKey, "inner",ThisSchema,IsMyShit, false);
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.OuterJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return Columns.CreateFirstJoin<T, TOther, Dto>(key, otherKey, "full outer", ThisSchema, IsMyShit, false);
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.LeftJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return Columns.CreateFirstJoin<T, TOther, Dto>(key, otherKey, "left", ThisSchema, IsMyShit, false);
        }

        JoinsData<Dto, T, TOther> IBHDataProvider<T, G>.RightJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return Columns.CreateFirstJoin<T, TOther, Dto>(key, otherKey, "right", ThisSchema, IsMyShit, false);
        }

        private bool UpdateMany(List<T> entries, string updateCommand)
        {
            BlackHoleTransaction bhTransaction = new();
            
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
            BlackHoleTransaction bhTransaction = new();

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
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<G>($"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<G>($"select {ThisId} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        private G? GetIdFromPredicate(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.QueryFirst<G>($"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.QueryFirst<G>($"select {ThisId} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        private async Task<G?> GetIdFromPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<G>($"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<G>($"select {ThisId} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        private async Task<G?> GetIdFromPredicateAsync(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<G>($"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.QueryFirstAsync<G>($"select {ThisId} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        private List<G> GetIdsFromPredicate(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.Query<G>($"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.Query<G>($"select {ThisId} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        private List<G> GetIdsFromPredicate(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return _dataProvider.Query<G>($"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return _dataProvider.Query<G>($"select {ThisId} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        private async Task<List<G>> GetIdsFromPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<G>($"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryAsync<G>($"select {ThisId} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        private async Task<List<G>> GetIdsFromPredicateAsync(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            if (WithActivator)
            {
                return await _dataProvider.QueryAsync<G>($"select {ThisId} from {ThisTable} where {ThisInactive}=0 and {sql.Columns}", sql.Parameters, bhTransaction.transaction);
            }
            return await _dataProvider.QueryAsync<G>($"select {ThisId} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        private string MyShit(string? propName)
        {
            if (!IsMyShit)
            {
                return $@"""{propName}""";
            }
            return propName ?? string.Empty;
        }

        private string CompareDtoToEntity(Type dto)
        {
            StringBuilder PNsb = new();
            foreach (PropertyInfo property in dto.GetProperties())
            {
                if (Columns.Contains(property.Name)
                    && typeof(T).GetProperty(property.Name)?.PropertyType == property.PropertyType)
                {
                    PNsb.Append($",{MyShit(property.Name)}");
                }
            }
            PNsb.Append(' ');
            return PNsb.ToString().Remove(0, 1);
        }

        private string CompareColumnsToEntity(Type dto)
        {
            StringBuilder PNsb = new();
            foreach (PropertyInfo property in dto.GetProperties())
            {
                if (property.Name != "Id" && Columns.Contains(property.Name)
                    && typeof(T).GetProperty(property.Name)?.PropertyType == property.PropertyType)
                {
                    PNsb.Append($",{MyShit(property.Name)}=@{property.Name}");
                }
            }
            PNsb.Append(' ');
            return PNsb.ToString().Remove(0,1);
        }
    }
}

