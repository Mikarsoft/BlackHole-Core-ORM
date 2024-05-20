
using BlackHole.Enums;

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class HighAvailabilityBHConfig : BHModeConfig
    {
        internal HighAvailabilityBHConfig() : base(BHMode.HighAvailability)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setup"></param>
        public void AddDatabases(Action<HighAvailabilitySetup> setup)
        {
            HighAvailabilitySetup haSetup = new();

            setup.Invoke(haSetup);

            DatabaseSetupConfigs.Add(haSetup);
        }
    }
}
