using BlackHole.Enums;

namespace BlackHole.Configuration
{
    /// <summary>
    /// The full configuration object.
    /// It contains all the settings for this library.
    /// </summary>
    public class BlackHoleSettings
    {
        internal DataPathSettings DirectorySettings = new DataPathSettings();

        internal ConnectionSettings ConnectionConfig = new ConnectionSettings();

        internal List<MultiConnectionSettings> MultipleConnectionsConfig = new List<MultiConnectionSettings>();

        internal MultiSchemaConnectionSettings MultiSchemaConfig = new();

        internal HighAvailabilityConnectionSettings HighAvailabilityConfig = new();

        internal bool UseDefaultServicesOnMultiDb { get; set; } = true;
        internal bool IsInDevMode { get; set; } = false;
        internal bool AutoUpdate { get; set; } = false;
        internal BHMode DatabaseConfig { get; set; }

        internal string ValidationErrors { get; set; } = string.Empty;

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

        internal bool ValidateSettings()
        {
            return DatabaseConfig switch
            {
                BHMode.Single => ValidateSingle(),
                BHMode.HighAvailability => ValidateHighAvailability(),
                BHMode.Multiple => ValidateMultiple(),
                BHMode.MultiSchema => ValidateMultiSchema(),
                _ => false
            };
        }

        private bool ValidateSingle()
        {
            return false;
        }

        private bool ValidateHighAvailability()
        {
            return false;
        }

        private bool ValidateMultiple()
        {
            if(MultipleConnectionsConfig.Count == 0)
            {
                return false;
            }

            List<string?> assemblyNames = MultipleConnectionsConfig.Where(x => x.IsUsingAssembly).Select(x => x.Ass?.FullName).ToList();

            if(assemblyNames.Any(x => x == null))
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if(assemblyNames.Count != assemblyNames.Distinct().Count())
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            List<string> nspNames = MultipleConnectionsConfig.Where(x => !x.IsUsingAssembly).Select(x => x.SelectedNamespace).ToList();

            if (nspNames.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if (nspNames.Count != nspNames.Distinct().Count())
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            List<string> connectionStrings = MultipleConnectionsConfig.Select(x => x.ConnectionString).ToList();

            if (connectionStrings.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if (connectionStrings.Count != connectionStrings.Distinct().Count())
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            return true;
        }

        private bool ValidateMultiSchema()
        {
            return false;
        }
    }
}
