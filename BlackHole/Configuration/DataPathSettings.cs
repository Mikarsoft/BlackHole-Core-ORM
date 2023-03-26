
namespace BlackHole.Configuration
{
    public class DataPathSettings
    {
        internal string DataPath { get; set; } = string.Empty;

        /// <summary>
        /// Set the path of the folder where BlackHole will store
        /// Sqlite databases, Logs and other data that will
        /// be required for the features in later updates
        /// </summary>
        /// <param name="DataPath">Full path of the data folder</param>
        public void SetDataPath(string dataPath)
        {
            DataPath = DataPath;
        }
    }
}
