using BlackHole.Enums;

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class SingleMsBHConfig : BHModeConfig
    {
        internal SingleMsBHConfig() : base(BHMode.Single)
        {
        }
    }
}
