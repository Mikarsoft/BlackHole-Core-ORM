
namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlackHoleOrderBy<T>
    {
        internal List<OrderByPair> OrderProperties {  get; set; }
        internal bool LockedByError { get; set; }
        internal BlackHoleOrderBy(string propertyName, string orientation, bool lockedByError)
        {
            OrderProperties = new()
            {
                new OrderByPair { Oriantation = orientation, PropertyName = propertyName }
            };

            LockedByError = lockedByError;
        }

        internal void AddPair(string propertyName, string orientation)
        {
            OrderProperties.Add(new OrderByPair { Oriantation=orientation, PropertyName = propertyName });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlackHoleLimiter<T>
    {
        internal BlackHoleOrderBy<T> Ordering { get; set; }
        internal int Skip { get; set; }
        internal int Take {  get; set; }

        internal BlackHoleLimiter(BlackHoleOrderBy<T> incomingOrdering, int fromRow, int toRow) 
        { 
            Ordering = incomingOrdering;
            Skip = fromRow;
            Take = toRow;
        }
    }

    internal class OrderByPair
    {
        internal string PropertyName { get; set; } = string.Empty;
        internal string Oriantation { get; set; } = string.Empty;
    }
}
