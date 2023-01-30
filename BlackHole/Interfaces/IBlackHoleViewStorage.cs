using BlackHole.Entities;

namespace BlackHole.Interfaces
{
    public interface IBlackHoleViewStorage
    {
        IList<Dto> ExecuteView<Dto>() where Dto : BlackHoleDto;
        IList<Dto> ExecuteGView<Dto>() where Dto : BlackHoleGDto;
    }
}
