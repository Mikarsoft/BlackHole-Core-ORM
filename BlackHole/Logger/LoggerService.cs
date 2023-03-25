using BlackHole.Statics;

namespace BlackHole.Logger
{
    internal class LoggerService : ILoggerService
    {
        bool canWriteLogs { get; set; }
        string LogsPath { get; set; }
        string dateChar { get; set; }

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

            dateChar = "D-";
        }

        public void CreateErrorLogs(string Area, string Message, string Details)
        {
            if (canWriteLogs)
            {
                try
                {
                    string Path = $"{LogsPath}\\{Area}_Error_{dateChar}{DateTime.Now.ToString("s").Replace(":", ".")}.txt";
                    using (var tw = new StreamWriter(Path, true))
                    {
                        tw.WriteLine(Message);
                        tw.WriteLine(Details);
                    }

                    string[] files = Directory.GetFiles(LogsPath, "*.txt", SearchOption.TopDirectoryOnly);

                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.CreationTime < DateTime.Now.AddDays(-60))
                        {
                            fi.Delete();
                        }
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
