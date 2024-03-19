using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public interface IStoredViewsProcess<Dto> where Dto : BHDtoIdentifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreStoredView<Dto, TSource, TOther> InnerJoin<TSource, TOther>()
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreStoredView<Dto, TSource, TOther> OuterJoin<TSource, TOther>()
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreStoredView<Dto, TSource, TOther> LeftJoin<TSource, TOther>()
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreStoredView<Dto, TSource, TOther> RightJoin<TSource, TOther>()
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier;
    }
}
