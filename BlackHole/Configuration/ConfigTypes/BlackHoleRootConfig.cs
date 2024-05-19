

namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class BlackHoleRootConfig
    {
        internal BlackHoleBaseConfig? _blackHoleConfig {  get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BlackHoleBaseConfig UseDefaults()
        {
            _blackHoleConfig = new BlackHoleBaseConfig();
            return _blackHoleConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootSettings"></param>
        /// <returns></returns>
        public BlackHoleBaseConfig Configure(Action<BHRootSettings> rootSettings)
        {
            BHRootSettings settings = new();
            rootSettings.Invoke(settings);

            _blackHoleConfig = settings.ExportConfiguration();

            return _blackHoleConfig;
        }
    }
}
