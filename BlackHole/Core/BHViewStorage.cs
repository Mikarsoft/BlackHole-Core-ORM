using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    /// <summary>
    /// A class that contains methods to get the Stored  Views of BlackHole
    /// </summary>
    public class BHViewStorage : IBHViewStorage
    {
        /// <summary>
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        List<Dto> IBHViewStorage.ExecuteView<Dto>() where Dto : class
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new List<Dto>();

            if (existingJoin != null)
            {
                result = existingJoin.ExecuteQuery<Dto>();
            }

            return result;
        }

        /// <summary>
        /// Transaction. Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        List<Dto> IBHViewStorage.ExecuteView<Dto>(BHTransaction transaction) where Dto : class
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new List<Dto>();

            if (existingJoin != null)
            {
                result = existingJoin.ExecuteQuery<Dto>(transaction);
            }

            return result;
        }

        /// <summary>
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        async Task<List<Dto>> IBHViewStorage.ExecuteViewAsync<Dto>() where Dto : class
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new List<Dto>();

            if (existingJoin != null)
            {
                result = await existingJoin.ExecuteQueryAsync<Dto>();
            }

            return result;
        }

        /// <summary>
        /// Transaction. Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        async Task<List<Dto>> IBHViewStorage.ExecuteViewAsync<Dto>(BHTransaction transaction) where Dto : class
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            List<Dto> result = new List<Dto>();

            if (existingJoin != null)
            {
                result = await existingJoin.ExecuteQueryAsync<Dto>(transaction);
            }

            return result;
        }
    }
}
