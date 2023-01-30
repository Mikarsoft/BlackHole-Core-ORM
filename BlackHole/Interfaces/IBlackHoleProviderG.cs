using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Interfaces
{
    public interface IBlackHoleProviderG<T> where T : BlackHoleEntityG
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<T> GetAllEntries();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        IList<Dto> GetAllEntries<Dto>() where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<T> GetAllInactiveEntries();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        T? GetEntryById(Guid Id);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        Dto? GetEntryById<Dto>(Guid Id) where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<T> GetEntriesWhere(Expression<Func<T, bool>> predicate);

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
        Guid InsertEntry(T entry);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        List<Guid> InsertEntries(List<T> entries);

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
        Task<T?> GetEntryByIdAsync(Guid Id);

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
        Task<Guid> InsertEntryAsync(T entry);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        Task<List<Guid>> InsertEntriesAsync(List<T> entries);

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
        Task DeleteEntryById(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        Task DeleteInactiveEntryById(Guid id);

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
        Task<Guid> GetIdWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<List<Guid>> GetIdsWhereAsync(Expression<Func<T, bool>> predicate);

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

