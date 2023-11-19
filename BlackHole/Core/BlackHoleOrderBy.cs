
namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlackHoleOrderBy<T>
    {
        internal List<OrderByPair> OrderProperties {  get; set; }
        internal BlackHoleOrderBy()
        {
            OrderProperties = new();
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
