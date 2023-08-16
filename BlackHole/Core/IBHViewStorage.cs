using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// An Interface that contains methods to let you get the
    /// stored Views of BlackHole
    /// </summary>
    public interface IBHViewStorage
    {
        /// <summary>
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        List<Dto> ExecuteView<Dto>() where Dto : BlackHoleDto;

        /// <summary>
        /// Transaction. Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        List<Dto> ExecuteView<Dto>(BHTransaction transaction) where Dto : BlackHoleDto;

        /// <summary>
        /// Asyncronous. Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        Task<List<Dto>> ExecuteViewAsync<Dto>() where Dto : BlackHoleDto;

        /// <summary>
        /// Asyncronous. Transaction. Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        Task<List<Dto>> ExecuteViewAsync<Dto>(BHTransaction transaction) where Dto : BlackHoleDto;
    }
}
