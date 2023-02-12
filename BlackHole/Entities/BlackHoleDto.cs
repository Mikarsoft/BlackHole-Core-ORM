
namespace BlackHole.Entities
{
    public abstract class BlackHoleDto<G> : BlackHoleDto
    {
        new public G? Id { get; set; }
    }

    public abstract class BlackHoleDto
    {
    }
}
