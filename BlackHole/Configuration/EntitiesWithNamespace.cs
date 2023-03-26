
using System.Reflection;

namespace BlackHole.Configuration
{
    public class EntitiesWithNamespace
    {
        internal List<string> EntitiesNamespaces = new List<string>();
        internal AssembliesUsed? AssemblyToUse { get; set; }

        public AssembliesUsed UseEntitiesInNamespace(string entitiesNamespace)
        {
            EntitiesNamespaces.Add(entitiesNamespace);
            AssemblyToUse = new AssembliesUsed();
            return AssemblyToUse;
        }

        public AssembliesUsed UseEntitiesInNamespaces(List<string> entitiesNamespaces)
        {
            EntitiesNamespaces = entitiesNamespaces;
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
