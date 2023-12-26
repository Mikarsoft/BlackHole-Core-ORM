using BlackHole.Identifiers;

namespace BlackHole.PreLoads
{
    public class AuditSettingsBuilder
    {
        public BHAuditSettings Save()
        {
            return new();
        }
    }

    public class DefaultDataBuilder
    {
        
    }


    /// <summary>
    /// 
    /// </summary>
    public class StoredViewsBuilder
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
    public class StoredProceduresBuilder
    {

    }
}
