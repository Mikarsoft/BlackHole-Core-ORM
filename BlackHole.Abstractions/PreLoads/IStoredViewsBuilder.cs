using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStoredViewsBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        IStoredViewsProcess<Dto> StartJoinUsing<Dto>() where Dto : BHDtoIdentifier;
    }
}
