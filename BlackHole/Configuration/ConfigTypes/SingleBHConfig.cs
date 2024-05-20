using BlackHole.Enums;

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class SingleBHConfig : BHModeConfig
    {
        internal SingleBHConfig() : base(BHMode.Single)
        {
        }
    }
}
