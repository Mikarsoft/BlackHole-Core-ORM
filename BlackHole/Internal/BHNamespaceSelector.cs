﻿using BlackHole.Entities;
using BlackHole.Services;
using System.Reflection;
using System.Xml.Linq;

namespace BlackHole.Internal
{
    internal class BHNamespaceSelector
    {
        internal List<Type> GetInitialData(Assembly ass)
        {
            Type type = ass.GetType(); //typeof(IBHInitialData);
            return ass.GetTypes().Where(p => type.IsAssignableFrom(p)).ToList();
        }

        internal List<Type> GetAllBHEntities(Assembly ass)
        {
            return ass.GetTypes().Where(t => t.BaseType == typeof(BHEntityAI<int>)
            || t.BaseType == typeof(BHEntityAI<Guid>)
            || t.BaseType == typeof(BHEntityAI<string>)).ToList();
        }

        internal List<Type> GetBHEntitiesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new();
            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && (t.BaseType == typeof(BHEntityAI<int>)
                || t.BaseType == typeof(BHEntityAI<Guid>)
                || t.BaseType == typeof(BHEntityAI<string>))).ToList());
            }
            return types;
        }

        internal List<Type> GetOpenAllBHEntities(Assembly ass)
        {
            return ass.GetTypes().Where(x => x.BaseType != null && x.BaseType.GetGenericTypeDefinition() == typeof(BHEntity<>)).ToList();
        }

        internal List<Type> GetOpenBHEntitiesInNamespaces(List<string> nameSpaces, Assembly ass)
        {
            List<Type> types = new();
            foreach (string nameSpace in nameSpaces)
            {
                types.AddRange(ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)
                && t.BaseType != null && t.BaseType.GetGenericTypeDefinition() == typeof(BHEntity<>)).ToList());
            }
            return types;
        }

        internal List<Type> GetAllServices(Assembly ass)
        {
            return ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleScoped) 
                || t.BaseType == typeof(BlackHoleSingleton) || t.BaseType == typeof(BlackHoleTransient)).ToList();
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
