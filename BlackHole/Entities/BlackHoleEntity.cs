

namespace BlackHole.Entities
{
    /// <summary>
    /// Black Hole Entity. The table is database is based on this
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public abstract class BlackHoleEntity<G> : BlackHoleEntity
    {
        [PrimaryKey]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public G Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public abstract class BlackHoleEntity
    {
        internal int Inactive { get; set; }
    }
}
