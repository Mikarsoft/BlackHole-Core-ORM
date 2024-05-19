namespace BlackHole.Configuration.ConfigTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class BHRootSettings
    {
        internal BlackHoleBaseConfig _configuration { get; set; } = new();

        internal BlackHoleBaseConfig ExportConfiguration()
        {
            return _configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datapath"></param>
        /// <returns></returns>
        public BHRootSettings SetDatapath(string datapath)
        {
            _configuration.DataPath = datapath;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useLogger"></param>
        /// <param name="useLogsCleaner"></param>
        /// <returns></returns>
        public BHRootSettings UseLogger(bool useLogger, bool useLogsCleaner = true)
        {
            _configuration.UseLogger = useLogger;
            _configuration.UseLogsCleaner = useLogger ? useLogsCleaner : false;
            return this;
        }
    }
}
