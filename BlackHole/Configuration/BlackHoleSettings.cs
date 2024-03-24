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

        internal bool ValidateSettings(string? callingAssemblyName)
        {
            return DatabaseConfig switch
            {
                BHMode.Single => ValidateSingle(callingAssemblyName),
                BHMode.HighAvailability => ValidateHighAvailability(callingAssemblyName),
                BHMode.Multiple => ValidateMultiple(callingAssemblyName),
                BHMode.MultiSchema => ValidateMultiSchema(callingAssemblyName),
                _ => false
            };
        }

        private bool ValidateSingle(string? callingAssemblyName)
        {
            if (string.IsNullOrWhiteSpace(ConnectionConfig.ConnectionString))
            {
                ValidationErrors = "The connection string is missing or is empty";
                return false;
            }

            List<string?> assemblyNames = ConnectionConfig.additionalSettings.AssembliesToUse.Select(x => x.FullName).ToList();

            if(ConnectionConfig.additionalSettings.EntityNamespaces != null)
            {
                if (ConnectionConfig.additionalSettings.EntityNamespaces.EntitiesNamespaces.Count != 
                    ConnectionConfig.additionalSettings.EntityNamespaces.EntitiesNamespaces.Distinct().Count())
                {
                    ValidationErrors = "Duplicate Entities namespace found in the configuration";
                    return false;
                }

                if (ConnectionConfig.additionalSettings.EntityNamespaces.AssemblyToUse != null)
                {
                    if (!string.IsNullOrEmpty(callingAssemblyName)
                        && callingAssemblyName == ConnectionConfig.additionalSettings.EntityNamespaces.AssemblyToUse.ScanAssembly?.FullName)
                    {
                        ValidationErrors = "The additional Assembly for the Entities and Services registration is the same as the main project Assembly";
                        return false;
                    }
                }
            }

            if (ConnectionConfig.additionalSettings.ServicesNamespaces != null)
            {
                if (ConnectionConfig.additionalSettings.ServicesNamespaces.ServicesNamespaces.Count !=
                    ConnectionConfig.additionalSettings.ServicesNamespaces.ServicesNamespaces.Distinct().Count())
                {
                    ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                    return false;
                }

                if(ConnectionConfig.additionalSettings.ServicesNamespaces.AssemblyToUse != null)
                {
                    if (!string.IsNullOrEmpty(callingAssemblyName) 
                        && callingAssemblyName == ConnectionConfig.additionalSettings.ServicesNamespaces.AssemblyToUse.ScanAssembly?.FullName)
                    {
                        ValidationErrors = "The additional Assembly for the Entities and Services registration is the same as the calling Assembly";
                        return false;
                    }
                }
            }

            if (assemblyNames.Any(x => x == null))
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if (assemblyNames.Count != assemblyNames.Distinct().Count())
            {
                ValidationErrors = "Duplicate Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if (ConnectionConfig.additionalSettings.useCallingAssembly)
            {
                if(!string.IsNullOrEmpty(callingAssemblyName) && assemblyNames.Contains(callingAssemblyName))
                {
                    ValidationErrors = "Duplicate Assembly, as the main project Assembly, reference found in a configuration of a database";
                    return false;
                }
            }

            return true;
        }

        private bool ValidateHighAvailability(string? callingAssemblyName)
        {
            if (string.IsNullOrWhiteSpace(HighAvailabilityConfig.ConnectionString))
            {
                ValidationErrors = "The connection string of the main database is missing or is empty";
                return false;
            }

            if (string.IsNullOrWhiteSpace(HighAvailabilityConfig.DatabaseIdentity))
            {
                ValidationErrors = "The database identity of the main database is missing or is empty";
                return false;
            }

            if (!HighAvailabilityConfig.secondDbConfig.BackUpIsSelected)
            {
                ValidationErrors = "There is no setup for the secondary database in the High Availability configuration";
                return false;
            }

            if (string.IsNullOrWhiteSpace(HighAvailabilityConfig.secondDbConfig.ConnectionString))
            {
                ValidationErrors = "The connection string of the secondary database is missing or is empty";
                return false;
            }

            if (string.IsNullOrWhiteSpace(HighAvailabilityConfig.secondDbConfig.DatabaseIdentity))
            {
                ValidationErrors = "The database identity of the secondary database is missing or is empty";
                return false;
            }

            if(HighAvailabilityConfig.ConnectionString.Replace(" ","") == HighAvailabilityConfig.secondDbConfig.ConnectionString.Replace(" ",""))
            {
                ValidationErrors = "Both main and secondary databases have the exact same connection string";
                return false;
            }

            if (HighAvailabilityConfig.DatabaseIdentity.Replace(" ", "") == HighAvailabilityConfig.secondDbConfig.DatabaseIdentity.Replace(" ", ""))
            {
                ValidationErrors = "Both main and secondary databases have the exact same identity";
                return false;
            }

            List<string?> assemblyNames = HighAvailabilityConfig.secondDbConfig.additionalConfig.AssembliesToUse.Select(x => x.FullName).ToList();

            if (assemblyNames.Any(x => x == null))
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if (assemblyNames.Count != assemblyNames.Distinct().Count())
            {
                ValidationErrors = "Duplicate Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }


            if (HighAvailabilityConfig.secondDbConfig.additionalConfig.EntityNamespaces != null)
            {
                if (HighAvailabilityConfig.secondDbConfig.additionalConfig.EntityNamespaces.EntitiesNamespaces.Count !=
                     HighAvailabilityConfig.secondDbConfig.additionalConfig.EntityNamespaces.EntitiesNamespaces.Distinct().Count())
                {
                    ValidationErrors = "Duplicate Entities namespace found in the configuration";
                    return false;
                }

                if (HighAvailabilityConfig.secondDbConfig.additionalConfig.EntityNamespaces.AssemblyToUse != null)
                {
                    if (!string.IsNullOrEmpty(callingAssemblyName)
                        && callingAssemblyName == HighAvailabilityConfig.secondDbConfig.additionalConfig.EntityNamespaces.AssemblyToUse?.ScanAssembly?.FullName)
                    {
                        ValidationErrors = "The additional Assembly for the Entities and Services registration is the same as the main project Assembly";
                        return false;
                    }
                }
            }

            if (HighAvailabilityConfig.secondDbConfig.additionalConfig.ServicesNamespaces != null)
            {
                if (HighAvailabilityConfig.secondDbConfig.additionalConfig.ServicesNamespaces.ServicesNamespaces.Count !=
                    HighAvailabilityConfig.secondDbConfig.additionalConfig.ServicesNamespaces.ServicesNamespaces.Distinct().Count())
                {
                    ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                    return false;
                }

                if (HighAvailabilityConfig.secondDbConfig.additionalConfig.ServicesNamespaces.AssemblyToUse != null)
                {
                    if (!string.IsNullOrEmpty(callingAssemblyName)
                        && callingAssemblyName == HighAvailabilityConfig.secondDbConfig.additionalConfig.ServicesNamespaces.AssemblyToUse?.ScanAssembly?.FullName)
                    {
                        ValidationErrors = "The additional Assembly for the Entities and Services registration is the same as the calling Assembly";
                        return false;
                    }
                }
            }

            if (HighAvailabilityConfig.secondDbConfig.additionalConfig.useCallingAssembly)
            {
                if (!string.IsNullOrEmpty(callingAssemblyName) && assemblyNames.Contains(callingAssemblyName))
                {
                    ValidationErrors = "Duplicate Assembly, as the main project Assembly, reference found in a configuration of a database";
                    return false;
                }
            }

            return true;
        }

        private bool ValidateMultiple(string? callingAssemblyName)
        {
            if(MultipleConnectionsConfig.Count == 0)
            {
                ValidationErrors = "No Configurations found for the multiple database setup";
                return false;
            }

            List<string?> assemblyNames = MultipleConnectionsConfig.Where(x => x.IsUsingAssembly).Select(x => x.Ass?.FullName).ToList();

            if(!string.IsNullOrWhiteSpace(callingAssemblyName) && assemblyNames.Contains(callingAssemblyName))
            {
                ValidationErrors = "Duplicate Assembly, as the main project Assembly, reference found in a configuration of a database";
                return false;
            }

            if(assemblyNames.Any(x => x == null))
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if(assemblyNames.Count != assemblyNames.Distinct().Count())
            {
                ValidationErrors = "Duplicate Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            List<string> nspNames = MultipleConnectionsConfig.Where(x => !x.IsUsingAssembly).Select(x => x.SelectedNamespace.Replace(" ","")).ToList();

            if (nspNames.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                ValidationErrors = "Null or Empty Namespace reference found in the multiple databases configuration";
                return false;
            }

            if (nspNames.Count != nspNames.Distinct().Count())
            {
                ValidationErrors = "Duplicate Namespace reference found in the multiple databases configuration";
                return false;
            }

            List<string> connectionStrings = MultipleConnectionsConfig.Select(x => x.ConnectionString.Replace(" ","")).ToList();

            if (connectionStrings.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                ValidationErrors = "A Connection string is missing in the multiple databases configuration";
                return false;
            }

            List<string> dbIdentities = MultipleConnectionsConfig.Select(x => x.DatabaseIdentity.Replace(" ", "")).ToList();

            if (dbIdentities.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                ValidationErrors = "A database Identity is missing in the multiple databases configuration";
                return false;
            }

            if (connectionStrings.Count != connectionStrings.Distinct().Count())
            {
                ValidationErrors = "Some connection strings are duplicate in the multiple databases configuration";
                return false;
            }

            List<string> serviceNamespaces = new();
            List<string?> serviceAssemblies = new();

            bool hasApplied;

            foreach(MultiConnectionSettings settings in MultipleConnectionsConfig)
            {
                hasApplied = false;

                if (settings.additionalSettings.RegisterServices)
                {
                    if (settings.additionalSettings.SamePathServices)
                    {
                        if (settings.IsUsingAssembly)
                        {
                            serviceAssemblies.Add(settings.Ass?.FullName);
                        }
                        else
                        {
                            serviceNamespaces.Add(settings.SelectedNamespace);
                        }

                        hasApplied = true;
                    }

                    if (settings.additionalSettings.ServicesInOtherAssembly)
                    {
                        if(settings.additionalSettings.ServicesAssembly != null)
                        {
                            serviceAssemblies.Add(settings.additionalSettings.ServicesAssembly.FullName);
                        }
                        else
                        {
                            ValidationErrors = "Null Assembly reference found in the services configuration of a database, that was configured to use an Assembly";
                            return false;
                        }

                        hasApplied = true;
                    }

                    if (!string.IsNullOrWhiteSpace(settings.additionalSettings.ServicesNamespace))
                    {
                        serviceNamespaces.Add(settings.additionalSettings.ServicesNamespace);
                    }
                    else
                    {
                        if (!hasApplied)
                        {
                            ValidationErrors = "A Namespace to register services in a database is empty, in the multiple databases configuration";
                            return false;
                        }
                    }
                }
            }

            if (serviceNamespaces.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                ValidationErrors = "A Namespace to register services in a database is empty, in the multiple databases configuration";
                return false;
            }

            if (serviceNamespaces.Count != serviceNamespaces.Distinct().Count())
            {
                ValidationErrors = "Duplicate Namespace to register services found, in the multiple databases configuration";
                return false;
            }

            if (serviceAssemblies.Any(x => x == null))
            {
                ValidationErrors = "Null Assembly reference found in the services configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if (serviceAssemblies.Count != serviceAssemblies.Distinct().Count())
            {
                ValidationErrors = "Duplicate Assembly reference found in the services configuration of a database, that was configured to use an Assembly";
                return false;
            }

            return true;
        }

        private bool ValidateMultiSchema(string? callingAssemblyName)
        {
            if (string.IsNullOrWhiteSpace(MultiSchemaConfig.ConnectionString))
            {
                ValidationErrors = "The connection string of the main database is missing or is empty";
                return false;
            }

            if (!MultiSchemaConfig.additionalSettings.SchemaSeparationSelected)
            {
                ValidationErrors = "No Schema separation selected for this configuration";
                return false;
            }

            List<string?> assemblyNames = MultiSchemaConfig.additionalSettings.AssembliesToUse.Select(x => x.FullName).ToList();

            if (MultiSchemaConfig.additionalSettings.IncludeCallingAssembly)
            {
                assemblyNames.Add(callingAssemblyName);
            }

            if (assemblyNames.Any(x => x == null))
            {
                ValidationErrors = "Null Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if (assemblyNames.Count != assemblyNames.Distinct().Count())
            {
                ValidationErrors = "Duplicate Assembly reference found in a configuration of a database, that was configured to use an Assembly";
                return false;
            }

            if (!MultiSchemaConfig.additionalSettings.ServicesSettings.UseDefaultProjectServices)
            {
                if(MultiSchemaConfig.additionalSettings.ServicesSettings.AssemblyToUse != null)
                {
                    string? servicesAss = MultiSchemaConfig.additionalSettings.ServicesSettings.AssemblyToUse.FullName;

                    if (!string.IsNullOrEmpty(callingAssemblyName)
                        && callingAssemblyName == servicesAss)
                    {
                        ValidationErrors = "The additional Assembly for the Services registration is the same as the calling Assembly";
                        return false;
                    }

                    if(!string.IsNullOrEmpty(servicesAss) && assemblyNames.Contains(servicesAss))
                    {
                        ValidationErrors = "The additional Assembly for the Services registration is already declared in one of the Entities assemblies";
                        return false;
                    }
                }

                if(MultiSchemaConfig.additionalSettings.ServicesSettings.ServicesNamespaces.Count > 0)
                {
                    List<string> nspNames = MultiSchemaConfig.additionalSettings.ServicesSettings.ServicesNamespaces;

                    if (nspNames.Any(x => string.IsNullOrWhiteSpace(x)))
                    {
                        ValidationErrors = "Null or Empty Namespace reference found in the multi schema database configuration";
                        return false;
                    }

                    if (nspNames.Count != nspNames.Distinct().Count())
                    {
                        ValidationErrors = "Duplicate Namespace reference found in the multi schema database configuration";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
