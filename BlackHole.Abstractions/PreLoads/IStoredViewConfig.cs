
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public interface IStoredViewConfig<Dto, TSource, TOther>
         where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier where Dto : BHDtoIdentifier
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IStoredViewConfig<Dto, TSource, TOther> And<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IStoredViewConfig<Dto, TSource, TOther> Or<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IStoredViewOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IStoredViewOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TOtherKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IStoredViewOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TOtherKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IStoredViewOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IStoredViewsProcess<Dto> Then();

        /// <summary>
        /// 
        /// </summary>
        void StoreAsView();
    }
}
