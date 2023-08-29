using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BHOpenDataProvider<T> : IBHOpenDataProvider<T> where T : BHOpenEntity<T>
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

        List<T> IBHOpenDataProvider<T>.GetAllEntries(BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        List<Dto> IBHOpenDataProvider<T>.GetAllEntries<Dto>() where Dto : class
        {
            throw new NotImplementedException();
        }

        List<Dto> IBHOpenDataProvider<T>.GetAllEntries<Dto>(BHTransaction transaction) where Dto : class
        {
            throw new NotImplementedException();
        }

        List<T> IBHOpenDataProvider<T>.GetAllInactiveEntries()
        {
            throw new NotImplementedException();
        }

        List<T> IBHOpenDataProvider<T>.GetAllInactiveEntries(BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        List<T> IBHOpenDataProvider<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        List<T> IBHOpenDataProvider<T>.GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        List<Dto> IBHOpenDataProvider<T>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            throw new NotImplementedException();
        }

        List<Dto> IBHOpenDataProvider<T>.GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class
        {
            throw new NotImplementedException();
        }

        T? IBHOpenDataProvider<T>.GetEntryWhere(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        T? IBHOpenDataProvider<T>.GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        Dto? IBHOpenDataProvider<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class
        {
            throw new NotImplementedException();
        }

        Dto? IBHOpenDataProvider<T>.GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class
        {
            throw new NotImplementedException();
        }

        List<T> IBHOpenDataProvider<T>.InsertAndReturnEntries(List<T> entries)
        {
            throw new NotImplementedException();
        }

        List<T> IBHOpenDataProvider<T>.InsertAndReturnEntries(List<T> entries, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        T? IBHOpenDataProvider<T>.InsertAndReturnEntry(T entry)
        {
            throw new NotImplementedException();
        }

        T? IBHOpenDataProvider<T>.InsertAndReturnEntry(T entry, BHTransaction transaction)
        {
            
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.JustInsertEntries(List<T> entries)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.JustInsertEntries(List<T> entries, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.JustInsertEntry(T entry)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.JustInsertEntry(T entry, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesById(List<T> entries)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesById(List<T> entries, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesById<Columns>(List<T> entries) where Columns : class
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntriesById<Columns>(List<T> entries, BHTransaction transaction) where Columns : class
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

        bool IBHOpenDataProvider<T>.UpdateEntryById(T entry)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntryById(T entry, BHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntryById<Columns>(T entry) where Columns : class
        {
            throw new NotImplementedException();
        }

        bool IBHOpenDataProvider<T>.UpdateEntryById<Columns>(T entry, BHTransaction transaction) where Columns : class
        {
            throw new NotImplementedException();
        }

        private T CheckGenerateValue(T entry)
        {
            foreach(AutoGeneratedProperty settings in _settings.AutoGeneratedColumns)
            {
                if (settings.Autogenerated && settings.Generator != null)
                {
                    entry.GetType().GetProperty(settings.PropertyName)?.SetValue(entry, GetGeneratorsValue(settings.Generator));
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
