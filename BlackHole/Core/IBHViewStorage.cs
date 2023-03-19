using BlackHole.CoreSupport;
using BlackHole.Entities;

namespace BlackHole.Core
{
    public interface IBHViewStorage
    {
        /// <summary>
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        IList<Dto> ExecuteView<Dto>() where Dto : BlackHoleDto;

        /// <summary>
        /// Transaction. Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        IList<Dto> ExecuteView<Dto>(BHTransaction transaction) where Dto : BlackHoleDto;
    }
}
