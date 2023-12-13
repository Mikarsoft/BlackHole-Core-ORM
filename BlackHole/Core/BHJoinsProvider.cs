
namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BHJoinsProvider : IBHJoinsProvider
    {
        JoinsProcess<Dto> IBHJoinsProvider.Using<Dto>()
        {
            return new JoinsProcess<Dto>();
        }
    }
}
