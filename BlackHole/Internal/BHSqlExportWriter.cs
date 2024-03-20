using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHSqlExportWriter
    {
        private string SqlFilesPath { get; set; }
        private bool canWriteFiles { get; set; }
        private string SqlFileName { get; set; }
        private string TypeOfFile { get; set; }
        private List<string> SqlCommandsList { get; set; }

        internal BHSqlExportWriter(string fileName,string folderName,string fileType)
        {
            SqlFileName = fileName;
            SqlCommandsList = new List<string>();
            TypeOfFile = fileType;

            try
            {
                if (BHStaticSettings.UseLogging)
                {
                    SqlFilesPath = Path.Combine(BHStaticSettings.DataPath, folderName);

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
                BHStaticSettings.DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlackHoleData");
                SqlFilesPath = Path.Combine(BHStaticSettings.DataPath, "Logs");
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

        internal void AddMultiple(List<string> commandLines)
        {
            SqlCommandsList.AddRange(commandLines);
        }

        internal void DeleteSqlFolder()
        {
            try
            {
                Console.WriteLine("_bhLog_");
                Console.WriteLine($"_bhLog_ Deleting Sql Folder... : {SqlFilesPath}");

                if (Directory.Exists(SqlFilesPath))
                {
                    Directory.Delete(SqlFilesPath, true);
                }
                Console.WriteLine($"_bhLog_ \t Sql Folder has been deleted.");
                Console.WriteLine("_bhLog_");
            }
            catch (Exception ex)
            {
                Console.WriteLine("_bhLog_");
                Console.WriteLine($"_bhLog_ Failed to delete Sql Directory : {ex.Message}");
                Console.WriteLine("_bhLog_");
            }
        }

        internal void CreateSqlFile()
        {
            if (canWriteFiles && SqlCommandsList.Count > 0)
            {
                try
                {
                    if (!Directory.Exists(SqlFilesPath))
                    {
                        Directory.CreateDirectory(SqlFilesPath);
                    }

                    string pathFile = Path.Combine(SqlFilesPath, $"{SqlFileName}.{TypeOfFile}");

                    Console.WriteLine("_bhLog_");
                    Console.WriteLine($"_bhLog_ Creating Sql File... : {pathFile}");

                    if (File.Exists(pathFile))
                    {
                        using(var sw = File.AppendText(pathFile))
                        {
                            sw.WriteLine("");
                            sw.WriteLine($"-- update at: {DateTime.Now} --");
                            sw.WriteLine("");

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
                            tw.WriteLine("");
                            tw.WriteLine($"-- update at: {DateTime.Now} --");
                            tw.WriteLine("");

                            foreach (string commandText in SqlCommandsList)
                            {
                                tw.WriteLine($"{commandText}");
                            }
                        }
                    }
                    Console.WriteLine("_bhLog_ \t Sql file has been created.");
                    Console.WriteLine("_bhLog_");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("_bhLog_");
                    Console.WriteLine($"_bhLog_ Sql Writer Error : {ex.Message}");
                    Console.WriteLine("_bhLog_");
                }
            }
        }
    }
}
