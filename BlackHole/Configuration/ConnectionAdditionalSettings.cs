
using System.Reflection;

namespace BlackHole.Configuration
{
    public class ConnectionAdditionalSettings
    {
        internal EntitiesWithNamespace? EntityNamespaces { get; set; }
        internal ServicesWithNamespace? ServicesNamespaces { get; set; }
        internal List<Assembly> AssembliesToUse { get; set; } = new List<Assembly>();
        internal bool useCallingAssembly { get; set; } = true;

        public ServicesWithNamespace UseEntitiesInNamespace(string entityNamespace)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            EntityNamespaces.EntitiesNamespaces.Add(entityNamespace);

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        public ServicesWithNamespace UseEntitiesInNamespaces(List<string> entityNamespaces)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            EntityNamespaces.EntitiesNamespaces = entityNamespaces;

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        public ServicesWithNamespace UseEntitiesInNamespaces(Action<List<string>> entityNamespaces)
        {
            EntityNamespaces = new EntitiesWithNamespace();
            entityNamespaces.Invoke(EntityNamespaces.EntitiesNamespaces);

            ServicesNamespaces = new ServicesWithNamespace();
            return ServicesNamespaces;
        }

        public EntitiesWithNamespace AddServicesFromNamespace(string servicesNamespace)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            ServicesNamespaces.ServicesNamespaces.Add(servicesNamespace);

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        public EntitiesWithNamespace AddServicesFromNamespaces(List<string> servicesNamespaces)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            ServicesNamespaces.ServicesNamespaces = servicesNamespaces;

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        public EntitiesWithNamespace AddServicesFromNamespaces(Action<List<string>> servicesNamespaces)
        {
            ServicesNamespaces = new ServicesWithNamespace();
            servicesNamespaces.Invoke(ServicesNamespaces.ServicesNamespaces);

            EntityNamespaces = new EntitiesWithNamespace();
            return EntityNamespaces;
        }

        public void UseOtherAssembly(Assembly otherAssembly)
        {
            AssembliesToUse.Add(otherAssembly);
            useCallingAssembly = false;
        }

        public void UseOtherAssemblies(List<Assembly> otherAssemblies)
        {
            AssembliesToUse = otherAssemblies;
            useCallingAssembly = false;
        }

        public void UseOtherAssemblies(Action<List<Assembly>> otherAssemblies)
        {
            otherAssemblies.Invoke(AssembliesToUse);
            useCallingAssembly = false;
        }

        public void UseAdditionalAssembly(Assembly additionalAssembly)
        {
            AssembliesToUse.Add(additionalAssembly);
            useCallingAssembly = true;
        }

        public void UseAdditionalAssemblies(List<Assembly> additionalAssemblies)
        {
            AssembliesToUse = additionalAssemblies;
            useCallingAssembly = true;
        }

        public void UseAdditionalAssemblies(Action<List<Assembly>> additionalAssemblies)
        {
            additionalAssemblies.Invoke(AssembliesToUse);
            useCallingAssembly = true;
        }
    }
}
