using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    public class BHViewStorage : IBHViewStorage
    {
        /// <summary>
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        IList<Dto> IBHViewStorage.ExecuteView<Dto>() where Dto : class
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            IList<Dto> result = new List<Dto>();

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
        IList<Dto> IBHViewStorage.ExecuteView<Dto>(BlackHoleTransaction transaction) where Dto : class
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            IList<Dto> result = new List<Dto>();

            if (existingJoin != null)
            {
                result = existingJoin.ExecuteQuery<Dto>(transaction);
            }

            return result;
        }
    }
}
