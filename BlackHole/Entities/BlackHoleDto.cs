
namespace BlackHole.Entities
{
    public abstract class BlackHoleDto<G> : BlackHoleDto
    {
        public new G? Id { get; set; }
    }

    public abstract class BlackHoleDto
    {
        public int Id { get; set; }
    }
}
