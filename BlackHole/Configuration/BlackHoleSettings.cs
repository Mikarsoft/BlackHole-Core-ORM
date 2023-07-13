
namespace BlackHole.Configuration
{
    /// <summary>
    /// The full configuration object.
    /// It contains all the settings for this library.
    /// </summary>
    public class BlackHoleSettings
    {
        /// <summary>
        /// set the default directory of BlackHole to store logs and data
        /// </summary>
        public DataPathSettings directorySettings { get; set; } = new DataPathSettings();

        /// <summary>
        /// connection settings for the database
        /// </summary>
        public ConnectionSettings connectionConfig { get; set; } = new ConnectionSettings();

        /// <summary>
        /// put BlackHole into developer mode
        /// </summary>
        public bool isInDevMode { get; set; } = false;

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

        /// <summary>
        /// <para>If this is set to TRUE, then all the changes to the Entities 
        /// will be applied to the database.</para>
        /// <para>If this is set to FALSE, BlackHole will switch to production mode
        /// and it will not allow to drop any tables or columns from the database, 
        /// to protect the production database from data loss in case of developer's mistake </para>
        /// <para>The default value is FALSE</para>
        /// </summary>
        /// <param name="isDevMode"></param>
        /// <returns></returns>
        public BlackHoleSettings IsDeveloperMode(bool isDevMode)
        {
            isInDevMode = isDevMode;
            return this;
        }
    }
}
