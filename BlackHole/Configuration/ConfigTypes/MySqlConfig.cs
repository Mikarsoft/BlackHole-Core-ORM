using BlackHole.Enums;

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlConfig : BHDatabaseConfig
    {
        internal MySqlConfig(bool useQuotedDb, int connectionTimeout) : base(useQuotedDb, connectionTimeout, BlackHoleSqlTypes.MySql)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useAutomaticUpdate"></param>
        /// <param name="useDevMode"></param>
        /// <returns></returns>
        public SingleBHConfig Single(bool useAutomaticUpdate = true, bool useDevMode = false)
        {
            UseAutomaticUpdate = useAutomaticUpdate;
            UseDevMode = useDevMode;
            _bhBase = new SingleMsBHConfig();
            return (SingleBHConfig)_bhBase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useAutomaticUpdate"></param>
        /// <param name="useDevMode"></param>
        /// <returns></returns>
        public HighAvailabilityBHConfig HighAvailability(bool useAutomaticUpdate = true, bool useDevMode = false)
        {
            UseAutomaticUpdate = useAutomaticUpdate;
            UseDevMode = useDevMode;
            _bhBase = new HighAvailabilityBHConfig();
            return (HighAvailabilityBHConfig)_bhBase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useAutomaticUpdate"></param>
        /// <param name="useDevMode"></param>
        /// <returns></returns>
        public MultipleBHConfig Multiple(bool useAutomaticUpdate = true, bool useDevMode = false)
        {
            UseAutomaticUpdate = useAutomaticUpdate;
            UseDevMode = useDevMode;
            _bhBase = new MultipleBHConfig();
            return (MultipleBHConfig)_bhBase;
        }
    }
}
