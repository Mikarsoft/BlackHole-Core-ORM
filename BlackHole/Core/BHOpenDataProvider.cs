using BlackHole.Engine;
using BlackHole.Entities;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BlackHole.Core
{
    /// <summary>
    /// Makes all the communication between the Database Table and The Specified Entity.
    /// <para>For custom commands, use IBHConnection Interface</para>
    /// </summary>
    /// <typeparam name="T">BHOpenEntity</typeparam>
    public class BHOpenDataProvider<T> : IBHOpenDataProvider<T> where T : BHOpenEntity<T>
    {
        #region Ctor

        private readonly EntitySettings<T> _settings;
        private string ThisTable { get; }
        private List<string> Columns { get; } = new();
        private string PropertyNames { get; }
        private string PropertyParams { get; }
        private string UpdateParams { get; }
        private string ThisSchema { get; }
        private string MainPK { get; set; } = string.Empty;
        private bool IsMyShit { get; }
        private PropertyInfo[] Tprops { get; set; }
        private string[] ReturningCase { get; set; } = new string[2];
        private readonly IDataProvider _executionProvider;

        /// <summary>
        /// Create a Data Provider that Automatically Communicates with the Database Using the BHOpenEntity, that you pass in.
        /// </summary>
        public BHOpenDataProvider()
        {
            if (Activator.CreateInstance(typeof(T)) is T entity)
            {
                _settings = entity.EntityOptions(new EntityOptionsBuilder<T>());
            }
            else
            {
                _settings = new(true);
            }

            _settings.MapEntitySettings();
            _executionProvider = _settings.ConnectionIndex.GetDataProvider();

            Type EntityType = typeof(T);

            IsMyShit = _executionProvider.SkipQuotes();
            ThisSchema = BlackHoleEngine.GetDatabaseSchema();
            ThisTable = $"{ThisSchema}{MyShit(EntityType.Name)}";

            if (_settings.HasAutoIncrement)
            {
                MainPK = $"{MyShit(_settings.MainPrimaryKey)},";
            }

            Tprops = EntityType.GetProperties();

            using (TripleStringBuilder sb = new())
            {
                foreach (PropertyInfo prop in Tprops)
                {
                    string property = MyShit(prop.Name);

                    if (_settings.HasAutoIncrement)
                    {
                        if (prop.Name != _settings.MainPrimaryKey)
                        {
                            sb.PNSb.Append($", {property}");
                            sb.PPSb.Append($", @{prop.Name}");
                        }
                        else
                        {
                            ReturningCase = _settings.GetReturningPrimaryKey(property, ThisTable);
                        }
                    }
                    else
                    {
                        sb.PNSb.Append($", {property}");
                        sb.PPSb.Append($", @{prop.Name}");
                    }

                    if (!_settings.PKPropertyNames.Contains(prop.Name))
                    {
                        sb.UPSb.Append($",{property} = @{prop.Name}");
                    }

                    Columns.Add(prop.Name);
                }
                PropertyNames = $"{sb.PNSb.ToString().Remove(0, 1)} ";
                PropertyParams = $"{sb.PPSb.ToString().Remove(0, 1)} ";
                UpdateParams = $"{sb.UPSb.ToString().Remove(0, 1)} ";
            }
        }
        #endregion

        #region Common Helper Methods
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
                if (!_settings.PKPropertyNames.Contains(property.Name) && Columns.Contains(property.Name)
                    && typeof(T).GetProperty(property.Name)?.PropertyType == property.PropertyType)
                {
                    PNsb.Append($",{MyShit(property.Name)}=@{property.Name}");
                }
            }
            PNsb.Append(' ');
            return PNsb.ToString().Remove(0, 1);
        }

        private List<BlackHoleParameter> MapObjectToParameters<Dto>(Dto parametersObject, PropertyInfo[] propertyInfos)
        {
            if (parametersObject == null) { return new(); }
            BHParameters parameters = new();
            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(parametersObject);
                parameters.Add(property.Name, value);
            }
            return parameters.Parameters;
        }

        private void CheckGenerateValue<Dto>(Dto entry)
        {
            foreach (AutoGeneratedProperty settings in _settings.AutoGeneratedColumns)
            {
                if (settings.Autogenerated && settings.Generator != null)
                {
                    typeof(Dto).GetProperty(settings.PropertyName)?.SetValue(entry, GetGeneratorsValue(settings.Generator));
                }
            }
        }

        private object? GetGeneratorsValue(object Generator)
        {
            return Generator.GetType().GetMethod("GenerateValue")?.Invoke(Generator, null);
        }
        #endregion

        // SYNC METHODS

        #region Helper Methods
        private bool InsertMany<Dto>(List<Dto> entries, string textCommand)
        {
            BlackHoleTransaction bhTransaction = new();
            bool result = true;
            foreach (Dto entry in entries)
            {
                CheckGenerateValue(entry);
                if (_settings.HasAutoIncrement)
                {
                    object? IdEntry = _executionProvider.ExecuteRawScalar($"insert into {ThisTable} ({PropertyNames}) {ReturningCase[0]} values({PropertyParams}) {ReturningCase[1]}", MapObjectToParameters(entry, Tprops), bhTransaction);
                    if (IdEntry != null) { typeof(Dto).GetProperty(_settings.MainPrimaryKey)?.SetValue(entry, IdEntry); }
                }
                else
                {
                    _executionProvider.JustExecute(textCommand, MapObjectToParameters(entry, Tprops), bhTransaction);
                }
            }
            if (!bhTransaction.Commit())
            {
                result = false;
            }
            bhTransaction.Dispose();
            return result;
        }

        // WITH TRANSACTION

        private bool InsertMany<Dto>(List<Dto> entries, string textCommand, BlackHoleTransaction bhTransaction)
        {
            bool result = true;
            foreach (Dto entry in entries)
            {
                CheckGenerateValue(entry);
                if (_settings.HasAutoIncrement)
                {
                    object? IdEntry = _executionProvider.ExecuteRawScalar($"insert into {ThisTable} ({PropertyNames}) {ReturningCase[0]} values({PropertyParams}) {ReturningCase[1]}", MapObjectToParameters(entry, Tprops), bhTransaction);
                    if (IdEntry != null)
                    {
                        typeof(Dto).GetProperty(_settings.MainPrimaryKey)?.SetValue(entry, IdEntry);
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    if (!_executionProvider.JustExecute(textCommand, MapObjectToParameters(entry, Tprops), bhTransaction))
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        #endregion

        #region Additional Methods

        bool IBHOpenDataProvider<T>.Any()
        {
            string[] oneRow = 1.GetLimiter();
            return _executionProvider.QueryFirst<T>($"select {oneRow[0]}{PropertyNames} from {ThisTable} where 1=1 {oneRow[1]}", null) != null;
        }

        bool IBHOpenDataProvider<T>.Any(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            string[] oneRow = 1.GetLimiter();
            return _executionProvider.QueryFirst<T>($"select {oneRow[0]}{PropertyNames} from {ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters) != null;
        }

        int IBHOpenDataProvider<T>.Count()
        {
            return _executionProvider.QueryFirst<int>($"select count(*) from {ThisTable}", null);
        }

        int IBHOpenDataProvider<T>.CountWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<int>($"select count(*) from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        bool IBHOpenDataProvider<T>.Any(BHTransaction bhTransaction)
        {
            string[] oneRow = 1.GetLimiter();
            return _executionProvider.QueryFirst<T>($"select {oneRow[0]}{PropertyNames} from {ThisTable} where 1=1 {oneRow[1]}", null, bhTransaction.transaction) != null;
        }

        bool IBHOpenDataProvider<T>.Any(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            string[] oneRow = 1.GetLimiter();
            return _executionProvider.QueryFirst<T>($"select {oneRow[0]}{PropertyNames} from {ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters, bhTransaction.transaction) != null;
        }

        int IBHOpenDataProvider<T>.Count(BHTransaction bhTransaction)
        {
            return _executionProvider.QueryFirst<int>($"select count(*) from {ThisTable}", null, bhTransaction.transaction);
        }

        int IBHOpenDataProvider<T>.CountWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<int>($"select count(*) from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        #endregion

        #region Select Methods

        List<T> IBHOpenDataProvider<T>.GetAllEntries()
        {
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable}", null);
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

        List<T> IBHOpenDataProvider<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<T>($"select {MainPK}{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
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

        T? IBHOpenDataProvider<T>.GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            string[] oneRow = 1.GetLimiter();
            return _executionProvider.QueryFirst<T>($"select {oneRow[0]}{MainPK}{PropertyNames} from {ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters);
        }

        Dto? IBHOpenDataProvider<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            string[] oneRow = 1.GetLimiter();
            return _executionProvider.QueryFirst<Dto>($"select {oneRow[0]}{commonColumns} from {ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters);
        }

        // WITH TRANSACTION

        List<T> IBHOpenDataProvider<T>.GetAllEntries(BHTransaction bhTransaction)
        {
            return _executionProvider.Query<T>($"select {MainPK}{PropertyNames} from {ThisTable}", null, bhTransaction.transaction);
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

        List<T> IBHOpenDataProvider<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<T>($"select {MainPK}{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
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

        T? IBHOpenDataProvider<T>.GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            string[] oneRow = 1.GetLimiter();
            return _executionProvider.QueryFirst<T>($"select {oneRow[0]}{MainPK}{PropertyNames} from {ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters, bhTransaction.transaction);
        }

        Dto? IBHOpenDataProvider<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        // WITH ORDER BY

        List<T> IBHOpenDataProvider<T>.GetAllEntries(Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable} {orderClass.OrderByToSql(IsMyShit)}", null);
        }

        List<Dto> IBHOpenDataProvider<T>.GetAllEntries<Dto>(Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            return _executionProvider.Query<Dto>($"select {commonColumns} from {ThisTable} {orderClass.OrderByToSql(IsMyShit)}", null);
        }

        T? IBHOpenDataProvider<T>.GetEntryWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters);
        }

        Dto? IBHOpenDataProvider<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters);
        }

        List<T> IBHOpenDataProvider<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters);
        }

        List<Dto> IBHOpenDataProvider<T>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            };
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters);
        }

        // WITH ORDER BY AND TRANSACTION

        List<T> IBHOpenDataProvider<T>.GetAllEntries(Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable} {orderClass.OrderByToSql(IsMyShit)}", null, bhTransaction.transaction);
        }

        List<Dto> IBHOpenDataProvider<T>.GetAllEntries<Dto>(Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            return _executionProvider.Query<Dto>($"select {commonColumns} from {ThisTable} {orderClass.OrderByToSql(IsMyShit)}", null, bhTransaction.transaction);
        }

        T? IBHOpenDataProvider<T>.GetEntryWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters, bhTransaction.transaction);
        }

        Dto? IBHOpenDataProvider<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.QueryFirst<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters, bhTransaction.transaction);
        }

        List<T> IBHOpenDataProvider<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters, bhTransaction.transaction);
        }

        List<Dto> IBHOpenDataProvider<T>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            };
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.Query<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters, bhTransaction.transaction);
        }

        #endregion

        #region Insert Methods

        bool IBHOpenDataProvider<T>.InsertEntries(List<T> entries)
        {
            return InsertMany(entries, $"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})");
        }

        bool IBHOpenDataProvider<T>.InsertEntry(T entry)
        {
            CheckGenerateValue(entry);
            if (_settings.HasAutoIncrement)
            {
                object? IdEntry = _executionProvider.ExecuteRawScalar($"insert into {ThisTable} ({PropertyNames}) {ReturningCase[0]} values({PropertyParams}) {ReturningCase[1]}", MapObjectToParameters(entry, Tprops));
                if (IdEntry == null) { return false; }
                typeof(T).GetProperty(_settings.MainPrimaryKey)?.SetValue(entry, IdEntry);
                return true;
            }
            return _executionProvider.JustExecute($"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", MapObjectToParameters(entry, Tprops));
        }

        // WITH TRANSACTION

        bool IBHOpenDataProvider<T>.InsertEntries(List<T> entries, BHTransaction bhTransaction)
        {
            return InsertMany(entries, $"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", bhTransaction.transaction);
        }

        bool IBHOpenDataProvider<T>.InsertEntry(T entry, BHTransaction bhTransaction)
        {
            CheckGenerateValue(entry);
            if (_settings.HasAutoIncrement)
            {
                object? IdEntry = _executionProvider.ExecuteRawScalar($"insert into {ThisTable} ({PropertyNames}) {ReturningCase[0]} values({PropertyParams}) {ReturningCase[1]}", MapObjectToParameters(entry, Tprops), bhTransaction.transaction);
                if (IdEntry == null) { return false; }
                typeof(T).GetProperty(_settings.MainPrimaryKey)?.SetValue(entry, IdEntry);
                return true;
            }
            return _executionProvider.JustExecute($"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", MapObjectToParameters(entry, Tprops), bhTransaction.transaction);
        }

        #endregion

        #region Update Methods
        bool IBHOpenDataProvider<T>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            return _executionProvider.JustExecute($"update {ThisTable} set {UpdateParams} where {sql.Columns}", sql.Parameters);
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            string commonColumns = CompareColumnsToEntity(typeof(Columns));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return false;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            return _executionProvider.JustExecute($"update {ThisTable} set {commonColumns} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        bool IBHOpenDataProvider<T>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            return _executionProvider.JustExecute($"update {ThisTable} set {UpdateParams} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction bhTransaction) where Columns : class
        {
            string commonColumns = CompareColumnsToEntity(typeof(Columns));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return false;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            return _executionProvider.JustExecute($"update {ThisTable} set {commonColumns} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }
        #endregion

        #region Delete Methods
        bool IBHOpenDataProvider<T>.DeleteAllEntries()
        {
            return _executionProvider.JustExecute($"delete from {ThisTable}", null);
        }

        bool IBHOpenDataProvider<T>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.JustExecute($"delete from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        bool IBHOpenDataProvider<T>.DeleteAllEntries(BHTransaction bhTransaction)
        {
            return _executionProvider.JustExecute($"delete from {ThisTable}", null, bhTransaction.transaction);
        }

        bool IBHOpenDataProvider<T>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return _executionProvider.JustExecute($"delete from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }
        #endregion

        // ASYNC METHODS

        #region Helper Methods Async
        private async Task<bool> InsertManyAsync<Dto>(List<Dto> entries, string textCommand)
        {
            BlackHoleTransaction bhTransaction = new();
            bool result = true;
            foreach (Dto entry in entries)
            {
                CheckGenerateValue(entry);
                if (_settings.HasAutoIncrement)
                {
                    object? IdEntry = await _executionProvider.ExecuteRawScalarAsync($"insert into {ThisTable} ({PropertyNames}) {ReturningCase[0]} values({PropertyParams}) {ReturningCase[1]}", MapObjectToParameters(entry, Tprops), bhTransaction);
                    if (IdEntry != null) { typeof(Dto).GetProperty(_settings.MainPrimaryKey)?.SetValue(entry, IdEntry); }
                }
                else
                {
                    await _executionProvider.JustExecuteAsync(textCommand, MapObjectToParameters(entry, Tprops), bhTransaction);
                }
            }
            if (!bhTransaction.Commit())
            {
                result = false;
            }
            bhTransaction.Dispose();
            return result;
        }

        // WITH TRANSACTION

        private async Task<bool> InsertManyAsync<Dto>(List<Dto> entries, string textCommand, BlackHoleTransaction bhTransaction)
        {
            bool result = true;
            foreach (Dto entry in entries)
            {
                CheckGenerateValue(entry);
                if (_settings.HasAutoIncrement)
                {
                    object? IdEntry = await _executionProvider.ExecuteRawScalarAsync($"insert into {ThisTable} ({PropertyNames}) {ReturningCase[0]} values({PropertyParams}) {ReturningCase[1]}", MapObjectToParameters(entry, Tprops), bhTransaction);
                    if (IdEntry != null)
                    {
                        typeof(Dto).GetProperty(_settings.MainPrimaryKey)?.SetValue(entry, IdEntry);
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    if (!await _executionProvider.JustExecuteAsync(textCommand, MapObjectToParameters(entry, Tprops), bhTransaction))
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        #endregion

        #region Additional Methods Async

        async Task<bool> IBHOpenDataProvider<T>.AnyAsync()
        {
            string[] oneRow = 1.GetLimiter();
            return await _executionProvider.QueryFirstAsync<T>($"select {oneRow[0]}{PropertyNames} from {ThisTable} where 1=1 {oneRow[1]}", null) != null;
        }

        async Task<bool> IBHOpenDataProvider<T>.AnyAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            string[] oneRow = 1.GetLimiter();
            return await _executionProvider.QueryFirstAsync<T>($"select {oneRow[0]}{PropertyNames} from {ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters) != null;
        }

        async Task<int> IBHOpenDataProvider<T>.CountAsync()
        {
            return await _executionProvider.QueryFirstAsync<int>($"select count(*) from {ThisTable}", null);
        }

        async Task<int> IBHOpenDataProvider<T>.CountWhereAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<int>($"select count(*) from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        async Task<bool> IBHOpenDataProvider<T>.AnyAsync(BHTransaction bhTransaction)
        {
            string[] oneRow = 1.GetLimiter();
            return await _executionProvider.QueryFirstAsync<T>($"select {oneRow[0]}{PropertyNames} from {ThisTable} where 1=1 {oneRow[1]}", null, bhTransaction.transaction) != null;
        }

        async Task<bool> IBHOpenDataProvider<T>.AnyAsync(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            string[] oneRow = 1.GetLimiter();
            return await _executionProvider.QueryFirstAsync<T>($"select {oneRow[0]}{PropertyNames} from {ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters, bhTransaction.transaction) != null; ;
        }

        async Task<int> IBHOpenDataProvider<T>.CountAsync(BHTransaction bhTransaction)
        {
            return await _executionProvider.QueryFirstAsync<int>($"select count(*) from {ThisTable}", null, bhTransaction.transaction);
        }

        async Task<int> IBHOpenDataProvider<T>.CountWhereAsync(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<int>($"select count(*) from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        #endregion

        #region Select Methods Async

        async Task<List<T>> IBHOpenDataProvider<T>.GetAllEntriesAsync()
        {
            return await _executionProvider.QueryAsync<T>($"select {MainPK}{PropertyNames} from {ThisTable}", null);
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

        async Task<List<T>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<T>($"select {MainPK}{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
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

        async Task<T?> IBHOpenDataProvider<T>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<T>($"select {MainPK}{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters);
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

        // WITH TRANSACTION

        async Task<List<T>> IBHOpenDataProvider<T>.GetAllEntriesAsync(BHTransaction bhTransaction)
        {
            return await _executionProvider.QueryAsync<T>($"select {MainPK}{PropertyNames} from {ThisTable}", null, bhTransaction.transaction);
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

        async Task<List<T>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<T>($"select {MainPK}{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
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

        async Task<T?> IBHOpenDataProvider<T>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<T>($"select {MainPK}{PropertyNames} from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
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

        // WITH ORDER BY

        async Task<List<T>> IBHOpenDataProvider<T>.GetAllEntriesAsync(Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            return await _executionProvider.QueryAsync<T>($"select {PropertyNames} from {ThisTable} {orderClass.OrderByToSql(IsMyShit)}", null);
        }

        async Task<List<Dto>> IBHOpenDataProvider<T>.GetAllEntriesAsync<Dto>(Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            return await _executionProvider.QueryAsync<Dto>($"select {commonColumns} from {ThisTable} {orderClass.OrderByToSql(IsMyShit)}", null);
        }

        async Task<T?> IBHOpenDataProvider<T>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters);
        }

        async Task<Dto?> IBHOpenDataProvider<T>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters);
        }

        async Task<List<T>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters);
        }

        async Task<List<Dto>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            };
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters);
        }

        // WITH ORDER BY AND TRANSACTION

        async Task<List<T>> IBHOpenDataProvider<T>.GetAllEntriesAsync(Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            return await _executionProvider.QueryAsync<T>($"select {PropertyNames} from {ThisTable} {orderClass.OrderByToSql(IsMyShit)}", null, bhTransaction.transaction);
        }

        async Task<List<Dto>> IBHOpenDataProvider<T>.GetAllEntriesAsync<Dto>(Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            }
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            return await _executionProvider.QueryAsync<Dto>($"select {commonColumns} from {ThisTable} {orderClass.OrderByToSql(IsMyShit)}", null, bhTransaction.transaction);
        }

        async Task<T?> IBHOpenDataProvider<T>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<Dto?> IBHOpenDataProvider<T>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return default;
            }
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryFirstAsync<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<T>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<T>($"select {PropertyNames} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<List<Dto>> IBHOpenDataProvider<T>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction bhTransaction) where Dto : class
        {
            string commonColumns = CompareDtoToEntity(typeof(Dto));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return new List<Dto>();
            };
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.QueryAsync<Dto>($"select {commonColumns} from {ThisTable} where {sql.Columns}{orderClass.OrderByToSql(IsMyShit)}", sql.Parameters, bhTransaction.transaction);
        }
        #endregion

        #region Insert Methods Async
        async Task<bool> IBHOpenDataProvider<T>.InsertEntriesAsync(List<T> entries)
        {
            return await InsertManyAsync(entries, $"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})");
        }

        async Task<bool> IBHOpenDataProvider<T>.InsertEntryAsync(T entry)
        {
            CheckGenerateValue(entry);
            if (_settings.HasAutoIncrement)
            {
                object? IdEntry = await _executionProvider.ExecuteRawScalarAsync($"insert into {ThisTable} ({PropertyNames}) {ReturningCase[0]} values({PropertyParams}) {ReturningCase[1]}", MapObjectToParameters(entry, Tprops));
                if (IdEntry == null) { return false; }
                typeof(T).GetProperty(_settings.MainPrimaryKey)?.SetValue(entry, IdEntry);
                return true;
            }
            return await _executionProvider.JustExecuteAsync($"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", MapObjectToParameters(entry, Tprops));
        }

        // WITH TRANSACTION

        async Task<bool> IBHOpenDataProvider<T>.InsertEntriesAsync(List<T> entries, BHTransaction bhTransaction)
        {
            return await InsertManyAsync(entries, $"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", bhTransaction.transaction);
        }

        async Task<bool> IBHOpenDataProvider<T>.InsertEntryAsync(T entry, BHTransaction bhTransaction)
        {
            CheckGenerateValue(entry);
            if (_settings.HasAutoIncrement)
            {
                object? IdEntry = await _executionProvider.ExecuteRawScalarAsync($"insert into {ThisTable} ({PropertyNames}) {ReturningCase[0]} values({PropertyParams}) {ReturningCase[1]}", MapObjectToParameters(entry, Tprops), bhTransaction.transaction);
                if (IdEntry == null) { return false; }
                typeof(T).GetProperty(_settings.MainPrimaryKey)?.SetValue(entry, IdEntry);
                return true;
            }
            return await _executionProvider.JustExecuteAsync($"insert into {ThisTable} ({PropertyNames}) values ({PropertyParams})", MapObjectToParameters(entry, Tprops), bhTransaction.transaction);
        }
        #endregion

        #region Update Methods Async
        async Task<bool> IBHOpenDataProvider<T>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            return await _executionProvider.JustExecuteAsync($"update {ThisTable} set {UpdateParams} where {sql.Columns}", sql.Parameters);
        }

        async Task<bool> IBHOpenDataProvider<T>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            string commonColumns = CompareColumnsToEntity(typeof(Columns));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return false;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            return await _executionProvider.JustExecuteAsync($"update {ThisTable} set {commonColumns} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        async Task<bool> IBHOpenDataProvider<T>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            return await _executionProvider.JustExecuteAsync($"update {ThisTable} set {UpdateParams} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }

        async Task<bool> IBHOpenDataProvider<T>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction bhTransaction) where Columns : class
        {
            string commonColumns = CompareColumnsToEntity(typeof(Columns));
            if (string.IsNullOrEmpty(commonColumns))
            {
                return false;
            }
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            sql.AdditionalParameters(entry);
            return await _executionProvider.JustExecuteAsync($"update {ThisTable} set {commonColumns} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }
        #endregion

        #region Delete Methods Async
        async Task<bool> IBHOpenDataProvider<T>.DeleteAllEntriesAsync()
        {
            return await _executionProvider.JustExecuteAsync($"delete from {ThisTable}", null);
        }

        async Task<bool> IBHOpenDataProvider<T>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.JustExecuteAsync($"delete from {ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        async Task<bool> IBHOpenDataProvider<T>.DeleteAllEntriesAsync(BHTransaction bhTransaction)
        {
            return await _executionProvider.JustExecuteAsync($"delete from {ThisTable}", null, bhTransaction.transaction);
        }

        async Task<bool> IBHOpenDataProvider<T>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction bhTransaction)
        {
            ColumnsAndParameters sql = predicate.Body.SplitMembers<T>(IsMyShit, string.Empty, null, 0);
            return await _executionProvider.JustExecuteAsync($"delete from {ThisTable} where {sql.Columns}", sql.Parameters, bhTransaction.transaction);
        }
        #endregion
    }
}
