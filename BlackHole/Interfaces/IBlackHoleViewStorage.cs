using BlackHole.Entities;

namespace BlackHole.Interfaces
{
    public interface IBlackHoleViewStorage
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
        /// Executes the stored view that has the inserted DTO as
        /// Identifier. If there is no view stored with this DTO it returns
        /// an empty IList
        /// </summary>
        /// <typeparam name="Dto">Class that the view will be mapped</typeparam>
        /// <returns>IList of the DTO</returns>
        IList<Dto> ExecuteGView<Dto>() where Dto : BlackHoleGDto;
    }
}
