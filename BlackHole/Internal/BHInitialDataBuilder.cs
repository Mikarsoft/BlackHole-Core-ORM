using BlackHole.Core;
using BlackHole.Engine;
using BlackHole.PreLoads;
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

                MethodInfo? dfMethod = initialData.GetMethod("DefaultData");
                MethodInfo? svMethod = initialData.GetMethod("StoredViews");
                MethodInfo? spMethod = initialData.GetMethod("StoredProcedures");

                InitialTransaction _transaction = new();

                if (instance != null)
                {
                    if(dfMethod != null)
                    {
                        object[] Argumnet = new object[1];
                        DefaultDataBuilder initializer = new DefaultDataBuilder(_transaction);

                        Argumnet[0] = initializer;
                        dfMethod.Invoke(instance, Argumnet);
                    }

                    if(svMethod != null)
                    {

                    }

                    if(spMethod != null)
                    {

                    }

                }

                _transaction.Commit();
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
