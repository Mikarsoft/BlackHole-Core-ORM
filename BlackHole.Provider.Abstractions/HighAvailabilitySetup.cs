
namespace Mikarsoft.BlackHoleCore.Connector
{
    /// <summary>
    /// 
    /// </summary>
    public class HighAvailabilitySetup : DatabaseSetupConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MasterDbConnectionString"></param>
        /// <param name="StandbyDbConnectionString"></param>
        /// <param name="BackUpDbConnectionString"></param>
        /// <returns></returns>
        public UtilizationConfig ConnectionStrings(string MasterDbConnectionString, string StandbyDbConnectionString, string? BackUpDbConnectionString = null)
        {
            PrimaryConnectionString = MasterDbConnectionString;
            SecondaryConnectionString = StandbyDbConnectionString;
            ReserveConnectionString = BackUpDbConnectionString;
            ExtendedProtection = BackUpDbConnectionString != null;

            return UtilsConfig;
        }
    }
}
