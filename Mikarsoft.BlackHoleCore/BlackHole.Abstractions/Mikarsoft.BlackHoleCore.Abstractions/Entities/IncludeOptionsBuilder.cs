

using Mikarsoft.BlackHoleCore.Entities;

namespace Mikarsoft.BlackHoleCore.Abstractions.Entities
{
    public class IncludeOptionsBuilder<T> where T : BHEntity<T>, new()
    {

        public IncludeSettings<T> IncludeNone()
        {
            return new();
        }
    }

    public class IncludeSettings<T> where T: BHEntity<T>, new()
    {

    }
}
