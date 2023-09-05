using BlackHole.Core;
using BlackHole.Services;
using BlackHole.Statics;
using System.Reflection;

namespace BlackHole.Internal
{
    internal class BHInitialDataBuilder
    {
        private readonly IBHConnection connection;
        private BHSqlExportWriter sqlWriter { get; set; }

        internal BHInitialDataBuilder()
        {
            connection = new BHConnection();
            sqlWriter = new BHSqlExportWriter("3_InitialDataSql", "SqlFiles", "sql");
        }

        internal void InsertDefaultData(List<Type> initialDataClasses)
        {
            CliConsoleLogs("Default data Initializer start..");
            foreach (Type initialData in initialDataClasses)
            {
                object? instance = Activator.CreateInstance(initialData);
                MethodInfo? method = initialData.GetMethod("DefaultData");
                if(instance !=null && method != null)
                {
                    object[] Argumnet = new object[1];
                    BHDataInitializer initializer = new();
                    Argumnet[0] = initializer;
                    method.Invoke(instance, Argumnet);
                    foreach(InitialCommandsAndParameters colsParams in initializer.commandsAndParameters)
                    {
                        bool result = connection.JustExecute(colsParams.commandText);

                        if (result)
                        {
                            CliConsoleLogs($"{colsParams.commandText};");

                            if (CliCommand.ExportSql)
                            {
                                sqlWriter.AddSqlCommand($"{colsParams.commandText};");
                            }
                        }
                        else
                        {
                            CliConsoleLogs($"Something went wrong with this command : {colsParams.commandText};");
                        }
                    }
                }
            }
            if (CliCommand.ExportSql)
            {
                sqlWriter.CreateSqlFile();
            }
            CliConsoleLogs("Default data were inserted.");
        }

        void CliConsoleLogs(string logCommand)
        {
            if (CliCommand.CliExecution)
            {
                Console.WriteLine("_bhLog_");
                Console.Write($"_bhLog_{logCommand}");
                Console.WriteLine("_bhLog_");
            }
        }
    }
}
