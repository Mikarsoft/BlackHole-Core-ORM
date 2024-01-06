using BlackHole.Enums;

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
        public DataPathSettings DirectorySettings { get; set; } = new DataPathSettings();

        internal ConnectionSettings ConnectionConfig { get; set; } = new ConnectionSettings();

        internal List<MultiConnectionSettings> MultipleConnectionsConfig { get; set; } = new List<MultiConnectionSettings>();

        internal bool UseDefaultServicesOnMultiDb { get; set; } = true;

        internal MultiSchemaConnectionSettings MultiSchemaConfig { get; set; } = new();

        internal HighAvailabilityConnectionSettings HighAvailabilityConfig { get; set; } = new();

        internal bool IsInDevMode { get; set; } = false;

        internal bool AutoUpdate { get; set; } = false;

        internal BHMode DatabaseConfig { get; set; }

        /// <summary>
        /// Add the configuration for a database.
        /// </summary>
        /// <param name="connectionSettings">connection settings</param>
        /// <returns>DataPath Settings to add more settings</returns>
        public DataPathSettings AddDatabase(Action<ConnectionSettings> connectionSettings)
        {
            connectionSettings.Invoke(ConnectionConfig);
            DatabaseConfig = BHMode.Single;
            return DirectorySettings;
        }

        /// <summary>
        /// Add the configuration for a database.
        /// </summary>
        /// <param name="connectionSettings">connection settings</param>
        /// <returns>DataPath Settings to add more settings</returns>
        public DataPathSettings AddMultiSchemaDatabase(Action<MultiSchemaConnectionSettings> connectionSettings)
        {
            connectionSettings.Invoke(MultiSchemaConfig);
            DatabaseConfig = BHMode.MultiSchema;
            return DirectorySettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionSettings"></param>
        /// <returns></returns>
        public DataPathSettings AddHighAvailabilityScheme(Action<HighAvailabilityConnectionSettings> connectionSettings)
        {
            connectionSettings.Invoke(HighAvailabilityConfig);
            DatabaseConfig = BHMode.HighAvailability;
            return DirectorySettings;
        }

        /// <summary>
        /// Add the configuration for a database.
        /// </summary>
        /// <param name="connectionSettings">connection settings</param>
        /// <returns>DataPath Settings to add more settings</returns>
        public DataPathSettings AddMultipleDatabases(Action<List<Action<MultiConnectionSettings>>> connectionSettings)
        {
            DatabaseConfig = BHMode.Multiple;

            List<Action<MultiConnectionSettings>> list = new();
            connectionSettings.Invoke(list);

            foreach(Action<MultiConnectionSettings> settings in list)
            {
                MultiConnectionSettings dbSettings = new();
                settings.Invoke(dbSettings);

                if (dbSettings.additionalSettings.SkipDefaultProjectServices)
                {
                    UseDefaultServicesOnMultiDb = false;
                }

                MultipleConnectionsConfig.Add(dbSettings);
            }

            return DirectorySettings;
        }


        /// <summary>
        /// <para>If this is set to TRUE, then all the changes to the Entities 
        /// will be applied to the database.</para>
        /// <para>If this is set to FALSE, BlackHole will switch to production mode
        /// and it will not allow to drop any tables or columns from the database, 
        /// to protect the production database from data loss in case of developer's mistake </para>
        /// <para>The default value is FALSE</para>
        /// </summary>
        /// <param name="isDevMode">Developer's Mode</param>
        /// <returns>BlackHoleSettings to add more settings</returns>
        public BlackHoleSettings IsDeveloperMode(bool isDevMode)
        {
            IsInDevMode = isDevMode;
            return this;
        }

        /// <summary>
        /// Enables Automatic Database Update on the Start of the Application.
        /// <para><b>Important</b> => Changes on Existing Column's Nullability, on Composite Primary Keys and on Dropping 
        /// Columns can only be performed, if 'DeveloperMode' is Enabled, or by using the CLI 'update' command with the 
        /// '--force' argument => 'bhl update --force'</para>
        /// </summary>
        /// <returns>BlackHoleSettings to add more settings</returns>
        public BlackHoleSettings AutomaticUpdate()
        {
            AutoUpdate = true;
            return this;
        }
    }
}
