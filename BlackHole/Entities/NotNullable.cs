
namespace BlackHole.Entities
{
    /// <summary>
    /// It turns the property to a Non Nullable Column in the Table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotNullable : Attribute
    {
        /// <summary>
        /// Nullability boolean
        /// </summary>
        public bool Nullability { get; } = false;
    }
}
