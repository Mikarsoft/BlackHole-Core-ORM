
namespace BlackHole.Configuration
{
    public class ConnectionAdditionalSettings
    {
        internal List<string> EntityNamespaces { get; set; } = new List<string>();
        internal List<string> ServicesNamespaces { get; set; } = new List<string>();

        public ConnectionAdditionalSettings UseEntityNamespace(string EntityNamespace)
        {
            return this;
        }

        public ConnectionAdditionalSettings UseEntityNamespaces(string connectionString, BHSqlType sqlType)
        {
            return this;
        }
    }
}
