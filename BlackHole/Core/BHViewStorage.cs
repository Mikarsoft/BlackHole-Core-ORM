using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    /// <summary>
    /// A class that contains methods to get the Stored  Views of BlackHole
    /// </summary>
    public class BHViewStorage : IBHViewStorage
    {
 
        List<Dto> IBHViewStorage.ExecuteView<Dto>()
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                result = existingJoin.ExecuteQuery<Dto>();
            }

            return result;
        }

        List<Dto> IBHViewStorage.ExecuteView<Dto>(BHTransaction transaction)
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                result = existingJoin.ExecuteQuery<Dto>(transaction);
            }

            return result;
        }

        async Task<List<Dto>> IBHViewStorage.ExecuteViewAsync<Dto>()
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                result = await existingJoin.ExecuteQueryAsync<Dto>();
            }

            return result;
        }

        async Task<List<Dto>> IBHViewStorage.ExecuteViewAsync<Dto>(BHTransaction transaction)
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new();

            if (existingJoin != null)
            {
                result = await existingJoin.ExecuteQueryAsync<Dto>(transaction);
            }

            return result;
        }
    }
}
