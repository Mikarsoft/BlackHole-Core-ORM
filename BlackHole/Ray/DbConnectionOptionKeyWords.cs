

namespace BlackHole.Ray
{
    internal static class DbConnectionOptionKeywords
    {
        // Odbc
        internal const string Driver = "driver";
        internal const string Pwd = "pwd";
        internal const string UID = "uid";

        // OleDb
        internal const string DataProvider = "data provider";
        internal const string ExtendedProperties = "extended properties";
        internal const string FileName = "file name";
        internal const string Provider = "provider";
        internal const string RemoteProvider = "remote provider";

        // common keywords (OleDb, OracleClient, SqlClient)
        internal const string Password = "password";
        internal const string UserID = "user id";
    }

    internal static class DbConnectionStringKeywords
    {
        // all
        //        internal const string NamedConnection           = "Named Connection";

        // Odbc
        internal const string Driver = "Driver";
        internal const string Dsn = "Dsn";
        internal const string FileDsn = "FileDsn";
        internal const string SaveFile = "SaveFile";

        // OleDb
        internal const string FileName = "File Name";
        internal const string OleDbServices = "OLE DB Services";
        internal const string Provider = "Provider";

        // OracleClient
        internal const string Unicode = "Unicode";
        internal const string OmitOracleConnectionName = "Omit Oracle Connection Name";

        // SqlClient
        internal const string ApplicationIntent = "ApplicationIntent";
        internal const string ApplicationName = "Application Name";
        internal const string AsynchronousProcessing = "Asynchronous Processing";
        internal const string AttachDBFilename = "AttachDbFilename";
        internal const string ConnectTimeout = "Connect Timeout";
        internal const string ConnectionReset = "Connection Reset";
        internal const string ContextConnection = "Context Connection";
        internal const string CurrentLanguage = "Current Language";
        internal const string Encrypt = "Encrypt";
        internal const string FailoverPartner = "Failover Partner";
        internal const string InitialCatalog = "Initial Catalog";
        internal const string MultipleActiveResultSets = "MultipleActiveResultSets";
        internal const string MultiSubnetFailover = "MultiSubnetFailover";
        internal const string TransparentNetworkIPResolution = "TransparentNetworkIPResolution";
        internal const string NetworkLibrary = "Network Library";
        internal const string PacketSize = "Packet Size";
        internal const string Replication = "Replication";
        internal const string TransactionBinding = "Transaction Binding";
        internal const string TrustServerCertificate = "TrustServerCertificate";
        internal const string TypeSystemVersion = "Type System Version";
        internal const string UserInstance = "User Instance";
        internal const string WorkstationID = "Workstation ID";
        internal const string ConnectRetryCount = "ConnectRetryCount";
        internal const string ConnectRetryInterval = "ConnectRetryInterval";
        internal const string Authentication = "Authentication";
        internal const string Certificate = "Certificate";
        internal const string ColumnEncryptionSetting = "Column Encryption Setting";
        internal const string EnclaveAttestationUrl = "Enclave Attestation Url";
        internal const string PoolBlockingPeriod = "PoolBlockingPeriod";

        // common keywords (OleDb, OracleClient, SqlClient)
        internal const string DataSource = "Data Source";
        internal const string IntegratedSecurity = "Integrated Security";
        internal const string Password = "Password";
        internal const string PersistSecurityInfo = "Persist Security Info";
        internal const string UserID = "User ID";

        // managed pooling (OracleClient, SqlClient)
        internal const string Enlist = "Enlist";
        internal const string LoadBalanceTimeout = "Load Balance Timeout";
        internal const string MaxPoolSize = "Max Pool Size";
        internal const string Pooling = "Pooling";
        internal const string MinPoolSize = "Min Pool Size";
    }
}
