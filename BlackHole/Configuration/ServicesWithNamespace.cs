
using System.Reflection;

namespace BlackHole.Configuration
{
    public class ServicesWithNamespace
    {
        internal List<string> ServicesNamespaces = new List<string>();
        internal AssembliesUsed? AssemblyToUse { get; set; }

        public AssembliesUsed AddServicesFromNamespace(string servicesNamespace)
        {
            ServicesNamespaces.Add(servicesNamespace);
            AssemblyToUse = new AssembliesUsed();
            return AssemblyToUse;
        }

        public AssembliesUsed AddServicesFromNamespaces(List<string> servicesNamespaces)
        {
            ServicesNamespaces = servicesNamespaces;
            AssemblyToUse = new AssembliesUsed();
            return AssemblyToUse;
        }

        public void UseOtherAssembly(Assembly otherAssembly)
        {
            AssemblyToUse = new AssembliesUsed();
            AssemblyToUse.ScanAssembly = otherAssembly;
        }
    }
}
