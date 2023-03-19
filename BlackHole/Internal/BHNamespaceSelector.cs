using BlackHole.Entities;
using BlackHole.Services;
using System.Reflection;

namespace BlackHole.Internal
{
    internal class BHNamespaceSelector : IBHNamespaceSelector
    {
        List<Type> IBHNamespaceSelector.GetAllBHEntities(Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleEntity<int>)
            || t.BaseType == typeof(BlackHoleEntity<Guid>)
            || t.BaseType == typeof(BlackHoleEntity<string>)).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBHEntitiesInNamespace(string nameSpace, Assembly ass)
        {
            List<Type> types = new List<Type>();
            types = ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
            && (t.BaseType == typeof(BlackHoleEntity<int>)
            || t.BaseType == typeof(BlackHoleEntity<Guid>)
            || t.BaseType == typeof(BlackHoleEntity<string>))).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBHEntitiesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new List<Type>();
            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleEntity<int>)
                || t.BaseType == typeof(BlackHoleEntity<Guid>)
                || t.BaseType == typeof(BlackHoleEntity<string>))).ToList());
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
            types = ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
            && (t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient))).ToList();
            return types;
        }

        List<Type> IBHNamespaceSelector.GetBHServicesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new List<Type>();

            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient))).ToList());
            }
            return types;
        }
    }
}
