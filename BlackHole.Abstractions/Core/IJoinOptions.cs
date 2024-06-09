

using BlackHole.Abstractions.Core;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public interface IJoinOptions<Dto, TSource, TOther> : IBHQuery<Dto> where Dto : class, BHDtoIdentifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<Tkey, TOtherkey>(Expression<Func<TSource, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<Tkey, TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBHJoinsProcess<Dto> Then();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IJoinComplete<Dto> Finally();
    }
}
