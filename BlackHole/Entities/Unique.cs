
namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Unique : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsUnique { get; }
        /// <summary>
        /// 
        /// </summary>
        public int PairId { get; }

        /// <summary>
        /// 
        /// </summary>
        public Unique()
        {
            IsUnique = true;
            PairId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniquePairId"></param>
        public Unique(int uniquePairId)
        {
            IsUnique = true;
            PairId = uniquePairId;
        }
    }
}
