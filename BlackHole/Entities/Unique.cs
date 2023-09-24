
namespace BlackHole.Entities
{
    /// <summary>
    /// Creates unique constraint using this column, alone or
    /// with other columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Unique : Attribute
    {
        /// <summary>
        /// The Identifier of the group 
        /// of the Unique columns combination
        /// </summary>
        public int UniqueGroupId { get;}

        /// <summary>
        /// Creates unique constraint using this column, alone or
        /// with other columns. 
        /// </summary>
        public Unique()
        {
            UniqueGroupId = 0;
        }

        /// <summary>
        /// Creates unique constraint using this column, alone or
        /// with other columns.
        /// </summary>
        /// <param name="groupId">The unique columns group Id</param>
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
