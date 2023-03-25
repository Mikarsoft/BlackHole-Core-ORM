
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
            EntitiesWithNamespace? existing = EntityNamespaces
                .Where(x => x.NamespaceUsed == entityNamespace && x.AssemblyNameUsed == string.Empty).FirstOrDefault();

            if(existing == null)
            {
                EntityNamespaces.Add(new EntitiesWithNamespace { NamespaceUsed = entityNamespace });
            }

            return this;
        }

        public ConnectionAdditionalSettings UseEntitiesFromNamespaces(List<string> entityNamespaces)
        {
            foreach(string entityNamespace in entityNamespaces)
            {
                EntitiesWithNamespace? existing = EntityNamespaces
                    .Where(x => x.NamespaceUsed == entityNamespace && x.AssemblyNameUsed == string.Empty).FirstOrDefault();

                if(existing == null)
                {
                    EntityNamespaces.Add(new EntitiesWithNamespace { NamespaceUsed = entityNamespace });
                }
            }

            return this;
        }

        public ConnectionAdditionalSettings AddServicesFromNamespace(string servicesNamespace)
        {
            ServicesWithNamespace? existing = ServicesNamespaces
                .Where(x => x.NamespaceUsed == servicesNamespace && x.AssemblyNameUsed == string.Empty).FirstOrDefault();

            if (existing == null)
            {
                ServicesNamespaces.Add(new ServicesWithNamespace { NamespaceUsed = servicesNamespace });
            }

            return this;
        }

        public ConnectionAdditionalSettings AddServicesFromNamespaces(List<string> servicesNamespaces)
        {
            foreach (string servicesNamespace in servicesNamespaces)
            {
                ServicesWithNamespace? existing = ServicesNamespaces
                    .Where(x => x.NamespaceUsed == servicesNamespace && x.AssemblyNameUsed == string.Empty).FirstOrDefault();

                if( existing == null)
                {
                    ServicesNamespaces.Add(new ServicesWithNamespace { NamespaceUsed = servicesNamespace });
                }
            }

            return this;
        }

        public ConnectionAdditionalSettings UseEntitiesFromNamespace(string entityNamespace, Assembly otherAssembly)
        {
            string? assemblyName = otherAssembly.FullName;

            if (assemblyName != null)
            {
                AssembliesUsed? existing = AssembliesToUse.Where(x => x.AssName == assemblyName).FirstOrDefault();

                if (existing == null)
                {
                    AssembliesToUse.Add(new AssembliesUsed { AssName = assemblyName, ScanAssembly = otherAssembly });
                }

                EntitiesWithNamespace? existingNamespace = EntityNamespaces
                    .Where(x => x.NamespaceUsed == entityNamespace && x.AssemblyNameUsed == assemblyName).FirstOrDefault();

                if (existingNamespace == null)
                {
                    EntityNamespaces.Add(new EntitiesWithNamespace { NamespaceUsed = entityNamespace, AssemblyNameUsed = assemblyName });
                }
            }

            return this;
        }

        public ConnectionAdditionalSettings UseEntitiesFromNamespaces(List<string> entityNamespaces, Assembly otherAssembly)
        {
            string? assemblyName = otherAssembly.FullName;

            if(assemblyName != null)
            {
                AssembliesUsed? existing = AssembliesToUse.Where(x => x.AssName == assemblyName).FirstOrDefault();

                if (existing == null)
                {
                    AssembliesToUse.Add(new AssembliesUsed { AssName = assemblyName, ScanAssembly = otherAssembly });
                }

                foreach (string entityNamespace in entityNamespaces)
                {
                    EntitiesWithNamespace? existingNamespace = EntityNamespaces
                        .Where(x => x.NamespaceUsed == entityNamespace && x.AssemblyNameUsed == assemblyName).FirstOrDefault();

                    if(existingNamespace == null)
                    {
                        EntityNamespaces.Add(new EntitiesWithNamespace { NamespaceUsed = entityNamespace, AssemblyNameUsed = assemblyName });
                    }
                }
            }

            return this;
        }

        public ConnectionAdditionalSettings AddServicesFromNamespace(string servicesNamespace, Assembly otherAssembly)
        {
            string? assemblyName = otherAssembly.FullName;

            if (assemblyName != null)
            {
                AssembliesUsed? existing = AssembliesToUse.Where(x => x.AssName == assemblyName).FirstOrDefault();

                if (existing == null)
                {
                    AssembliesToUse.Add(new AssembliesUsed { AssName = assemblyName, ScanAssembly = otherAssembly });
                }

                ServicesWithNamespace? existingNamespace = ServicesNamespaces
                    .Where(x => x.NamespaceUsed == servicesNamespace && x.AssemblyNameUsed == assemblyName).FirstOrDefault();

                if(existingNamespace == null)
                {
                    ServicesNamespaces.Add(new ServicesWithNamespace { NamespaceUsed = servicesNamespace, AssemblyNameUsed = assemblyName});
                }
            }

            return this;
        }

        public ConnectionAdditionalSettings AddServicesFromNamespaces(List<string> servicesNamespaces, Assembly otherAssembly)
        {
            string? assemblyName = otherAssembly.FullName;

            if (assemblyName != null)
            {
                AssembliesUsed? existing = AssembliesToUse.Where(x => x.AssName == assemblyName).FirstOrDefault();

                if (existing == null)
                {
                    AssembliesToUse.Add(new AssembliesUsed { AssName = assemblyName, ScanAssembly = otherAssembly });
                }

                foreach (string servicesNamespace in servicesNamespaces)
                {
                    ServicesWithNamespace? existingNamespace = ServicesNamespaces
                        .Where(x => x.NamespaceUsed == servicesNamespace && x.AssemblyNameUsed == assemblyName).FirstOrDefault();

                    if (existingNamespace == null)
                    {
                        ServicesNamespaces.Add(new ServicesWithNamespace { NamespaceUsed = servicesNamespace, AssemblyNameUsed = assemblyName });
                    }
                }
            }

            return this;
        }
    }
}
