namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class HighAvailabilityMsBHConfig : BHModeConfig
    {
        internal UtilizationConfig? _bhItems {  get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MasterDbConnectionString"></param>
        /// <param name="StandbyDbConnectionString"></param>
        /// <param name="BackUpDbConnectionString"></param>
        public UtilizationConfig AddDatabases(string MasterDbConnectionString, string StandbyDbConnectionString, string? BackUpDbConnectionString = null)
        {
            _bhItems = new UtilizationConfig();
            return _bhItems;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSchemaConfig"></param>
        /// <param name="MasterDbConnectionString"></param>
        /// <param name="StandbyDbConnectionString"></param>
        /// <param name="BackUpDbConnectionString"></param>
        public UtilizationConfig AddDatabases(Action<MultiSchemaBHConfig> multiSchemaConfig, string MasterDbConnectionString, string StandbyDbConnectionString, string? BackUpDbConnectionString = null)
        {
            _bhItems = new UtilizationConfig();
            return _bhItems;
        }
    }
}
