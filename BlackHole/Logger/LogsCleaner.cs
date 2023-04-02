

using BlackHole.Statics;

namespace BlackHole.Logger
{
    internal class LogsCleaner
    {
        private string LogsPath { get; set; } = string.Empty;

        LogsCleaner()
        {
            LogsPath = Path.Combine(DatabaseStatics.DataPath, "Logs");

            try
            {
                string pathFile = Path.Combine(LogsPath, $"TestingCleaner.txt");

                using (var tw = new StreamWriter(pathFile, true))
                {
                    tw.WriteLine($"Success");
                }

                File.Delete(pathFile);

                if (DatabaseStatics.UseLogsCleaner)
                {
                    StartCleanerThread();
                }
            }
            catch
            {
            }
        }

        private void StartCleanerThread()
        {

        }
    }
}
