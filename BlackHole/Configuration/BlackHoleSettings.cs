
using System.Reflection;

namespace BlackHole.Configuration
{
    public class BlackHoleSettings
    {
        public BHConfig BHConfiguration { get; set; } = new BHConfig();
        public string LogsPath { get; set; } = string.Empty;
        public Assembly? callingAssembly { get; set; }


        public BlackHoleSettings AddDatabase(Action<ConnectionSettings> connectionSettings)
        {
            return this;
        }

        public BlackHoleSettings SetLogsPath(string LogsPath)
        {
            this.LogsPath = LogsPath;
            return this;
        }

        public BlackHoleSettings GetEntitiesFromAnotherAssembly(Assembly assembly)
        {
            callingAssembly = assembly;
            return this;
        }
    }
}
