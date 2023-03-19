using System.Reflection;

namespace BlackHole.Internal
{
    internal interface IBHNamespaceSelector
    {
        List<Type> GetAllBHEntities(Assembly ass);
        List<Type> GetBHEntitiesInNamespace(string nameSpace, Assembly ass);
        List<Type> GetBHEntitiesInNamespaces(List<string> nameSpaces, Assembly ass);


        List<Type> GetAllBHServices(Assembly ass);
        List<Type> GetBHServicesInNamespace(string nameSpace, Assembly ass);
        List<Type> GetBHServicesInNamespaces(List<string> nameSpaces, Assembly ass);
    }
}
