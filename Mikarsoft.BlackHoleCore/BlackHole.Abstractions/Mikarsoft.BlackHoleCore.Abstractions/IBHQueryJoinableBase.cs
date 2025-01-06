
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;


namespace Mikarsoft.BlackHoleCore
{
    public interface IBHQueryJoinableBase<Dto, T> where T : BHEntity<T> where Dto : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderBy<T> OrderByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderBy<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <param name="keySelectors"></param>
        /// <returns></returns>
        IBHJoinsEnumerable<IBHGroup<G, T>, Dto, T> GroupBy<G>(Expression<Func<T, G>> keySelectors);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        IBHJoinsGroupedQuery<T, Dto> Map(Func<T, Dto> selector);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Dto> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Dto>> ToListAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Dto? FirstOrDefault();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<Dto?> FirstOrDefaultAsync();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHGroupedQueryJoinable<T, Dto> where T : BHEntity<T> where Dto : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderByQuery<T> OrderByAscending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderByQuery<T> OrderByDescending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Dto> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Dto>> ToListAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Dto"></typeparam>
    public interface IBHJoinsEnumerable<J, Dto, T> where T : BHEntity<T> where Dto : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        IBHJoinsGroupedQuery<T, Dto> Map(Func<J, Dto> selector);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHJoinsGroupedQuery<T, Dto> where T : BHEntity<T> where Dto : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHJoinsOrderByQuery<T, Dto> OrderByAscending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHJoinsOrderByQuery<T, Dto> OrderByDescending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Dto> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Dto>> ToListAsync();
    }

    public interface IBHJoinsOrderByQuery<T, Dto> where T : BHEntity<T> where Dto : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHJoinsOrderByQuery<T, Dto> ThenByAscending<TKey>(Expression<Func<T, TKey?>> action) where TKey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHJoinsOrderByQuery<T, Dto> ThenByDescending<TKey>(Expression<Func<T, TKey?>> action) where TKey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fetchRows"></param>
        IBHFinalQuery<Dto> Take(int fetchRows);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetRows"></param>
        /// <param name="fetchRows"></param>
        IBHFinalQuery<Dto> TakeWithOffset(int offsetRows, int fetchRows);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Dto> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Dto>> ToListAsync();
    }
}
