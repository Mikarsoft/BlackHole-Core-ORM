
namespace BlackHole.Entities
{
    [System.Serializable]
    public class DatabaseConnection
    {
        public string SQLServer { get; set; } = "";
        public string DummyData { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public int Port { get; set; }
        public string DatabaseName { get; set; } = "";
        public int SQLType { get; set; }
        public string Key { get; set; } = "";
    }

    [System.Serializable]
    public class SupportMail
    {
        public string Host { get; set; } = "";
        public string Port { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string SiteName { get; set; } = "";
        public bool AdminMailPasswordRecover { get; set; }
        public string Key { get; set; } = "";
    }

    [System.Serializable]
    public class AdminCreds
    {
        public string Credentials { get; set; } = "";
        public string Key { get; set; } = "";
    }
}
