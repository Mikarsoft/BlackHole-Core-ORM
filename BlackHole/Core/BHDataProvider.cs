using BlackHole.Entities;
using System.Reflection;
using System.Linq.Expressions;
using BlackHole.Engine;
using System.Text;

namespace BlackHole.Core
{
    /// <summary>
    /// Makes all the communication between the Database Table and The Specified Entity
    /// </summary>
    /// <typeparam name="T">BlackHoleEntity</typeparam>
    /// <typeparam name="G">The type of Entity's Id</typeparam>
    public class BHDataProvider<T, G> : IBHDataProvider<T, G> where T : BlackHoleEntity<G> where G :IComparable<G>
    {
        #region Ctor
        private EntityContext _context;
        private readonly IDataProvider _dataProvider;
        /// <summary>
        /// Create a Data Provider that Automatically Communicates with the Database Using the Black Hole Entity, that you pass in.
        /// </summary>
        public BHDataProvider()
        {
            Type EntityType = typeof(T);
            _context = EntityType.GetEntityContext<G>();
            _dataProvider = _context.ConnectionIndex.GetDataProvider();
        }

        #endregion

        #region Common Helper Methods

        private bool SetIds(List<T> entities, List<G?> Ids)
        {
            if (Ids.Any() && Ids.Count == entities.Count)
            {
                for(int i = 0; i < Ids.Count; i++)
                {
                    if(!entities[i].SetId(Ids[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private string CompareDtoToEntity(Type dto)
        {
            StringBuilder PNsb = new();
            foreach (PropertyInfo property in dto.GetProperties())
            {
                if (_context.Columns.Contains(property.Name)
                    && typeof(T).GetProperty(property.Name)?.PropertyType == property.PropertyType)
                {
                    PNsb.Append($",{property.Name.UseNameQuotes(_context.IsQuotedDb)}");
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
                if (property.Name != "Id" && _context.Columns.Contains(property.Name)
                    && typeof(T).GetProperty(property.Name)?.PropertyType == property.PropertyType)
                {
                    PNsb.Append($",{property.Name.UseNameQuotes(_context.IsQuotedDb)} = @{property.Name}");
                }
            }
            PNsb.Append(' ');
            return PNsb.ToString().Remove(0, 1);
        }
        #endregion

        // SYNC METHODS

        #region Helper Methods
        private bool UpdateMany(List<T> entries, string updateCommand)
        {
            BHTransaction bhTransaction = new();

            foreach (T entry in entries)
            {
                _dataProvider.ExecuteEntry(updateCommand, entry, bhTransaction.transaction, _context.ConnectionIndex);
            }

            bool result = bhTransaction.Commit();
            bhTransaction.Dispose();

            return result;
        }

        private G? GetIdFromPredicate(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<G>($"select {_context.ThisId} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<G>($"select {_context.ThisId} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        private List<G> GetIdsFromPredicate(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<G>($"select {_context.ThisId} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.Query<G>($"select {_context.ThisId} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        private bool UpdateMany(List<T> entries, string updateCommand, BlackHoleTransaction bhTransaction)
        {
            bool result = true;

            foreach (T entry in entries)
            {
                if (!_dataProvider.ExecuteEntry(updateCommand, entry, bhTransaction, _context.ConnectionIndex))
                {
                    result = false;
                }
            }

            return result;
        }

        private G? GetIdFromPredicate(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers<T>(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<G>($"select {_context.ThisId} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<G>($"select {_context.ThisId} from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        private List<G> GetIdsFromPredicate(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<G>($"select {_context.ThisId} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<G>($"select {_context.ThisId} from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }
        #endregion

        #region Additional Methods
        bool IBHDataProvider<T, G>.Any()
        {
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {oneRow[1]}", null) != null;
            }
            return _dataProvider.QueryFirst<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where 1 = 1 {oneRow[1]}", null) != null;
        }

        bool IBHDataProvider<T, G>.Any(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {oneRow[0]} {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}", sql.Parameters) != null;
            }
            return _dataProvider.QueryFirst<T>($"select {oneRow[0]} {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters) != null;
        }

        int IBHDataProvider<T, G>.Count()
        {
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {_context.ThisInactive} = 0", null);
            }
            return _dataProvider.QueryFirst<int>($"select count({_context.ThisId}) from {_context.ThisTable}", null);
        }

        int IBHDataProvider<T, G>.CountWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers<T>(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        G? IBHDataProvider<T, G>.GetIdWhere(Expression<Func<T, bool>> predicate)
        {
            return GetIdFromPredicate(predicate);
        }

        List<G> IBHDataProvider<T, G>.GetIdsWhere(Expression<Func<T, bool>> predicate)
        {
            return GetIdsFromPredicate(predicate);
        }

        // WITH TRANSACTION

        bool IBHDataProvider<T, G>.Any(IBHTransaction bhTransaction)
        {
            string[] oneRow = 1.GetLimiter();
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {oneRow[1]}",
                    null, transactionBh.transaction, _context.ConnectionIndex) != null;
            }
            return _dataProvider.QueryFirst<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where 1 = 1 {oneRow[1]}",
                null, transactionBh.transaction, _context.ConnectionIndex) != null;
        }

        bool IBHDataProvider<T, G>.Any(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex) != null;
            }
            return _dataProvider.QueryFirst<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex) != null;
        }

        int IBHDataProvider<T, G>.Count(IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {_context.ThisInactive} = 0",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<int>($"select count({_context.ThisId}) from {_context.ThisTable}",
                null, transactionBh.transaction, _context.ConnectionIndex);
        }

        int IBHDataProvider<T, G>.CountWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        G? IBHDataProvider<T, G>.GetIdWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            return GetIdFromPredicate(predicate, bhTransaction);
        }

        List<G> IBHDataProvider<T, G>.GetIdsWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            return GetIdsFromPredicate(predicate, bhTransaction);
        }
        #endregion

        #region Select Methods
        List<T> IBHDataProvider<T, G>.GetAllEntries()
        {
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0", null);
            }
            return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable}", null);
        }

        List<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>() where Dto : class
        {
            if (_context.WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0", null);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable}", null);
        }

        List<T> IBHDataProvider<T, G>.GetAllInactiveEntries()
        {
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 1", null);
            }
            return new List<T>();
        }

        T? IBHDataProvider<T, G>.GetEntryById(G Id)
        {
            BHParameters Params = new BHParameters();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 0", Params.Parameters);
            }
            return _dataProvider.QueryFirst<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisId} = @Id", Params.Parameters);
        }

        Dto? IBHDataProvider<T, G>.GetEntryById<Dto>(G Id) where Dto : class
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 0", Params.Parameters);
            }
            return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisId} = @Id", Params.Parameters);
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null , 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters);
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {oneRow[0]}{CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<Dto>($"select {oneRow[0]}{CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters);
        }

        List<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        List<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        List<T> IBHDataProvider<T, G>.GetAllEntries(IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0", null,
                    transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable}", null,
                transactionBh.transaction, _context.ConnectionIndex);
        }

        List<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>(IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0", null,
                    transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable}", null,
                transactionBh.transaction, _context.ConnectionIndex);
        }

        List<T> IBHDataProvider<T, G>.GetAllInactiveEntries(IBHTransaction bhTransaction)
        {
            if (_context.WithActivator)
            {
                BHTransaction transactionBh = (BHTransaction)bhTransaction;

                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 1", null,
                    transactionBh.transaction, _context.ConnectionIndex);
            }
            return new List<T>();
        }

        T? IBHDataProvider<T, G>.GetEntryById(G Id, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 0",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisId} = @Id",
                Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        Dto? IBHDataProvider<T, G>.GetEntryById<Dto>(G Id, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 0",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisId} = @Id",
                Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {oneRow[0]} {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<T>($"select {oneRow[0]} {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {oneRow[0]}{CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<Dto>($"select {oneRow[0]}{CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        List<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        List<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        // ORDER BY

        List<T> IBHDataProvider<T, G>.GetAllEntries(Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
            }
            return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
        }

        List<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>(Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
        }

        List<T> IBHDataProvider<T, G>.GetAllInactiveEntries(Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 1 {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
            }
            return new List<T>();
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
            }
            return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
        }

        List<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
            }
            return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
        }

        List<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy) where Dto : class
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
        }

        // ORDER BY WITH TRANSACTION

        List<T> IBHDataProvider<T, G>.GetAllEntries(Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                null, transactionBh.transaction, _context.ConnectionIndex);
        }

        List<Dto> IBHDataProvider<T, G>.GetAllEntries<Dto>(Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<Dto>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                null, transactionBh.transaction, _context.ConnectionIndex);
        }

        List<T> IBHDataProvider<T, G>.GetAllInactiveEntries(Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction)
        {
            if (_context.WithActivator)
            {
                BHOrderBy<T> orderClass = new();
                orderBy.Invoke(orderClass);
                BHTransaction transactionBh = (BHTransaction)bhTransaction;
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return new List<T>();
        }

        T? IBHDataProvider<T, G>.GetEntryWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        Dto? IBHDataProvider<T, G>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.QueryFirst<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        List<T> IBHDataProvider<T, G>.GetEntriesWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        List<Dto> IBHDataProvider<T, G>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.Query<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }
        #endregion

        #region Insert Methods

        bool IBHDataProvider<T, G>.InsertEntry(T entry)
        {
            G? id = _dataProvider.InsertScalar<T, G>($"insert into {_context.ThisTable} ({_context.PropertyNames},{_context.ThisInactive}",
                $"values ({_context.PropertyParams}, 0", entry, _context.UseIdGenerator, _context.ReturningId);
            return entry.SetId(id);
        }

        bool IBHDataProvider<T, G>.InsertEntries(List<T> entries)
        {
            List<G?> Ids = new();
            using(BHTransaction bhTransaction = new())
            {
                Ids = _dataProvider.MultiInsertScalar<T, G>($"insert into {_context.ThisTable} ({_context.PropertyNames},{_context.ThisInactive}",
                    $"values ({_context.PropertyParams}, 0", entries, bhTransaction.transaction, _context.UseIdGenerator, _context.ReturningId, _context.ConnectionIndex);
            }
            return SetIds(entries, Ids);
        }

        // WITH TRANSACTION

        bool IBHDataProvider<T, G>.InsertEntry(T entry, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            G? id = _dataProvider.InsertScalar<T, G>($"insert into {_context.ThisTable} ({_context.PropertyNames}, {_context.ThisInactive}",
                $"values ({_context.PropertyParams}, 0", entry, transactionBh.transaction, _context.UseIdGenerator, _context.ReturningId, _context.ConnectionIndex);
            return entry.SetId(id);
        }

        bool IBHDataProvider<T, G>.InsertEntries(List<T> entries, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            List<G?> Ids = _dataProvider.MultiInsertScalar<T, G>($"insert into {_context.ThisTable} ({_context.PropertyNames},{_context.ThisInactive}",
                $"values ({_context.PropertyParams}, 0", entries, transactionBh.transaction, _context.UseIdGenerator, _context.ReturningId, _context.ConnectionIndex);
            return SetIds(entries, Ids);
        }
        #endregion

        #region Update Methods
        bool IBHDataProvider<T, G>.UpdateEntryById(T entry)
        {
            return _dataProvider.ExecuteEntry($"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisId} = @Id", entry);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById<Columns>(T entry) where Columns : class
        {
            return _dataProvider.ExecuteEntry($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisId} = @Id", entry);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById(List<T> entries)
        {
            return UpdateMany(entries, $"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisId} = @Id");
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById<Columns>(List<T> entries) where Columns : class
        {
            return UpdateMany(entries, $"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisId}=@Id");
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            sql.AdditionalParameters(entry);
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.UpdateParams} where {sql.Columns}", sql.Parameters);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            sql.AdditionalParameters(entry);
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.JustExecute($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        bool IBHDataProvider<T, G>.UpdateEntryById(T entry, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            return _dataProvider.ExecuteEntry($"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisId} = @Id",
                entry, transactionBh.transaction, _context.ConnectionIndex);
        }

        bool IBHDataProvider<T, G>.UpdateEntryById<Columns>(T entry, IBHTransaction bhTransaction) where Columns : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            return _dataProvider.ExecuteEntry($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisId} = @Id",
                entry, transactionBh.transaction, _context.ConnectionIndex);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById(List<T> entries, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            return UpdateMany(entries, $"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisId} = @Id", transactionBh.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesById<Columns>(List<T> entries, IBHTransaction bhTransaction) where Columns : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            return UpdateMany(entries, $"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisId} = @Id", transactionBh.transaction);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            sql.AdditionalParameters(entry);
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.UpdateParams} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        bool IBHDataProvider<T, G>.UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, IBHTransaction bhTransaction) where Columns : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            sql.AdditionalParameters(entry);
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.JustExecute($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }
        #endregion

        #region Delete Methods

        bool IBHDataProvider<T, G>.DeleteAllEntries()
        {
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {_context.ThisInactive} = 0", null);
            }
            return _dataProvider.JustExecute($"delete from {_context.ThisTable}", null);
        }

        bool IBHDataProvider<T, G>.DeleteAllEntries(IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {_context.ThisInactive} = 0",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.JustExecute($"delete from {_context.ThisTable}", null, transactionBh.transaction, _context.ConnectionIndex);
        }

        bool IBHDataProvider<T, G>.DeleteEntryById(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"Update {_context.ThisTable} set {_context.ThisInactive} = 1 where {_context.ThisId} = @Id", Params.Parameters);
            }
            return _dataProvider.JustExecute($"delete from {_context.ThisTable} where {_context.ThisId} = @Id", Params.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteInactiveEntryById(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"delete from {_context.ThisTable} where {_context.ThisId}= @Id and {_context.ThisInactive} = 1", Params.Parameters);
        }

        bool IBHDataProvider<T, G>.ReactivateEntryById(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.ThisInactive} = 0 where {_context.ThisId} = @Id and {_context.ThisInactive} = 1", Params.Parameters);
        }

        bool IBHDataProvider<T, G>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {sql.Columns}", sql.Parameters);
            }
            return _dataProvider.JustExecute($"delete from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        bool IBHDataProvider<T, G>.DeleteEntryById(G Id, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {_context.ThisId} = @Id",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.JustExecute($"delete from {_context.ThisTable} where {_context.ThisId} = @Id",
                Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        bool IBHDataProvider<T, G>.DeleteInactiveEntryById(G Id, IBHTransaction bhTransaction)
        {
            if (_context.WithActivator)
            {
                BHTransaction transactionBh = (BHTransaction)bhTransaction;
                BHParameters Params = new();
                Params.Add("Id", Id);
                return _dataProvider.JustExecute($"delete from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 1",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return false;
        }

        bool IBHDataProvider<T, G>.ReactivateEntryById(G Id, IBHTransaction bhTransaction)
        {
            if (_context.WithActivator)
            {
                BHTransaction transactionBh = (BHTransaction)bhTransaction;
                BHParameters Params = new();
                Params.Add("Id", Id);
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.ThisInactive} = 0 where {_context.ThisId} = @Id and {_context.ThisInactive} = 1",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return false;
        }

        bool IBHDataProvider<T, G>.DeleteEntriesWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return _dataProvider.JustExecute($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return _dataProvider.JustExecute($"delete from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }
        #endregion

        // ASYNC METHODS

        #region Helper Methods Async
        private async Task<bool> UpdateManyAsync(List<T> entries, string updateCommand)
        {
            BHTransaction bhTransaction = new();

            foreach (T entry in entries)
            {
                await _dataProvider.ExecuteEntryAsync(updateCommand, entry, bhTransaction.transaction, _context.ConnectionIndex);
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
                if (!await _dataProvider.ExecuteEntryAsync(updateCommand, entry, bhTransaction, _context.ConnectionIndex))
                {
                    result = false;
                }
            }
            return result;
        }

        private async Task<List<G>> GetIdsFromPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<G>($"select {_context.ThisId} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryAsync<G>($"select {_context.ThisId} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION
        private async Task<G?> GetIdFromPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<G>($"select {_context.ThisId} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<G>($"select {_context.ThisId} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        private async Task<G?> GetIdFromPredicateAsync(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<G>($"select {_context.ThisId} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<G>($"select {_context.ThisId} from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        private async Task<List<G>> GetIdsFromPredicateAsync(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<G>($"select {_context.ThisId} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<G>($"select {_context.ThisId} from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }
        #endregion

        #region Additional Methods Async
        async Task<bool> IBHDataProvider<T, G>.AnyAsync()
        {
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {oneRow[1]}", null) != null;
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where 1 = 1 {oneRow[1]}", null) != null;
        }

        async Task<bool> IBHDataProvider<T, G>.AnyAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]} {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}", sql.Parameters) != null;
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]} {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters) != null;
        }

        async Task<int> IBHDataProvider<T, G>.CountAsync()
        {
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {_context.ThisInactive} = 0", null);
            }
            return await _dataProvider.QueryFirstAsync<int>($"select count({_context.ThisId}) from {_context.ThisTable}", null);
        }

        async Task<int> IBHDataProvider<T, G>.CountWhereAsync(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<G?> IBHDataProvider<T, G>.GetIdAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            return await GetIdFromPredicateAsync(predicate);
        }

        async Task<List<G>> IBHDataProvider<T, G>.GetIdsAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            return await GetIdsFromPredicateAsync(predicate);
        }

        // WITH TRANSACTION

        async Task<bool> IBHDataProvider<T, G>.AnyAsync(IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {oneRow[1]}",
                    null, transactionBh.transaction, _context.ConnectionIndex) != null;
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where 1 = 1 {oneRow[1]}",
                null, transactionBh.transaction, _context.ConnectionIndex) != null;
        }

        async Task<bool> IBHDataProvider<T, G>.AnyAsync(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex) != null;
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex) != null;
        }

        async Task<int> IBHDataProvider<T, G>.CountAsync(IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {_context.ThisInactive} = 0",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<int>($"select count({_context.ThisId}) from {_context.ThisTable}",
                null, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<int> IBHDataProvider<T, G>.CountWhereAsync(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<int>($"select count({_context.ThisId}) from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<G?> IBHDataProvider<T, G>.GetIdAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            return await GetIdFromPredicateAsync(predicate, bhTransaction);
        }

        async Task<List<G>> IBHDataProvider<T, G>.GetIdsAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            return await GetIdsFromPredicateAsync(predicate, bhTransaction);
        }
        #endregion

        #region Select Methods Async

        async Task<List<T>> IBHDataProvider<T, G>.GetAllEntriesAsync()
        {
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0", null);
            }
            return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable}", null);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>() where Dto : class
        {
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0", null);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable}", null);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync()
        {
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 1", null);
            }
            return new List<T>();
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryByIdAsync(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 0", Params.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisId} = @Id", Params.Parameters);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryByIdAsync(G Id, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 0",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisId} = @Id",
                Params.Parameters,transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryByIdAsync<Dto>(G Id) where Dto : class
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 0", Params.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisId} = @Id", Params.Parameters);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {oneRow[0]}{CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {oneRow[0]}{CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}", sql.Parameters);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        async Task<List<T>> IBHDataProvider<T, G>.GetAllEntriesAsync(IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable}",
                null, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>(IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable}",
                null, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync(IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 1",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return new List<T>();
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryByIdAsync<Dto>(G Id, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 0",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisId} = @Id",
                Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {oneRow[0]}{_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            string[] oneRow = 1.GetLimiter();
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {oneRow[0]}{CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{oneRow[1]}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {oneRow[0]}{CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{oneRow[1]}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        // WITH ORDER BY

        async Task<List<T>> IBHDataProvider<T, G>.GetAllEntriesAsync(Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
            }
            return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>(Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync(Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 1 {orderClass.OrderByToSql(_context.IsQuotedDb)}", null);
            }
            return new List<T>();
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy)
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : class
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy)
        {

            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
            }
            return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy) where Dto : class
        {
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}", sql.Parameters);
        }

        // WITH ORDER BY AND TRANSACTION

        async Task<List<T>> IBHDataProvider<T, G>.GetAllEntriesAsync(Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                null, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetAllEntriesAsync<Dto>(Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                null, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetAllInactiveEntriesAsync(Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 {orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return new List<T>();
        }

        async Task<T?> IBHDataProvider<T, G>.GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<Dto?> IBHDataProvider<T, G>.GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            orderClass.OrderBy.TakeWithOffset(0, 1);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryFirstAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<List<T>> IBHDataProvider<T, G>.GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<T>($"select {_context.ThisId},{_context.PropertyNames} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<List<Dto>> IBHDataProvider<T, G>.GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy, IBHTransaction bhTransaction) where Dto : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHOrderBy<T> orderClass = new();
            orderBy.Invoke(orderClass);
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {_context.ThisInactive} = 0 and {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.QueryAsync<Dto>($"select {CompareDtoToEntity(typeof(Dto))} from {_context.ThisTable} where {sql.Columns}{orderClass.OrderByToSql(_context.IsQuotedDb)}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }
        #endregion

        #region Insert Methods Async

        async Task<bool> IBHDataProvider<T, G>.InsertEntryAsync(T entry)
        {
            G? Id  = await _dataProvider.InsertScalarAsync<T, G>($"insert into {_context.ThisTable} ({_context.PropertyNames},{_context.ThisInactive}",
                $"values ({_context.PropertyParams}, 0", entry, _context.UseIdGenerator, _context.ReturningId);
            return entry.SetId(Id);
        }

        async Task<bool> IBHDataProvider<T, G>.InsertEntriesAsync(List<T> entries)
        {
            List<G?> Ids = new();
            using(BHTransaction bhTransaction = new())
            {
                Ids = await _dataProvider.MultiInsertScalarAsync<T, G>($"insert into {_context.ThisTable} ({_context.PropertyNames},{_context.ThisInactive}",
                    $"values ({_context.PropertyParams}, 0", entries, bhTransaction.transaction, _context.UseIdGenerator, _context.ReturningId, _context.ConnectionIndex);
            }
            return SetIds(entries, Ids);
        }

        // WITH TRANSACTION

        async Task<bool> IBHDataProvider<T, G>.InsertEntryAsync(T entry, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            G? Id = await _dataProvider.InsertScalarAsync<T, G>($"insert into {_context.ThisTable} ({_context.PropertyNames},{_context.ThisInactive}",
                $"values ({_context.PropertyParams}, 0", entry, transactionBh.transaction, _context.UseIdGenerator, _context.ReturningId, _context.ConnectionIndex);
            return entry.SetId(Id);
        }

        async Task<bool> IBHDataProvider<T, G>.InsertEntriesAsync(List<T> entries, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            List<G?> Ids = await _dataProvider.MultiInsertScalarAsync<T, G>($"insert into {_context.ThisTable} ({_context.PropertyNames},{_context.ThisInactive}",
                $"values ({_context.PropertyParams}, 0", entries, transactionBh.transaction, _context.UseIdGenerator, _context.ReturningId, _context.ConnectionIndex);
            return SetIds(entries, Ids);
        }
        #endregion

        #region Update Methods Async

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync(T entry)
        {
            return await _dataProvider.ExecuteEntryAsync($"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisId} = @Id", entry);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync<Columns>(T entry) where Columns : class
        {
            return await _dataProvider.ExecuteEntryAsync($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisId} = @Id", entry);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync(List<T> entries)
        {
            return await UpdateManyAsync(entries, $"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisId} = @Id");
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync<Columns>(List<T> entries) where Columns : class
        {
            return await UpdateManyAsync(entries, $"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisId} = @Id");
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            sql.AdditionalParameters(entry);
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.UpdateParams} where {sql.Columns}", sql.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            sql.AdditionalParameters(entry);
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisInactive} = 0 and {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync(T entry, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            return await _dataProvider.ExecuteEntryAsync($"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisId} = @Id",
                entry, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntryByIdAsync<Columns>(T entry, IBHTransaction bhTransaction) where Columns : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            return await _dataProvider.ExecuteEntryAsync($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisId} = @Id",
                entry, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync(List<T> entries, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            return await UpdateManyAsync(entries, $"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisId} = @Id", transactionBh.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesByIdAsync<Columns>(List<T> entries, IBHTransaction bhTransaction) where Columns : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            return await UpdateManyAsync(entries, $"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisId} = @Id", transactionBh.transaction);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            sql.AdditionalParameters(entry);
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.UpdateParams} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.UpdateParams} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<bool> IBHDataProvider<T, G>.UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, IBHTransaction bhTransaction) where Columns : class
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            sql.AdditionalParameters(entry);
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {_context.ThisInactive} = 0 and {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {CompareColumnsToEntity(typeof(Columns))} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }
        #endregion

        #region Delete Methods Async
        async Task<bool> IBHDataProvider<T, G>.DeleteAllEntriesAsync()
        {
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {_context.ThisInactive} = 0", null);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {_context.ThisTable}", null);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntryByIdAsync(G Id)
        {
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {_context.ThisId} = @Id", Params.Parameters);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {_context.ThisTable} where {_context.ThisId} = @Id", Params.Parameters);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteInactiveEntryByIdAsync(G Id)
        {
            if (_context.WithActivator)
            {
                BHParameters Params = new();
                Params.Add("Id", Id);
                return await _dataProvider.JustExecuteAsync($"delete from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 1", Params.Parameters);
            }
            return false;
        }

        async Task<bool> IBHDataProvider<T, G>.ReactivateEntryByIdAsync(G Id)
        {
            if (_context.WithActivator)
            {
                BHParameters Params = new();
                Params.Add("Id", Id);
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.ThisInactive} = 0 where {_context.ThisId} = @Id and {_context.ThisInactive} = 1", Params.Parameters);
            }
            return false;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate)
        {
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {sql.Columns}", sql.Parameters);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {_context.ThisTable} where {sql.Columns}", sql.Parameters);
        }

        // WITH TRANSACTION

        async Task<bool> IBHDataProvider<T, G>.DeleteAllEntriesAsync(IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {_context.ThisInactive} = 0",
                    null, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {_context.ThisTable}", null, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntryByIdAsync(G Id, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            BHParameters Params = new();
            Params.Add("Id", Id);
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {_context.ThisId} = @Id",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {_context.ThisTable} where {_context.ThisId} = @Id",
                Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteInactiveEntryByIdAsync(G Id, IBHTransaction bhTransaction)
        {
            if (_context.WithActivator)
            {
                BHTransaction transactionBh = (BHTransaction)bhTransaction;
                BHParameters Params = new();
                Params.Add("Id", Id);
                return await _dataProvider.JustExecuteAsync($"delete from {_context.ThisTable} where {_context.ThisId} = @Id and {_context.ThisInactive} = 1",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return false;
        }

        async Task<bool> IBHDataProvider<T, G>.ReactivateEntryByIdAsync(G Id, IBHTransaction bhTransaction)
        {
            if (_context.WithActivator)
            {
                BHTransaction transactionBh = (BHTransaction)bhTransaction;
                BHParameters Params = new();
                Params.Add("Id", Id);
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.ThisInactive} = 0 where {_context.ThisId} = @Id and {_context.ThisInactive} = 1",
                    Params.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return false;
        }

        async Task<bool> IBHDataProvider<T, G>.DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction bhTransaction)
        {
            BHTransaction transactionBh = (BHTransaction)bhTransaction;
            ColumnsAndParameters sql = predicate.SplitMembers(_context.IsQuotedDb, string.Empty, null, 0, _context.ThisSchema, _context.ConnectionIndex);
            if (_context.WithActivator)
            {
                return await _dataProvider.JustExecuteAsync($"update {_context.ThisTable} set {_context.ThisInactive} = 1 where {sql.Columns}",
                    sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
            }
            return await _dataProvider.JustExecuteAsync($"delete from {_context.ThisTable} where {sql.Columns}",
                sql.Parameters, transactionBh.transaction, _context.ConnectionIndex);
        }

        IBHTransaction IBHDataProvider<T, G>.BeginIBHTransaction()
        {
            return new BHTransaction();
        }
        #endregion
    }
}