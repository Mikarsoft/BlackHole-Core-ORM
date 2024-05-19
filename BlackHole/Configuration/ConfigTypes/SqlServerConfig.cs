using BlackHole.Enums;

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlServerConfig : BHDatabaseConfig
    {
        internal SqlServerConfig(bool useQuotedDb, int connectionTimeout) : base(useQuotedDb, connectionTimeout, BlackHoleSqlTypes.SqlServer)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useAutomaticUpdate"></param>
        /// <param name="useDevMode"></param>
        /// <returns></returns>
        public SingleMsBHConfig Single(bool useAutomaticUpdate = true, bool useDevMode = false)
        {
            UseAutomaticUpdate = useAutomaticUpdate;
            UseDevMode = useDevMode;
            _bhBase = new SingleMsBHConfig();
            return (SingleMsBHConfig)_bhBase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useAutomaticUpdate"></param>
        /// <param name="useDevMode"></param>
        /// <returns></returns>
        public HighAvailabilityMsBHConfig HighAvailability(bool useAutomaticUpdate = true, bool useDevMode = false)
        {
            UseAutomaticUpdate = useAutomaticUpdate;
            UseDevMode = useDevMode;
            _bhBase = new HighAvailabilityMsBHConfig();
            return (HighAvailabilityMsBHConfig)_bhBase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useAutomaticUpdate"></param>
        /// <param name="useDevMode"></param>
        /// <returns></returns>
        public MultipleMsBHConfig Multiple(bool useAutomaticUpdate = true, bool useDevMode = false)
        {
            UseAutomaticUpdate = useAutomaticUpdate;
            UseDevMode = useDevMode;
            _bhBase = new MultipleMsBHConfig();
            return (MultipleMsBHConfig)_bhBase;
        }
    }
}
