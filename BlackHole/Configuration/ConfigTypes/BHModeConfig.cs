
using BlackHole.Enums;

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class BHModeConfig
    {
        internal BHModeConfig(BHMode mode)
        {
            _bhMode = mode;
        }

        internal BHMode _bhMode {  get; set; }

        internal List<DatabaseSetupConfig> DatabaseSetupConfigs { get; set; } = new();

        internal MultiSchemaBHConfig? MultiSchemaConfiguration { get; set; }
    }
}
