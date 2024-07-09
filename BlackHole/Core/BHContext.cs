using BlackHole.Entities;

namespace BlackHole.Core
{
    internal class BHContext : BHContextBase, IBHContext
    {
        public IBHTable<T, G> Table<T, G>()
            where T : BHEntityAI<G>
            where G : IComparable<G>
        {
            throw new NotImplementedException();
        }

        IBHTable<T> IBHContext.Table<T>()
        {
            throw new NotImplementedException();
        }
    }
}
