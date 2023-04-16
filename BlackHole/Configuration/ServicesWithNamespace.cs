
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
        /// Using only the services that are in the
        /// specified Namespace and inherit from BlackHole Services Classes.
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
        /// Using only the services that are in the
        /// specified Namespaces and inherit from BlackHole Services Classes.
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
        /// Scans a specified assembly for BlackHole Entities and Services
        /// and uses only them.
        /// </summary>
        /// <param name="otherAssembly">Full Assembly</param>
        public void UseOtherAssembly(Assembly otherAssembly)
        {
            AssemblyToUse = new AssembliesUsed();
            AssemblyToUse.ScanAssembly = otherAssembly;
        }
    }
}
