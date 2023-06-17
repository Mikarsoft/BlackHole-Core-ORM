
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// Additional configuration object 
    /// for the Namespaces and the Assemblies that
    /// will be used by the BlackHole.
    /// </summary>
    public class ConnectionAdditionalSettings
    {
        internal EntitiesWithNamespace? EntityNamespaces { get; set; }
        internal ServicesWithNamespace? ServicesNamespaces { get; set; }
        internal List<Assembly> AssembliesToUse { get; set; } = new List<Assembly>();
        internal bool useCallingAssembly { get; set; } = true;
        internal int ConnectionTimeOut { get; set; } = 180;

        /// <summary>
        /// Change the Timeout of each command.The Default timeout of BlackHole is 180s.
        /// <para>This Feature does not apply to SqLite database</para>
        /// </summary>
        /// <param name="timoutInSeconds">The timtout in seconds that will be applied to each command</param>
        /// <returns>Connection Additional Settings</returns>
        public ConnectionAdditionalSettings SetConnectionTimeoutSeconds(int timoutInSeconds)
        {
            ConnectionTimeOut = timoutInSeconds;
            return this;
        }

        /// <summary>
        /// Using only the Entities that are in the specified 
        /// Namespace.
        /// </summary>
        /// <param name="entityNamespace">Namespace Full Name 'MyProject.Entities....'</param>
        /// <returns></returns>
        public ServicesWithNamespace UseEntitiesInNamespace(string entityNamespace)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            EntityNamespaces.EntitiesNamespaces.Add(entityNamespace);

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        /// <summary>
        /// Using only the Entities in the specified Namespaces.
        /// </summary>
        /// <param name="entityNamespaces">List of Namespaces Full Names</param>
        /// <returns></returns>
        public ServicesWithNamespace UseEntitiesInNamespaces(List<string> entityNamespaces)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            EntityNamespaces.EntitiesNamespaces = entityNamespaces;

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        /// <summary>
        /// Using only the Entities in the specified Namespaces.
        /// </summary>
        /// <param name="entityNamespaces">List of Namespaces Full Names</param>
        /// <returns></returns>
        public ServicesWithNamespace UseEntitiesInNamespaces(Action<List<string>> entityNamespaces)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            entityNamespaces.Invoke(EntityNamespaces.EntitiesNamespaces);

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        /// <summary>
        /// Using only the services that are in the
        /// specified Namespace and inherit from BlackHole Services Classes.
        /// </summary>
        /// <param name="servicesNamespace">Namespace Full Name 'MyProject.Services....'</param>
        /// <returns></returns>
        public EntitiesWithNamespace AddServicesFromNamespace(string servicesNamespace)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            ServicesNamespaces.ServicesNamespaces.Add(servicesNamespace);

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        /// <summary>
        /// Using only the services that are in the
        /// specified Namespaces and inherit from BlackHole Services Classes.
        /// </summary>
        /// <param name="servicesNamespaces">List of Namespaces Full Names</param>
        /// <returns></returns>
        public EntitiesWithNamespace AddServicesFromNamespaces(List<string> servicesNamespaces)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            ServicesNamespaces.ServicesNamespaces = servicesNamespaces;

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        /// <summary>
        /// Using only the services that are in the
        /// specified Namespaces and inherit from BlackHole Services Classes.
        /// </summary>
        /// <param name="servicesNamespaces">List of Namespaces Full Names</param>
        /// <returns></returns>
        public EntitiesWithNamespace AddServicesFromNamespaces(Action<List<string>> servicesNamespaces)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            servicesNamespaces.Invoke(ServicesNamespaces.ServicesNamespaces);

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services ONLY from the 
        /// specified Assembly
        /// </summary>
        /// <param name="otherAssembly">Full Assembly</param>
        public void UseOtherAssembly(Assembly otherAssembly)
        {
            AssembliesToUse.Add(otherAssembly);
            useCallingAssembly = false;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services ONLY from the 
        /// specified Assemblies
        /// </summary>
        /// <param name="otherAssemblies">List of Assemblies</param>
        public void UseOtherAssemblies(List<Assembly> otherAssemblies)
        {
            AssembliesToUse = otherAssemblies;
            useCallingAssembly = false;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services ONLY from the 
        /// specified Assemblies
        /// </summary>
        /// <param name="otherAssemblies">List of Assemblies</param>
        public void UseOtherAssemblies(Action<List<Assembly>> otherAssemblies)
        {
            otherAssemblies.Invoke(AssembliesToUse);
            useCallingAssembly = false;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services from the 
        /// specified Assembly and the Calling Assembly
        /// </summary>
        /// <param name="additionalAssembly">Full Assembly</param>
        public void UseAdditionalAssembly(Assembly additionalAssembly)
        {
            AssembliesToUse.Add(additionalAssembly);
            useCallingAssembly = true;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services from the 
        /// specified Assemblies and the Calling Assembly
        /// </summary>
        /// <param name="additionalAssemblies">List of Assemblies</param>
        public void UseAdditionalAssemblies(List<Assembly> additionalAssemblies)
        {
            AssembliesToUse = additionalAssemblies;
            useCallingAssembly = true;
        }

        /// <summary>
        /// Using BlackHole Entities and BlackHole Services from the 
        /// specified Assemblies and the Calling Assembly
        /// </summary>
        /// <param name="additionalAssemblies">List of Assemblies</param>
        public void UseAdditionalAssemblies(Action<List<Assembly>> additionalAssemblies)
        {
            additionalAssemblies.Invoke(AssembliesToUse);
            useCallingAssembly = true;
        }
    }
}
