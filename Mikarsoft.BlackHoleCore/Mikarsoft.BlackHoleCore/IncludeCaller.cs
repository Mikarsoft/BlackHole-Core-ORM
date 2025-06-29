using Mikarsoft.BlackHoleCore.Abstractions;

namespace Mikarsoft.BlackHoleCore
{
    internal class MIncludeCaller : IMIncludeCaller
    {
        public Task<T?> GetItemAsync<T>(string column, object? value, IBHTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetItemsAsync<T>(string column, object? value, IBHTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
