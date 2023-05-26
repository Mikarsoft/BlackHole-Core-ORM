using BlackHole.Core;
using BlackHole.Services;
using System.Reflection;

namespace BlackHole.Internal
{
    internal class BHInitialDataBuilder
    {
        private readonly IBHConnection connection;

        internal BHInitialDataBuilder()
        {
            connection = new BHConnection();
        }

        internal void InsertDefaultData(List<Type> initialDataClasses)
        {
            foreach(Type initialData in initialDataClasses)
            {
                object? instance = Activator.CreateInstance(initialData);

                MethodInfo? method = initialData.GetMethod("DefaultData");

                if(instance !=null && method != null)
                {
                    object[] Argumnet = new object[1];
                    BHDataInitializer initializer = new BHDataInitializer();
                    Argumnet[0] = initializer;
                    method.Invoke(instance, Argumnet);

                    foreach(InitialCommandsAndParameters colsParams in initializer.commandsAndParameters)
                    {
                        if(colsParams.comandParameters != null)
                        {
                            connection.JustExecute(colsParams.commandText, colsParams.comandParameters);
                        }
                        else
                        {
                            connection.JustExecute(colsParams.commandText);
                        }
                    }
                }
            }

            Console.WriteLine("Default Data Initialized");
        }
    }
}
