
namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHJoinsProcess<Dto>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPrejoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPrejoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPrejoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPrejoin<Dto, TSource, TOther> RightJoin<TSource, TOther>();
    }
}
