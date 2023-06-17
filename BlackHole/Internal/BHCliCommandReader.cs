using BlackHole.Enums;
using BlackHole.Statics;
using System.IO.Pipes;
using System.Reflection;
using System.Text.Json;

namespace BlackHole.Internal
{
    internal static class BHCliCommandReader
    {
        internal static bool ReadCliJson(Assembly assembly)
        {
            bool isInCliMode = false;

            string? mainDir = Path.GetDirectoryName(assembly.Location);

            if (mainDir != null)
            {
                string commandFilePath = Path.Combine(mainDir, "blackHole_Cli_command.json");

                if (File.Exists(commandFilePath))
                {
                    Console.WriteLine("_bhLog_");
                    Console.WriteLine("_bhLog_ Mikarsoft (R) BlackHole Cli v6.0.1 (C) for .Net Core");
                    Console.WriteLine("_bhLog_");
                    Console.WriteLine("_bhLog_ \t Cli Mode Enabled. Reading Command Arguments..");

                    try
                    {
                        CliCommand.CliExecution = true;
                        string jsonString = File.ReadAllText(commandFilePath);
                        BHCommandProperties commandFile = JsonSerializer.Deserialize<BHCommandProperties>(jsonString)!;
                        Console.WriteLine($"_bhLog_ \t {commandFile.CliCommand.ToUpper()} Database: '{DatabaseStatics.ConnectionString}'");
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
            CliCommandSettings cliSettings = new CliCommandSettings
            {
                forceExecution = CliCommand.ForceAction,
                saveExecutionSql = CliCommand.ExportSql
            };

            if (CliCommand.CliExecution)
            {
                switch (CliCommand.BHRun)
                {
                    case "update":
                        cliSettings.commandType = CliCommandTypes.Update;
                        break;
                    case "drop":
                        cliSettings.commandType = CliCommandTypes.Drop;
                        break;
                    case "parse":
                        cliSettings.commandType = CliCommandTypes.Parse;
                        break;
                    default:
                        cliSettings.commandType = CliCommandTypes.Default;
                        break;
                }
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

        public static bool InputPipe()
        {
            Console.WriteLine("bhInput");

            using (var pipeClient = new NamedPipeClientStream(".", "BlackHolePipe", PipeDirection.In))
            {
                // Attempt to connect to the pipe server
                pipeClient.Connect();

                using (var reader = new StreamReader(pipeClient))
                {
                    string? input = reader.ReadToEnd();
                    Console.WriteLine(input + "eftase");
                }
            }
            return false;
        }
    }
}
