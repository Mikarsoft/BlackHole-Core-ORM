using BlackHole.Attributes.ColumnAttributes;

namespace BlackHole.Entities
{
    public abstract class BlackHoleEntityG
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        internal int Inactive { get; set; }
    }
}
