using BlackHole.Attributes.ColumnAttributes;

namespace BlackHole.Entities
{
    /// <summary>
    /// A Required Entity For Black Hole Data Provider
    /// It Contains a Guid Id Column as Primary Key
    /// </summary>
    public abstract class BlackHoleEntityG
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        internal int Inactive { get; set; }
    }
}
