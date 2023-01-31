using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Interfaces
{
    /// <summary>
    /// Makes all the communication between the Datbase Table and The Specified Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBlackHoleProvider<T> where T : BlackHoleEntity
    {
        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns></returns>
        IList<T> GetAllEntries();

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns></returns>
        IList<Dto> GetAllEntries<Dto>() where Dto : BlackHoleDto;

        /// <summary>
        /// In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns></returns>
        IList<T> GetAllInactiveEntries();

        /// <summary>
        /// Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <returns></returns>
        T? GetEntryById(int Id);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <returns></returns>
        Dto? GetEntryById<Dto>(int Id) where Dto : BlackHoleDto;

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns></returns>
        T? GetEntryWhere(Expression<Func<T,bool>> predicate);

        /// <summary>
        /// Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns></returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto;

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<T> GetEntriesWhere(Expression<Func<T,bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        int InsertEntry(T entry);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        List<int> InsertEntries(List<T> entries);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> GetAllEntriesAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        Task<IList<Dto>> GetAllEntriesAsync<Dto>() where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> GetAllInactiveEntriesAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T?> GetEntryByIdAsync(int Id);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<Dto?> GetEntryByIdAsync<Dto>(int Id) where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IList<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IList<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        Task<int> InsertEntryAsync(T entry);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        Task<List<int>> InsertEntriesAsync(List<T> entries);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        Task UpdateEntryById(T entry);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Columns"></typeparam>
        /// <param name="entry"></param>
        Task UpdateEntryById<Columns>(T entry) where Columns : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        Task UpdateEntriesById(List<T> entries);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Columns"></typeparam>
        /// <param name="entries"></param>
        Task UpdateEntriesById<Columns>(List<T> entries) where Columns : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="entry"></param>
        Task UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Columns"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="entry"></param>
        Task UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class;

        /// <summary>
        /// 
        /// </summary>
        Task DeleteAllEntries();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        Task DeleteEntryById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        Task DeleteInactiveEntryById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        Task DeleteEntriesWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> GetIdWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<List<int>> GetIdsWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        JoinsData<Dto, T, TOther> InnerJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
           where Dto : BlackHoleDto where TOther : BlackHoleEntity where Tkey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        JoinsData<Dto, T, TOther> OuterJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
            where Dto : BlackHoleDto where TOther : BlackHoleEntity where Tkey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        JoinsData<Dto, T, TOther> LeftJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
            where Dto : BlackHoleDto where TOther : BlackHoleEntity where Tkey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        JoinsData<Dto, T, TOther> RightJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
           where Dto : BlackHoleDto where TOther : BlackHoleEntity where Tkey : IComparable;
    }
}
