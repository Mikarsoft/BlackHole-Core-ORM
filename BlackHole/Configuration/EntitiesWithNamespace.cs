
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// Settings for the Entities Namespaces that will be used
    /// </summary>
    public class EntitiesWithNamespace
    {
        internal List<string> EntitiesNamespaces = new List<string>();
        internal AssembliesUsed? AssemblyToUse { get; set; }

        /// <summary>
        /// Using only the Entities that are in the specified 
        /// Namespace.
        /// </summary>
        /// <param name="entityNamespace">Namespace Full Name 'MyProject.Entities....'</param>
        /// <returns></returns>
        public AssembliesUsed UseEntitiesInNamespace(string entityNamespace)
        {
            EntitiesNamespaces.Add(entityNamespace);
            AssemblyToUse = new AssembliesUsed();
            return AssemblyToUse;
        }

        /// <summary>
        /// Using only the Entities in the specified Namespaces.
        /// </summary>
        /// <param name="entityNamespaces">List of Namespaces Full Names</param>
        /// <returns></returns>
        public AssembliesUsed UseEntitiesInNamespaces(List<string> entityNamespaces)
        {
            EntitiesNamespaces = entityNamespaces;
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
