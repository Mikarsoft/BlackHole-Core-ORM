

namespace BlackHole.Configuration
{
    public class ConnectionSettings
    {
        internal string ConnectionString { get; set; } = string.Empty;
        internal BHSqlType ConnectionType { get; set; }
        internal ConnectionAdditionalSettings additionalSettings = new ConnectionAdditionalSettings();

        public ConnectionAdditionalSettings UseSqlServer()
        {
            return additionalSettings;
        }

        public ConnectionAdditionalSettings UseNpgSql()
        {
            return additionalSettings;
        }
        public ConnectionAdditionalSettings UseMySql()
        {
            return additionalSettings;
        }
        public ConnectionAdditionalSettings UseSqlite()
        {
            return additionalSettings;
        }

        public ConnectionAdditionalSettings UseOracle()
        {
            return additionalSettings;
        }
    }
}
