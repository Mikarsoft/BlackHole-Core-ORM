using BlackHole.Entities;
using BlackHole.Services;
using System.Reflection;

namespace BlackHole.Internal
{
    internal class BHNamespaceSelector
    {
        internal List<Type> GetInitialData(Assembly ass)
        {
            Type type = typeof(IBHInitialData);
            return ass.GetTypes().Where(p => type.IsAssignableFrom(p)).ToList();
        }

        internal List<Type> GetAllBHEntities(Assembly ass)
        {
            return ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleEntity<int>)
            || t.BaseType == typeof(BlackHoleEntity<Guid>)
            || t.BaseType == typeof(BlackHoleEntity<string>)).ToList();
        }

        internal List<Type> GetBHEntitiesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new();
            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleEntity<int>)
                || t.BaseType == typeof(BlackHoleEntity<Guid>)
                || t.BaseType == typeof(BlackHoleEntity<string>))).ToList());
            }
            return types;
        }

        internal List<Type> GetOpenAllBHEntities(Assembly ass)
        {
            Type openEntity = typeof(IBHOpenEntity<>);
            return ass.GetTypes().Where(t => openEntity.IsAssignableFrom(t)).ToList();
        }

        internal List<Type> GetOpenBHEntitiesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new();
            Type openEntity = typeof(IBHOpenEntity<>);
            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && openEntity.IsAssignableFrom(t)).ToList());
            }
            return types;
        }

        internal List<Type> GetBHServicesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new();
            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BlackHoleScoped) || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient))).ToList());
            }
            return types;
        }
    }
}
