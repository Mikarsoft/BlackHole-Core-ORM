
namespace BlackHole.Entities
{
    public abstract class BlackHoleDto<G> : BlackHoleDto
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public G Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public abstract class BlackHoleDto
    {
    }
}
