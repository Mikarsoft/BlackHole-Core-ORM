using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class SqlExportWriter
    {
        private string SqlFilesPath { get; set; }
        private bool canWriteFiles { get; set; }
        private string SqlFileName { get; set; }
        private List<string> SqlCommandsList { get; set; }

        internal SqlExportWriter(string fileName)
        {
            SqlFileName = fileName;
            SqlCommandsList = new List<string>();

            try
            {
                if (DatabaseStatics.UseLogging)
                {
                    SqlFilesPath = Path.Combine(DatabaseStatics.DataPath, "SqlFiles");

                    if (!Directory.Exists(SqlFilesPath))
                    {
                        Directory.CreateDirectory(SqlFilesPath);
                    }

                    canWriteFiles = true;
                }
                else
                {
                    SqlFilesPath = string.Empty;
                    canWriteFiles = false;
                }
            }
            catch
            {
                DatabaseStatics.DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlackHoleData");
                SqlFilesPath = Path.Combine(DatabaseStatics.DataPath, "Logs");
                canWriteFiles = false;
            }

            if (!canWriteFiles)
            {
                try
                {
                    Directory.CreateDirectory(SqlFilesPath);
                    canWriteFiles = true;
                }
                catch
                {
                    canWriteFiles = false;
                }
            }
        }

        internal void AddSqlCommand(string commandText)
        {
            SqlCommandsList.Add(commandText);
        }

        internal void CreateSqlFile()
        {
            if (canWriteFiles && SqlCommandsList.Count > 0)
            {
                try
                {
                    string pathFile = Path.Combine(SqlFilesPath, $"{SqlFileName}.sql");

                    if (File.Exists(pathFile))
                    {
                        using(var sw = File.AppendText(pathFile))
                        {
                            foreach (string commandText in SqlCommandsList)
                            {
                                sw.WriteLine($"{commandText}");
                            }
                        }
                    }
                    else
                    {
                        using (var tw = new StreamWriter(pathFile, true))
                        {
                            foreach (string commandText in SqlCommandsList)
                            {
                                tw.WriteLine($"{commandText}");
                            }
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
