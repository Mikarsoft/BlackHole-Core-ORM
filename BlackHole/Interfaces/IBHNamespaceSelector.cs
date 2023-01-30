
using System.Reflection;

namespace BlackHole.Interfaces
{
    internal interface IBHNamespaceSelector
    {
        List<Type> GetAllBlazarEntities();
        List<Type> GetBlazarEntitiesInNamespace(string nameSpace);
        List<Type> GetBlazarEntitiesInNamespaces(List<string> nameSpaces);

        List<Type> GetAllBlazarEntities(Assembly ass);
        List<Type> GetBlazarEntitiesInNamespace(string nameSpace, Assembly ass);
        List<Type> GetBlazarEntitiesInNamespaces(List<string> nameSpaces, Assembly ass);

        List<Type> GetAllBlazarServices();
        List<Type> GetBlazarServicesInNamespace(string nameSpace);
        List<Type> GetBlazarServicesInNamespaces(List<string> nameSpaces);

        List<Type> GetAllBlazarServices(Assembly ass);
        List<Type> GetBlazarServicesInNamespace(string nameSpace, Assembly ass);
        List<Type> GetBlazarServicesInNamespaces(List<string> nameSpaces, Assembly ass);
    }
}
