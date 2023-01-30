using BlackHole.Entities;
using BlackHole.ExtensionMethods;
using BlackHole.Interfaces;
using BlackHole.Statics;

namespace BlackHole.Services
{
    public class BlackHoleViewStorage : IBlackHoleViewStorage
    {
        public IList<Dto> ExecuteView<Dto>() where Dto : BlackHoleDto
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            IList<Dto> result = new List<Dto>();

            if (existingJoin != null)
            {
                result = existingJoin.ExecuteQuery<Dto>();
            }
            return result;
        }

        public IList<Dto> ExecuteGView<Dto>() where Dto : BlackHoleGDto
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.Where(x => x.DtoType == typeof(Dto)).FirstOrDefault();
            IList<Dto> result = new List<Dto>();

            if (existingJoin != null)
            {
                result = existingJoin.ExecuteGQuery<Dto>();
            }
            return result;
        }
    }
}
