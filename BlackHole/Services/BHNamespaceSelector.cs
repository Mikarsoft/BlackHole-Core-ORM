using BlackHole.Entities;
using BlackHole.Interfaces;
using System.Reflection;

namespace BlackHole.Services
{
    internal class BHNamespaceSelector : IBHNamespaceSelector
    {
        List<Type> IBHNamespaceSelector.GetAllBHEntities(Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleEntity) || t.BaseType== typeof(BlackHoleEntityG)).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBHEntitiesInNamespace(string nameSpace, Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
            && (t.BaseType == typeof(BlackHoleEntity) || t.BaseType == typeof(BlackHoleEntityG))).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBHEntitiesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new List<Type>();

            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleEntity) || t.BaseType == typeof(BlackHoleEntityG))).ToList());
            }
            return types;
        }

        List<Type> IBHNamespaceSelector.GetAllBHServices(Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient)).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBHServicesInNamespace(string nameSpace, Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
            && (t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient))).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBHServicesInNamespaces(List<string> nameSpaces, Assembly ass)
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
