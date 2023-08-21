using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BHOpenDataProvider<T> : IBHOpenDataProvider<T> where T : IBHOpenEntity<T>
    {
        private readonly PKSettings<T> _settings;

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


        }

        bool IBHOpenDataProvider<T>.DeleteAllEntries()
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.DeleteAllEntries(BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        List<T> IBHOpenDataProvider<T>.GetAllEntries()
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllEntries(BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public List<Dto> GetAllEntries<Dto>() where Dto : class
        {
            throw new NotImplementedException();
        }

        public List<Dto> GetAllEntries<Dto>(BHTransaction transaction) where Dto : class
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllInactiveEntries()
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllInactiveEntries(BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            throw new NotImplementedException();
        }

        public List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class
        {
            throw new NotImplementedException();
        }

        public T? GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public T? GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            throw new NotImplementedException();
        }

        public Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class
        {
            throw new NotImplementedException();
        }

        public List<T> InsertEntries(List<T> entries)
        {
            throw new NotImplementedException();
        }

        public List<T> InsertEntries(List<T> entries, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public T? InsertEntry(T entry)
        {
            throw new NotImplementedException();
        }

        public T? InsertEntry(T entry, BHTransaction transaction)
        {
            
            throw new NotImplementedException();
        }

        public bool UpdateEntriesById(List<T> entries)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntriesById(List<T> entries, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntriesById<Columns>(List<T> entries) where Columns : class
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntriesById<Columns>(List<T> entries, BHTransaction transaction) where Columns : class
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction transaction) where Columns : class
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntryById(T entry)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntryById(T entry, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntryById<Columns>(T entry) where Columns : class
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntryById<Columns>(T entry, BHTransaction transaction) where Columns : class
        {
            throw new NotImplementedException();
        }
    }
}
