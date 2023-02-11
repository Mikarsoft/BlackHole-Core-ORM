
using BlackHole.Attributes.ColumnAttributes;

namespace BlackHole.Entities
{
    public abstract class BlackHoleEntity<G> : BlackHoleEntity
    {
        [PrimaryKey]
        public new G? Id { get; set; }
    }

    public abstract class BlackHoleEntity
    {
        [PrimaryKey]
        public int Id { get; set; }

        internal int Inactive { get; set; }
    }
}
