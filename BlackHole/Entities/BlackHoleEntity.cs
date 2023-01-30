using BlackHole.Attributes.ColumnAttributes;

namespace BlackHole.Entities
{
    public class BlackHoleEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        internal int Inactive { get; set; }
    }
}
