using BlackHole.Entities;

namespace BlackHole.Interfaces
{
    public interface IBlackHoleViewStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        IList<Dto> ExecuteView<Dto>() where Dto : BlackHoleDto;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <returns></returns>
        IList<Dto> ExecuteGView<Dto>() where Dto : BlackHoleGDto;
    }
}
