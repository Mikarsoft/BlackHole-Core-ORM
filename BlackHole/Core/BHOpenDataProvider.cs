using BlackHole.CoreSupport;
using BlackHole.Entities;
using BlackHole.Identifiers;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BHOpenDataProvider<T> : IBHOpenDataProvider<T> where T : BHOpenEntity<T>
    {
        private readonly PKSettings<T> _settings;
        private string ThisTable { get; }
        private List<string> Columns { get; } = new();
        private string PropertyNames { get; }
        private string PropertyParams { get; }
        private string UpdateParams { get; }
        private string ThisSchema { get; }
        private bool IsMyShit { get; }
        private readonly IExecutionProvider _executionProvider;

        /// <summary>
        /// 
        /// </summary>
        public BHOpenDataProvider()
        {
            if(Activator.CreateInstance(typeof(T)) is T entity)
            {
                _settings = entity.PrimaryKeyOptions(new PKOptionsBuilder<T>());
            }
            else
            {
                _settings = new(true);
            }

            Type EntityType = typeof(T);

            _executionProvider = BHDataProviderSelector.GetExecutionProvider();
            IsMyShit = _executionProvider.SkipQuotes();
            ThisSchema = BHDataProviderSelector.GetDatabaseSchema();
            ThisTable = $"{ThisSchema}{MyShit(EntityType.Name)}";

            using (TripleStringBuilder sb = new())
            {
                foreach (PropertyInfo prop in EntityType.GetProperties())
                {
                    string property = MyShit(prop.Name);
                    sb.PNSb.Append($", {property}");
                    sb.PPSb.Append($", @{prop.Name}");
                    sb.UPSb.Append($",{property} = @{prop.Name}");
                        
                    Columns.Add(prop.Name);                   
                }
                PropertyNames = $"{sb.PNSb.ToString().Remove(0, 1)} ";
                PropertyParams = $"{sb.PPSb.ToString().Remove(0, 1)} ";
                UpdateParams = $"{sb.UPSb.ToString().Remove(0, 1)} ";
            }
        }

        bool IBHOpenDataProvider<T>.DeleteAllEntries()
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.DeleteAllEntries(BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IBHOpenDataProvider<T>.DeleteAllEntriesAsync()
        {
            throw new NotImplementedException();
        }

        async Task<bool> IBHOpenDataProvider<T>.DeleteAllEntriesAsync(BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IBHOpenDataProvider<T>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IBHOpenDataProvider<T>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        List<T> IBHOpenDataProvider<T>.GetAllEntries()
        {
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable}", null);
        }

        List<T> IBHOpenDataProvider<T>.GetAllEntries(BHTransaction bhTransaction)
        {
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable}", null , bhTransaction.transaction);
        }

        List<Dto> IBHOpenDataProvider<T>.GetAllEntries<Dto>()
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            return _executionProvider.Query<Dto>($"select {commonColumns} from {ThisTable}", null);
        }

        List<Dto> IBHOpenDataProvider<T>.GetAllEntries<Dto>(BHTransaction bhTransaction)
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            return _executionProvider.Query<Dto>($"select {commonColumns} from {ThisTable}", null, bhTransaction.transaction);
        }

        async Task<List<T>> IBHOpenDataProvider<T>.GetAllEntriesAsync()
        {
            return await _executionProvider.QueryAsync<T>($"select {PropertyNames} from {ThisTable}", null);
        }

        async Task<List<T>> IBHOpenDataProvider<T>.GetAllEntriesAsync(BHTransaction bhTransaction)
        {
            return await _executionProvider.QueryAsync<T>($"select {PropertyNames} from {ThisTable}", null, bhTransaction.transaction);
        }

        async Task<List<Dto>> IBHOpenDataProvider<T>.GetAllEntriesAsync<Dto>()
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            return await _executionProvider.QueryAsync<Dto>($"select {commonColumns} from {ThisTable}", null);
        }

        async Task<List<Dto>> IBHOpenDataProvider<T>.GetAllEntriesAsync<Dto>(BHTransaction bhTransaction)
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            return await _executionProvider.QueryAsync<Dto>($"select {commonColumns} from {ThisTable}", null, bhTransaction.transaction);
        }

        async Task<List<T>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<List<T>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<Dto>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate)
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<List<Dto>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        List<T> IBHOpenDataProvider<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        List<T> IBHOpenDataProvider<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        List<Dto> IBHOpenDataProvider<T>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate)
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        List<Dto> IBHOpenDataProvider<T>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<T?> IBHOpenDataProvider<T>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<T?> IBHOpenDataProvider<T>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<Dto?> IBHOpenDataProvider<T>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<Dto?> IBHOpenDataProvider<T>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        T? IBHOpenDataProvider<T>.GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        T? IBHOpenDataProvider<T>.GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        Dto? IBHOpenDataProvider<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        Dto? IBHOpenDataProvider<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters,bhTransaction.transaction);
        }

        bool IBHOpenDataProvider<T>.InsertEntries(List<T> entries)
        {
            return InsertMany(entries, $"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})");
        }

        bool IBHOpenDataProvider<T>.InsertEntries(List<T> entries, BHTransaction bhTransaction)
        {
            return InsertMany(entries, $"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", bhTransaction.transaction);
        }

        async Task<bool> IBHOpenDataProvider<T>.InsertEntriesAsync(List<T> entries)
        {
            return await InsertManyAsync(entries, $"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})");
        }

        async Task<bool> IBHOpenDataProvider<T>.InsertEntriesAsync(List<T> entries, BHTransaction bhTransaction)
        {
            return await InsertManyAsync(entries, $"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", bhTransaction.transaction);
        }

        bool IBHOpenDataProvider<T>.InsertEntry(T entry)
        {
            return _executionProvider.JustExecute($"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", MapObjectToParameters(CheckGenerateValue(entry)));
        }

        bool IBHOpenDataProvider<T>.InsertEntry(T entry, BHTransaction bhTransaction)
        {
            return _executionProvider.JustExecute($"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", MapObjectToParameters(CheckGenerateValue(entry)), bhTransaction.transaction);
        }

        async Task<bool> IBHOpenDataProvider<T>.InsertEntryAsync(T entry)
        {
            return await _executionProvider.JustExecuteAsync($"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", MapObjectToParameters(CheckGenerateValue(entry)));
        }

        async Task<bool> IBHOpenDataProvider<T>.InsertEntryAsync(T entry, BHTransaction bhTransaction)
        {
            return await _executionProvider.JustExecuteAsync($"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", MapObjectToParameters(CheckGenerateValue(entry)), bhTransaction.transaction);
        }

        JoinsData<Dto, T, TOther> IBHOpenDataProvider<T>.InnerJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return Columns.CreateFirstJoin<T, TOther, Dto>(key, otherKey, "inner", ThisSchema, IsMyShit, true);
        }

        JoinsData<Dto, T, TOther> IBHOpenDataProvider<T>.LeftJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return Columns.CreateFirstJoin<T, TOther, Dto>(key, otherKey, "left", ThisSchema, IsMyShit, true);
        }

        JoinsData<Dto, T, TOther> IBHOpenDataProvider<T>.OuterJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return Columns.CreateFirstJoin<T, TOther, Dto>(key, otherKey, "full outer", ThisSchema, IsMyShit, true);
        }

        JoinsData<Dto, T, TOther> IBHOpenDataProvider<T>.RightJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
        {
            return Columns.CreateFirstJoin<T, TOther, Dto>(key, otherKey, "right", ThisSchema, IsMyShit, true);
        }

        async Task<bool> IBHOpenDataProvider<T>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IBHOpenDataProvider<T>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IBHOpenDataProvider<T>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            throw new NotImplementedException();
        }

        async Task<bool> IBHOpenDataProvider<T>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction transaction) where Columns : class
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction transaction) where Columns : class
        {
            throw new NotImplementedException();
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

        private bool InsertMany<Dto>(List<Dto> entries, string textCommand)
        {
            BlackHoleTransaction bhTransaction = new();

            foreach (Dto entry in entries)
            {
                _executionProvider.JustExecute(textCommand, MapObjectToParameters(CheckGenerateValue(entry)), bhTransaction);
            }

            bool result = bhTransaction.Commit();
            bhTransaction.Dispose();

            return result;
        }

        private bool InsertMany<Dto>(List<Dto> entries, string textCommand, BlackHoleTransaction bhTransaction)
        {
            bool result = true;

            foreach (Dto entry in entries)
            {
                if (!_executionProvider.JustExecute(textCommand, MapObjectToParameters(CheckGenerateValue(entry)), bhTransaction))
                {
                    result = false;
                }
            }

            return result;
        }

        private async Task<bool> InsertManyAsync<Dto>(List<Dto> entries, string textCommand)
        {
            BlackHoleTransaction bhTransaction = new();

            foreach (Dto entry in entries)
            {
                await _executionProvider.JustExecuteAsync(textCommand, MapObjectToParameters(CheckGenerateValue(entry)), bhTransaction);
            }

            bool result = bhTransaction.Commit();
            bhTransaction.Dispose();

            return result;
        }

        private async Task<bool> InsertManyAsync<Dto>(List<Dto> entries, string textCommand, BlackHoleTransaction bhTransaction)
        {
            bool result = true;

            foreach (Dto entry in entries)
            {
                if (!await _executionProvider.JustExecuteAsync(textCommand, MapObjectToParameters(CheckGenerateValue(entry)), bhTransaction))
                {
                    result = false;
                }
            }
            return result;
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
            return PNsb.ToString().Remove(0, 1);
        }

        private List<BlackHoleParameter> MapObjectToParameters<Dto>(Dto parametersObject)
        {
            if (parametersObject == null)
            {
                return new();
            }

            PropertyInfo[] propertyInfos = parametersObject.GetType().GetProperties();

            BHParameters parameters = new();

            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(parametersObject);
                parameters.Add(property.Name, value);
            }

            return parameters.Parameters;
        }

        private Dto CheckGenerateValue<Dto>(Dto entry)
        {
            foreach(AutoGeneratedProperty settings in _settings.AutoGeneratedColumns)
            {
                if (settings.Autogenerated && settings.Generator != null)
                {
                    entry?.GetType().GetProperty(settings.PropertyName)?.SetValue(entry, GetGeneratorsValue(settings.Generator));
                }
            }
            return entry;
        }

        private object? GetGeneratorsValue(object Generator)
        {
            object? result = null;
            Generator.GetType().GetMethod("GenerateValue")?.Invoke(result, null);
            return result;
        }
    }
}
