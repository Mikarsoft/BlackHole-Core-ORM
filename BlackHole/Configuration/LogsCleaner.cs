using BlackHole.Statics;

namespace BlackHole.Configuration
{
    internal class LogsCleaner
    {
        private string LogsPath { get; set; } = string.Empty;

        internal LogsCleaner()
        {
            LogsPath = Path.Combine(BHStaticSettings.DataPath, "Logs");

            try
            {
                string pathFile = Path.Combine(LogsPath, $"TestingCleaner.txt");

                using (var tw = new StreamWriter(pathFile, true))
                {
                    tw.WriteLine($"Success");
                }

                File.Delete(pathFile);

                if (BHStaticSettings.UseLogsCleaner)
                {
                    Thread CleanerThread = new(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        LogsCleanProcedure.StartJob(BHStaticSettings.CleanUpDays, LogsPath);
                    });
                    Console.WriteLine("Starting Cleaner Thread..");
                    CleanerThread.Name = "BlackHoleLogsCleaner";
                    CleanerThread.Start();
                }
            }
            catch (Exception ex)
            {
                BHStaticSettings.UseLogsCleaner = false;
                Console.WriteLine(ex.Message);
            }
        }
    }

    internal static class LogsCleanProcedure
    {
        private static bool startCleaning = true;

        internal static async void StartJob(int daysBefore, string logsPath)
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                startCleaning = false;
            };

            double loops = CalculateLoops();
            double loopsCounter = 0;
            int days = daysBefore * -1;

            while (startCleaning)
            {
                await Task.Delay(5000);
                loopsCounter++;

                if (loopsCounter >= loops)
                {
                    RunCleaningJob(days, logsPath);
                    loopsCounter = 0;
                    loops = CalculateLoops();
                }
            }
        }

        private static double CalculateLoops()
        {
            DateTime now = DateTime.Now;
            DateTime rounded = new DateTime(now.Year, now.Month, now.Day, 0, 0, 10).AddDays(1);
            TimeSpan timeDifference = rounded - now;
            return Math.Round(timeDifference.TotalMinutes * 60 / 5);
        }

        private static void RunCleaningJob(int days, string logsPath)
        {
            try
            {
                string[] files = Directory.GetFiles(logsPath, "*.txt", SearchOption.TopDirectoryOnly);
                DateTime cleanBefore = DateTime.Now.AddDays(days);

                foreach (string file in files)
                {
                    FileInfo fi = new(file);

                    if (fi.CreationTime < cleanBefore)
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
