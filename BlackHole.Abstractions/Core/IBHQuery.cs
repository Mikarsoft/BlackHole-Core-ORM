using System.Linq.Expressions;

namespace BlackHole.Abstractions.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHQuery<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHQuery<T> OrderByAscending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHQuery<T> OrderByDescending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHGroup<T,G> GroupBy<G>(Expression<Func<T, G?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<T> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ToListAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHGroup<T, G>
    {

    }
}
