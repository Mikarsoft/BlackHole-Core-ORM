

using BlackHole.Entities;
using BlackHole.Identifiers;

namespace BlackHole.Abstractions.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHTable<T> : IBHTableCommon<T> where T : BHEntity<T>
    {
    }

    public interface IBHTable<T, G> : IBHTableCommon<T> where T : BHEntityAI<G> where G : IComparable<G>
    {

    }

    public interface IBHTableCommon<T> where T: BHEntityIdentifier
    {

    }
}
