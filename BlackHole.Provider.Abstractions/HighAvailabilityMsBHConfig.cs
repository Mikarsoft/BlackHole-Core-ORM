namespace Mikarsoft.BlackHoleCore.Connector
{
    public class HighAvailabilityMsBHConfig : BHModeConfig
    {
        public HighAvailabilityMsBHConfig() : base(BHMode.HighAvailability)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setup"></param>
        /// <param name="multiSchemaConfig"></param>
        public void AddDatabases(Action<HighAvailabilitySetup> setup, Action<MultiSchemaBHConfig> multiSchemaConfig)
        {
            HighAvailabilitySetup haSetup = new();

            setup.Invoke(haSetup);

            DatabaseSetupConfigs.Add(haSetup);
        }
    }
}
