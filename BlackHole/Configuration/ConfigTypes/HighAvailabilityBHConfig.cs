
namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class HighAvailabilityBHConfig : BHModeConfig
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
    }
}
