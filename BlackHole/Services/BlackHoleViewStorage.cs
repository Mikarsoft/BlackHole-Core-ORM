using BlackHole.Entities;
using BlackHole.Interfaces;
using BlackHole.Statics;

namespace BlackHole.Services
{
    public class BlackHoleViewStorage : IBlackHoleViewStorage
    {
        /// <summary>
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        IList<Dto> IBlackHoleViewStorage.ExecuteView<Dto>() where Dto : class
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
        IList<Dto> IBlackHoleViewStorage.ExecuteView<Dto>(BHTransaction transaction) where Dto : class
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
