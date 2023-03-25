
using System.Reflection;

namespace BlackHole.Configuration
{
    public class ConnectionAdditionalSettings
    {
        internal List<EntitiesWithNamespace> EntityNamespaces { get; set; } = new List<EntitiesWithNamespace>();
        internal List<ServicesWithNamespace> ServicesNamespaces { get; set; } = new List<ServicesWithNamespace>();
        internal List<AssembliesUsed> AssembliesToUse { get; set; } = new List<AssembliesUsed>();

        public ConnectionAdditionalSettings UseEntitiesFromNamespace(string entityNamespace)
        {
            EntityNamespaces.Add(new EntitiesWithNamespace { NamespaceUsed = entityNamespace});
            return this;
        }

        public ConnectionAdditionalSettings UseEntitiesFromNamespaces(List<string> entityNamespaces)
        {
            List<EntitiesWithNamespace> namespacesToAdd = new List<EntitiesWithNamespace>();

            foreach(string entityNamespace in entityNamespaces)
            {
                namespacesToAdd.Add(new EntitiesWithNamespace { NamespaceUsed = entityNamespace });
            }

            EntityNamespaces.AddRange(namespacesToAdd);
            return this;
        }

        public ConnectionAdditionalSettings AddServicesFromNamespace(string servicesNamespace)
        {
            ServicesNamespaces.Add(new ServicesWithNamespace { NamespaceUsed = servicesNamespace});
            return this;
        }

        public ConnectionAdditionalSettings AddServicesFromNamespaces(List<string> servicesNamespaces)
        {
            List<ServicesWithNamespace> namespacesToAdd = new List<ServicesWithNamespace>();

            foreach (string servicesNamespace in servicesNamespaces)
            {
                namespacesToAdd.Add(new ServicesWithNamespace { NamespaceUsed = servicesNamespace });
            }

            ServicesNamespaces.AddRange(namespacesToAdd);
            return this;
        }

        public ConnectionAdditionalSettings UseEntitiesFromNamespace(string entityNamespace, Assembly assbly)
        {
            EntityNamespaces.Add(new EntitiesWithNamespace { NamespaceUsed = entityNamespace });
            return this;
        }

        public ConnectionAdditionalSettings UseEntitiesFromNamespaces(List<string> entityNamespaces, Assembly assbly)
        {
            string? assemblyName = assbly.FullName;

            AssembliesUsed? existing = AssembliesToUse.Where(x => x.AssName == assemblyName).FirstOrDefault();

            if(assemblyName != null)
            {
                if (existing == null)
                {
                    AssembliesToUse.Add(new AssembliesUsed { AssName = assemblyName, ScanAssembly = assbly });
                }

                List<EntitiesWithNamespace> namespacesToAdd = new List<EntitiesWithNamespace>();

                foreach (string entityNamespace in entityNamespaces)
                {
                    namespacesToAdd.Add(new EntitiesWithNamespace { NamespaceUsed = entityNamespace, AssemblyNameUsed = assemblyName });
                }

                EntityNamespaces.AddRange(namespacesToAdd);
            }

            return this;
        }

        public ConnectionAdditionalSettings AddServicesFromNamespace(string servicesNamespace, Assembly assbly)
        {
            ServicesNamespaces.Add(new ServicesWithNamespace { NamespaceUsed = servicesNamespace });
            return this;
        }

        public ConnectionAdditionalSettings AddServicesFromNamespaces(List<string> servicesNamespaces, Assembly assbly)
        {
            List<ServicesWithNamespace> namespacesToAdd = new List<ServicesWithNamespace>();

            foreach (string servicesNamespace in servicesNamespaces)
            {
                namespacesToAdd.Add(new ServicesWithNamespace { NamespaceUsed = servicesNamespace });
            }

            ServicesNamespaces.AddRange(namespacesToAdd);
            return this;
        }
    }
}
