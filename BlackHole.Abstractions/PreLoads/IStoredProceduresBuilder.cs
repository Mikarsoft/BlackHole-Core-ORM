

using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStoredProceduresBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        IStoredProcedureProcess<Dto> DeclareExisting<Dto>(string procedureName) where Dto : BHDtoIdentifier;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        IStoredProcedureProcess<Dto> CreateOrUpdate<Dto>(string procedureName, string commandText) where Dto : BHDtoIdentifier;
    }
}
