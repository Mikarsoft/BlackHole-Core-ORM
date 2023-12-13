
namespace BlackHole.Core
{
    public interface IBHJoinsProvider
    {
        JoinsProcess<Dto> Using<Dto>();
    }
}
