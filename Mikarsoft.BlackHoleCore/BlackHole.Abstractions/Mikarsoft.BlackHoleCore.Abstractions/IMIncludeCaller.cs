

namespace Mikarsoft.BlackHoleCore.Abstractions
{
    internal interface IMIncludeCaller
    {
        Task<List<T>> GetItemsAsync<T>(string column, object? value, IBHTransaction transaction);
    }
}
