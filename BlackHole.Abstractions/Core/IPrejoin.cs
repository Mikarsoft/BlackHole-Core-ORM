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
    public interface IPrejoin<Dto, TSource, TOther> where Dto : BHDtoIdentifier
    {
        /// <summary>
        /// Skata <para> <b>mpla</b></para>
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherkey"></param>
        /// <returns></returns>
        IJoinConfig<Dto, TSource, TOther> On<Tkey>(Expression<Func<TSource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey);
    }
}
