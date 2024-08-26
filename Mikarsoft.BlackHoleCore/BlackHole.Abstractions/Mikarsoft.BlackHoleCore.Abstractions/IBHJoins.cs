using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHJoinsProcess<Dto> where Dto : BHDto
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreJoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>() where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreJoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>() where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreJoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>() where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreJoin<Dto, TSource, TOther> RightJoin<TSource, TOther>() where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public interface IPreJoin<Dto, TSource, TOther> where Dto : BHDto where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IJoinConfig<Dto, TSource, TOther> On<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public interface IJoinConfig<Dto, TSource, TOther> : IBHQuery<Dto> where Dto : BHDto where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IJoinConfig<Dto, TSource, TOther> And<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IJoinConfig<Dto, TSource, TOther> Or<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);

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
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TOtherKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TOtherKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto);

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

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public interface IJoinOptions<Dto, TSource, TOther> : IBHQuery<Dto> where Dto : BHDto
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
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TOtherKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TOtherKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto);

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

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public interface IJoinComplete<Dto> where Dto : BHDto
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Dto> ExecuteQuery();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Dto>> ExecuteQueryAsync();
    }
}
