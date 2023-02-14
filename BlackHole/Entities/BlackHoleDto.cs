
namespace BlackHole.Entities
{
    public abstract class BlackHoleDto<G> : BlackHoleDto
    {
        public G Id { get; set; }
    }

    public abstract class BlackHoleDto
    {
    }
}
