using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Provider.Abstractions
{
    public class UtilizationConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityNamespaces"></param>
        /// <param name="servicesNamespaces"></param>
        public void UseNamespaces(Action<List<string>> entityNamespaces, Action<List<string>> servicesNamespaces)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityNamespaces"></param>
        public void UseEntitiesInNamespaces(Action<List<string>> entityNamespaces)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityNamespaces"></param>
        public void UseServicesInNamespaces(Action<List<string>> entityNamespaces)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityNamespace"></param>
        /// <param name="servicesNamespace"></param>
        public void UseNamespaces(string entityNamespace, string servicesNamespace)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityNamespaces"></param>
        public void UseEntitiesInNamespace(string entityNamespaces)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="entityNamespaces"></param>
        public void UseServicesInNamespace(string assemblyPath, string entityNamespaces)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="entityNamespaces"></param>
        /// <param name="servicesNamespaces"></param>
        /// <param name="includeCallingAssembly"></param>
        public void UseNamespacesFromAssembly(string assemblyPath, Action<List<string>> entityNamespaces,
            Action<List<string>> servicesNamespaces, bool includeCallingAssembly = false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="entityNamespaces"></param>
        /// <param name="includeCallingAssembly"></param>
        public void UseEntitiesInNamespacesFromAssembly(string assemblyPath, Action<List<string>> entityNamespaces,
            bool includeCallingAssembly = false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="entityNamespaces"></param>
        /// <param name="includeCallingAssembly"></param>
        public void UseServicesInNamespacesFromAssembly(string assemblyPath, Action<List<string>> entityNamespaces,
            bool includeCallingAssembly = false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="entityNamespace"></param>
        /// <param name="servicesNamespace"></param>
        /// <param name="includeCallingAssembly"></param>
        public void UseNamespacesFromAssembly(string assemblyPath, string entityNamespace,
            string servicesNamespace, bool includeCallingAssembly = false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="entityNamespaces"></param>
        /// <param name="includeCallingAssembly"></param>
        public void UseEntitiesInNamespaceFromAssembly(string assemblyPath, string entityNamespaces,
            bool includeCallingAssembly = false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="entityNamespaces"></param>
        /// <param name="includeCallingAssembly"></param>
        public void UseServicesInNamespaceFromAssembly(string assemblyPath, string entityNamespaces,
            bool includeCallingAssembly = false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="includeCallingAssembly"></param>
        public void UseOtherAssembly(string assemblyPath, bool includeCallingAssembly = false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembliesFolderPath"></param>
        /// <param name="includeCallingAssembly"></param>
        public void UseOtherAssemblies(string assembliesFolderPath, bool includeCallingAssembly = false)
        {

        }
    }
}
