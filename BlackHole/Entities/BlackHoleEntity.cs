

namespace BlackHole.Entities
{
    /// <summary>
    /// Black Hole Entity. The table in database is based on this
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public abstract class BlackHoleEntity<G> : IBHEntity
    {
        /// <summary>
        /// The Primary Key of the Entity
        /// </summary>
        [PrimaryKey]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public G Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [DefaultValue(0)]
        internal int Inactive { get; set; }
    }
}
