
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class ServicesWithNamespace
    {
        internal List<string> ServicesNamespaces = new List<string>();
        internal AssembliesUsed? AssemblyToUse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="servicesNamespace"></param>
        /// <returns></returns>
        public AssembliesUsed AddServicesFromNamespace(string servicesNamespace)
        {
            ServicesNamespaces.Add(servicesNamespace);
            AssemblyToUse = new AssembliesUsed();
            return AssemblyToUse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="servicesNamespaces"></param>
        /// <returns></returns>
        public AssembliesUsed AddServicesFromNamespaces(List<string> servicesNamespaces)
        {
            ServicesNamespaces = servicesNamespaces;
            AssemblyToUse = new AssembliesUsed();
            return AssemblyToUse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherAssembly"></param>
        public void UseOtherAssembly(Assembly otherAssembly)
        {
            AssemblyToUse = new AssembliesUsed();
            AssemblyToUse.ScanAssembly = otherAssembly;
        }
    }
}
