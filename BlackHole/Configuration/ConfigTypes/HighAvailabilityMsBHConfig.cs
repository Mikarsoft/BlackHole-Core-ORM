namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class HighAvailabilityMsBHConfig : BHModeConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MasterDbConnectionString"></param>
        /// <param name="StandbyDbConnectionString"></param>
        /// <param name="BackUpDbConnectionString"></param>
        public void AddDatabases(string MasterDbConnectionString, string StandbyDbConnectionString, string? BackUpDbConnectionString = null)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiSchemaConfig"></param>
        /// <param name="MasterDbConnectionString"></param>
        /// <param name="StandbyDbConnectionString"></param>
        /// <param name="BackUpDbConnectionString"></param>
        public void AddDatabases(Action<MultiSchemaBHConfig> multiSchemaConfig, string MasterDbConnectionString, string StandbyDbConnectionString, string? BackUpDbConnectionString = null)
        {

        }
    }
}
