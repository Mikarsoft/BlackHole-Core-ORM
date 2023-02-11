
using BlackHole.Attributes.ColumnAttributes;
using BlackHole.Enums;

namespace BlackHole.Entities
{
    public abstract class BlackHoleEntity<G> : BlackHoleEntity
    {
        [PrimaryKey]
        public G? Id { get; set; }
    }

    public abstract class BlackHoleEntity
    {
        internal int Inactive { get; set; }
    }
}
