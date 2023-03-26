
namespace BlackHole.Configuration
{
    public class BlackHoleSettings
    {
        public DataPathSettings directorySettings { get; set; } = new DataPathSettings();
        public ConnectionSettings connectionConfig { get; set; } = new ConnectionSettings();

        /// <summary>
        /// Add the configuration for a database.
        /// </summary>
        /// <param name="connectionSettings">connection settings</param>
        /// <returns>BlackHoleSettings to add more settings</returns>
        public DataPathSettings AddDatabase(Action<ConnectionSettings> connectionSettings)
        {
            connectionSettings.Invoke(connectionConfig);
            return directorySettings;
        }
    }
}
