using System.Reflection;

namespace BlackHole.Configuration
{
    public class MultiSchemaSettings
    {
        internal List<Assembly> AssembliesToUse { get; set; } = new List<Assembly>();
        internal bool useCallingAssembly { get; set; } = true;
        internal int ConnectionTimeOut { get; set; } = 60;

        /// <summary>
        /// Change the Timeout of each command.The Default timeout of BlackHole is 60s.
        /// <para>This Feature does not apply to SqLite database</para>
        /// </summary>
        /// <param name="timoutInSeconds">The timtout in seconds that will be applied to each command</param>
        /// <returns>Connection Additional Settings</returns>
        public MultiSchemaSettings SetConnectionTimeoutSeconds(int timoutInSeconds)
        {
            ConnectionTimeOut = timoutInSeconds;
            return this;
        }
    }
}
