﻿namespace Mikarsoft.BlackHoleCore.Connector
{
    public class DatabaseSetupConfig
    {
        internal UtilizationConfig UtilsConfig { get; set; } = new();

        internal string PrimaryConnectionString { get; set; } = string.Empty;

        internal string SecondaryConnectionString { get; set; } = string.Empty;

        internal string? ReserveConnectionString { get; set; }

        internal bool ExtendedProtection { get; set; }
    }
}
