using BlackHole.Statics;
using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Logger
{
    internal static class LoggerService
    {
        internal static bool CanWriteLogs { get; set; } = false;
        internal static string LogsPath { get; set; } = string.Empty;

        internal static void  SetUpLogger()
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

                    CanWriteLogs = true;
                }
                else
                {
                    LogsPath = string.Empty;
                    CanWriteLogs = false;
                }
            }
            catch
            {
                DatabaseStatics.DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlackHoleData");
                LogsPath = Path.Combine(DatabaseStatics.DataPath, "Logs");
                CanWriteLogs = false;
            }
        }

        internal static string GenerateSHA1(this string text)
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

        internal static void CreateErrorLogs(this string commandText, string Area, string Message, string Details)
        {
            if (CanWriteLogs)
            {
                try
                {
                    string LogId = Guid.NewGuid().ToString() + DateTime.Now.ToString();
                    string pathFile = Path.Combine(LogsPath,$"{Area}_{LogId.GenerateSHA1()}.txt");

                    using (StreamWriter tw = new(pathFile, true))
                    {
                        tw.WriteLine($"Date and Time: {DateTime.Now.ToString("s").Replace(":", ".")}");
                        tw.WriteLine("");
                        tw.WriteLine($"Command : {commandText}");
                        tw.WriteLine("");
                        tw.WriteLine($"Error: {Message}");
                        tw.WriteLine("");
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
