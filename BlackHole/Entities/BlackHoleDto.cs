
namespace BlackHole.Entities
{
    /// <summary>
    /// A Data Transfer Object that is required for Joins 
    /// and DTO mapping. It Contains an Integer Id Column
    /// </summary>
    public abstract class BlackHoleDto
    {
        public int Id { get; set; }
    }
}
