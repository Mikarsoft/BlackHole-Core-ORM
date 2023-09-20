
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
        public int UniqueGroupId { get;}

        /// <summary>
        /// 
        /// </summary>
        public Unique()
        {
            UniqueGroupId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        public Unique(int groupId)
        {
            if(groupId < 1)
            {
                groupId = 1;
            }
            UniqueGroupId = groupId;
        }
    }
}
