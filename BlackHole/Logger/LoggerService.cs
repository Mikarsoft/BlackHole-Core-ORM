using BlackHole.Statics;
using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Logger
{
    internal class LoggerService : ILoggerService
    {
        bool canWriteLogs { get; set; }
        string LogsPath { get; set; }

        internal LoggerService()
        {
            try
            {
                LogsPath = Path.Combine(DatabaseStatics.DataPath, "Logs");

                if (!Directory.Exists(LogsPath))
                {
                    Directory.CreateDirectory(LogsPath);
                }

                canWriteLogs = true;
            }
            catch
            {
                DatabaseStatics.DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlackHoleData");
                LogsPath = Path.Combine(DatabaseStatics.DataPath, "Logs");
                canWriteLogs = false;
            }
        }

        public string LogHashId(string text)
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

        public void CreateErrorLogs(string Area, string Message, string Details)
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
                        tw.WriteLine($"Error: {Message}");
                        tw.WriteLine($"Details: {Details}");
                    }

                    //string[] files = Directory.GetFiles(LogsPath, "*.txt", SearchOption.TopDirectoryOnly);

                    //foreach (string file in files)
                    //{
                    //    FileInfo fi = new FileInfo(file);
                    //    if (fi.CreationTime < DateTime.Now.AddDays(-60))
                    //    {
                    //        fi.Delete();
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}
