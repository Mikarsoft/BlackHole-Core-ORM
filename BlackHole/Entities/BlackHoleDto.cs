using BlackHole.Identifiers;

namespace BlackHole.Entities
{
    /// <summary>
    /// Is Required to use Mapping on DTO, Views and Joins Functionality
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public abstract class BlackHoleDto<G> : BHDtoIdentifier
    {
        /// <summary>
        /// The Primary Key of the DTO
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public G Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
