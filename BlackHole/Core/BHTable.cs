using BlackHole.Engine;
using BlackHole.Entities;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    internal class BHTable<T, G> : BHTable<T>, IBHTable<T, G> where T : BHEntityAI<G> where G : IComparable<G>
    {
        #region Ctor
        internal EntityContext _context;
        private readonly IDataProvider _dataProvider;

        /// <summary>
        /// Create a Data Provider that Automatically Communicates with the Database Using the Black Hole Entity, that you pass in.
        /// </summary>
        public BHTable()
        {
            Type EntityType = typeof(T);
            _context = EntityType.GetEntityContext<G>();
            _dataProvider = _context.ConnectionIndex.GetDataProvider();
        }

        public T? GetById(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Dto? GetById<Dto>(G Id, IBHTransaction? transaction = null) where Dto : BHDto<G>
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<T> UpdateById(T entry, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<T, Dto> UpdateById<Dto>(Dto entry, IBHTransaction? transaction = null) where Dto : BHDto<G>
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<T> UpdateById(List<T> entries, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<Dto> UpdateById<Dto>(List<Dto> entries, IBHTransaction? transaction = null) where Dto : BHDto<G>
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveById(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool RestoreById(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public List<G> GetIdsWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetByIdAsync(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<Dto?> GetByIdAsync<Dto>(G Id, IBHTransaction? transaction = null) where Dto : BHDto<G>
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveByIdAsync(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestoreByIdAsync(G Id, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<G>> GetIdsAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

    internal class BHTable<T> : BHContextBase, IBHTable<T> where T : class, BHEntityIdentifier
    {
        public bool Any(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool Any(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public int Count(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllEntries(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllEntriesAsync(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool InsertEntries(IBHQuery<T> entries, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertEntriesAsync(IBHQuery<T> entries, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public bool InsertEntry(T entry, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertEntryAsync(T entry, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public IBHQuerySearchable<T> Select(IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public IBHQueryJoinable<Dto, T> Select<Dto>(IBHTransaction? transaction = null) where Dto : BHDto
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<T> UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, IBHTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public IBHQueryUpdatable<T, Columns> UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, IBHTransaction? transaction = null) where Columns : class
        {
            throw new NotImplementedException();
        }
    }
}
