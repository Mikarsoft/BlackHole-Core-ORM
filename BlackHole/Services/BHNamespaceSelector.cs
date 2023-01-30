using BlackHole.Entities;
using BlackHole.Interfaces;
using System.Reflection;

namespace BlackHole.Services
{
    internal class BHNamespaceSelector : IBHNamespaceSelector
    {
        List<Type> IBHNamespaceSelector.GetAllBlazarEntities()
        {
            Assembly ass = Assembly.GetCallingAssembly();
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleEntity) || t.BaseType == typeof(BlackHoleEntityG)).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBlazarEntitiesInNamespace(string nameSpace)
        {
            Assembly ass = Assembly.GetCallingAssembly();
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) 
            && (t.BaseType== typeof(BlackHoleEntity) || t.BaseType == typeof(BlackHoleEntityG))).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBlazarEntitiesInNamespaces(List<string> nameSpaces)
        {
            List<Type> types = new List<Type>();
            Assembly ass = Assembly.GetCallingAssembly();

            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleEntity) || t.BaseType == typeof(BlackHoleEntityG))).ToList());
            }
            return types;
        }

        List<Type> IBHNamespaceSelector.GetAllBlazarEntities(Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleEntity) || t.BaseType== typeof(BlackHoleEntityG)).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBlazarEntitiesInNamespace(string nameSpace, Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
            && (t.BaseType == typeof(BlackHoleEntity) || t.BaseType == typeof(BlackHoleEntityG))).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBlazarEntitiesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new List<Type>();

            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleEntity) || t.BaseType == typeof(BlackHoleEntityG))).ToList());
            }
            return types;
        }

        List<Type> IBHNamespaceSelector.GetAllBlazarServices()
        {
            Assembly ass = Assembly.GetCallingAssembly();
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient)).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBlazarServicesInNamespace(string nameSpace)
        {
            Assembly ass = Assembly.GetCallingAssembly();
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
            && (t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient))).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBlazarServicesInNamespaces(List<string> nameSpaces)
        {
            List<Type> types = new List<Type>();
            Assembly ass = Assembly.GetCallingAssembly();

            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient))).ToList());
            }
            return types;
        }

        List<Type> IBHNamespaceSelector.GetAllBlazarServices(Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient)).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBlazarServicesInNamespace(string nameSpace, Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
            && (t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient))).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBlazarServicesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new List<Type>();

            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient))).ToList());
            }
            return types;
        }
    }
}
