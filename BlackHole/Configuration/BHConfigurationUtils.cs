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
        /// <param name="otherAssembly">Full Assembly</param>
        public void UseOtherAssembly(Assembly otherAssembly)
        {
            ScanAssembly = otherAssembly;
        }
    }

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

    /// <summary>
    /// Settings for the Services Namespaces that will be used
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
