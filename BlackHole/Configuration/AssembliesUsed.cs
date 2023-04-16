using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// Settings for the target Assemblies.
    /// </summary>
    public class AssembliesUsed
    {
        internal Assembly? ScanAssembly { get; set; }

        /// <summary>
        /// Scans a specified assembly for BlackHole Entities and Services
        /// and uses only them.
        /// </summary>
        /// <param name="otherAssembly"></param>
        public void UseOtherAssembly(Assembly otherAssembly)
        {
            ScanAssembly = otherAssembly;
        }
    }
}
