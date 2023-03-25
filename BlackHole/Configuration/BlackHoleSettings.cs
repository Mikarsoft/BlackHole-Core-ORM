
namespace BlackHole.Configuration
{
    public class BlackHoleSettings
    {
        public string dataPath { get; set; } = string.Empty;
        public ConnectionSettings connectionConfig { get; set; } = new ConnectionSettings();

        /// <summary>
        /// Add the configuration for a database.
        /// </summary>
        /// <param name="connectionSettings">connection settings</param>
        /// <returns>BlackHoleSettings to add more settings</returns>
        public BlackHoleSettings AddDatabase(Action<ConnectionSettings> connectionSettings)
        {
            connectionSettings.Invoke(connectionConfig);
            return this;
        }

        /// <summary>
        /// Set the path of the folder where BlackHole will store
        /// Sqlite databases, Logs and other data that will
        /// be required for the features in later updates
        /// </summary>
        /// <param name="DataPath">Full path of the data folder</param>
        public void SetDataPath(string DataPath)
        {
            dataPath = DataPath;
        }
    }
}
