using BlackHole.Statics;
using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Logger
{
    internal class LoggerService : ILoggerService
    {
        bool canWriteLogs { get; set; } = false;
        string LogsPath { get; set; }

        internal LoggerService()
        {
            try
            {
                if (DatabaseStatics.UseLogging)
                {
                    LogsPath = Path.Combine(DatabaseStatics.DataPath, "Logs");

                    if (!Directory.Exists(LogsPath))
                    {
                        Directory.CreateDirectory(LogsPath);
                    }

                    canWriteLogs = true;
                }
                else
                {
                    LogsPath = string.Empty;
                    canWriteLogs = false;
                }
            }
            catch
            {
                DatabaseStatics.DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlackHoleData");
                LogsPath = Path.Combine(DatabaseStatics.DataPath, "Logs");
                canWriteLogs = false;
            }
        }

        private string LogHashId(string text)
        {
            var sh = SHA1.Create();
            var hash = new StringBuilder();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] b = sh.ComputeHash(bytes);

            foreach (byte a in b)
            {
                var h = a.ToString("x2");
                hash.Append(h);
            }

            return hash.ToString();
        }

        public void CreateErrorLogs(string Area,string commandText, string Message, string Details)
        {
            if (canWriteLogs)
            {
                try
                {
                    string LogId = LogHashId(Guid.NewGuid().ToString() + DateTime.Now.ToString());
                    string pathFile = Path.Combine(LogsPath,$"{Area}_{LogId}.txt");

                    using (var tw = new StreamWriter(pathFile, true))
                    {
                        tw.WriteLine($"Date and Time: {DateTime.Now.ToString("s").Replace(":", ".")}");
                        tw.WriteLine($"Command : {commandText}");
                        tw.WriteLine($"Error: {Message}");
                        tw.WriteLine($"Details: {Details}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}
