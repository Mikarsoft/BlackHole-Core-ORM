

using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public interface IJoinConfig<Dto, Tsource, TOther> where Dto : BHDtoIdentifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherkey"></param>
        /// <returns></returns>
        IJoinConfig<Dto, Tsource, TOther> And<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherkey"></param>
        /// <returns></returns>
        IJoinConfig<Dto, Tsource, TOther> Or<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IJoinOptions<Dto, Tsource, TOther> WhereFirst(Expression<Func<Tsource, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IJoinOptions<Dto, Tsource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IJoinOptions<Dto, Tsource, TOther> CastColumnOfFirst<Tkey, TOtherkey>(Expression<Func<Tsource, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        IJoinOptions<Dto, Tsource, TOther> CastColumnOfSecond<Tkey, TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto);

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
