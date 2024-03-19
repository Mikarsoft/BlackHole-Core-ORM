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
    public interface IPreStoredView<Dto, TSource, TOther>
        where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier where Dto : BHDtoIdentifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IStoredViewConfig<Dto, TSource, TOther> On<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);
    }
}
