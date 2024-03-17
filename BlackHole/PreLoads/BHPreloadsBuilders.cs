using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{

    /// <summary>
    /// 
    /// </summary>
    public class DefaultDataBuilder : IDefaultDataBuilder
    {
        
    }


    /// <summary>
    /// 
    /// </summary>
    public class StoredViewsBuilder : IStoredViewsBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        public StoredViewsProcess<Dto> StartJoinUsing<Dto>() where Dto : BHDtoIdentifier
        {
            return new StoredViewsProcess<Dto>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StoredProceduresBuilder : IStoredProceduresBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public StoredProcedureProcess<Dto> DeclareExisting<Dto>(string procedureName) where Dto : BHDtoIdentifier
        {
            return new StoredProcedureProcess<Dto>(procedureName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public StoredProcedureProcess<Dto> CreateOrUpdate<Dto>(string procedureName, string commandText) where Dto : BHDtoIdentifier
        {
            return new StoredProcedureProcess<Dto>(procedureName, commandText);
        }
    }
}
