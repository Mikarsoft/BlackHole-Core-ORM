using BlackHole.Identifiers;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public interface IJoinComplete<Dto> where Dto : BHDtoIdentifier
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
