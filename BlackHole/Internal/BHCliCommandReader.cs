using BlackHole.Enums;
using BlackHole.Statics;
using System.Reflection;
using System.Text.Json;

namespace BlackHole.Internal
{
    internal static class BHCliCommandReader
    {
        internal static bool ReadCliJson(Assembly assembly, string connectionString)
        {
            bool isInCliMode = false;

            string? mainDir = Path.GetDirectoryName(assembly.Location);

            if (mainDir != null)
            {
                string commandFilePath = Path.Combine(mainDir, "blackHole_Cli_command.json");

                if (File.Exists(commandFilePath))
                {
                    Console.WriteLine("_bhLog_");
                    Console.WriteLine("_bhLog_ Mikarsoft (R) BlackHole Cli v6.2.1 Singularity Edition (C) for .Net Core");
                    Console.WriteLine("_bhLog_");
                    Console.WriteLine("_bhLog_ \t Cli Mode Enabled. Reading Command Arguments..");

                    try
                    {
                        CliCommand.CliExecution = true;
                        string jsonString = File.ReadAllText(commandFilePath);
                        BHCommandProperties commandFile = JsonSerializer.Deserialize<BHCommandProperties>(jsonString)!;
                        Console.WriteLine($"_bhLog_ \t {commandFile.CliCommand.ToUpper()} Database: '{connectionString}'");
                        Console.WriteLine($"_bhLog_ \t Project Directory: '{commandFile.ProjectPath}'");
                        CliCommand.BHRun = commandFile.CliCommand;
                        CliCommand.ProjectPath = commandFile.ProjectPath;
                        CheckCommandParameters(commandFile);
                        File.Delete(commandFilePath);
                        isInCliMode = true;
                        Console.WriteLine("_bhLog_");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        isInCliMode=false;
                        Environment.Exit(500);
                    }
                }
            }
            return isInCliMode;
        }

        internal static CliCommandSettings GetCliCommandSettings()
        {
            CliCommandSettings cliSettings = new()
            {
                forceExecution = CliCommand.ForceAction,
                saveExecutionSql = CliCommand.ExportSql
            };

            if (CliCommand.CliExecution)
            {
                cliSettings.commandType = CliCommand.BHRun switch
                {
                    "update" => CliCommandTypes.Update,
                    "drop" => CliCommandTypes.Drop,
                    "parse" => CliCommandTypes.Parse,
                    _ => CliCommandTypes.Default,
                };
            }
            else
            {
                cliSettings.commandType = CliCommandTypes.Default;
            }

            return cliSettings;
        }

        private static void CheckCommandParameters(BHCommandProperties commandParameters)
        {
            if (!string.IsNullOrEmpty(commandParameters.SettingMode))
            {
                if(commandParameters.SettingMode == "forceaction")
                {
                    CliCommand.ForceAction = true;
                }

                if(commandParameters.SettingMode == "savesql")
                {
                    CliCommand.ExportSql = true;
                }
            }

            if (!string.IsNullOrEmpty(commandParameters.ExtraMode))
            {
                if (commandParameters.ExtraMode == "forceaction")
                {
                    CliCommand.ForceAction = true;
                }

                if (commandParameters.ExtraMode == "savesql")
                {
                    CliCommand.ExportSql = true;
                }
            }
        }
    }

    internal class CliCommandSettings
    {
        internal CliCommandTypes commandType { get; set; }
        internal bool forceExecution { get; set; }
        internal bool saveExecutionSql { get; set; }
    }
}
