using BlackHole.Attributes.ColumnAttributes;

namespace BlackHole.Entities
{
    /// <summary>
    /// A Required Entity For Black Hole Data Provider
    /// It Contains an Integer Id Column as Primary Key
    /// </summary>
    public class BlackHoleEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        internal int Inactive { get; set; }
    }
}
