
namespace BlackHole.Configuration
{
    /// <summary>
    /// Simple Configuration that is using all Black Hole Entities
    /// in the Calling Assembly and automatically registers all
    /// Black Hole Services and Interfaces
    /// </summary>
    public class BHConfig
    {
        internal List<ConnectionSettings> connectionSettings { get; set; } = new List<ConnectionSettings>();
        internal List<string> ServicesNamespaces { get; set; } = new List<string>();


        public ConnectionSettings AddConnection(string connectionString, BHSqlType sqlType)
        {
            ConnectionSettings newConnection = new ConnectionSettings { ConnectionType = sqlType, ConnectionString = connectionString };
            connectionSettings.Add(newConnection);
            return newConnection;
        }
    }
}
