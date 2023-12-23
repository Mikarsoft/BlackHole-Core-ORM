

using BlackHole.Identifiers;

namespace BlackHole.Entities
{
    /// <summary>
    /// Black Hole Entity. The table in database is based on this
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public abstract class BlackHoleEntity<G> : IBHEntityIdentifier where G : IComparable<G>
    {
        /// <summary>
        /// The Primary Key of the Entity
        /// </summary>
        [PrimaryKey]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public G Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        internal int Inactive { get; set; }

        internal bool SetId(G? id)
        {
            if(id != null)
            {
                Id = id;

                if (typeof(G) == typeof(int))
                {
                    object value = id;
                    return (int)value != 0;
                }

                if (typeof(G) == typeof(Guid))
                {
                    object value = id;
                    return (Guid)value != Guid.Empty;
                }

                if (typeof(G) == typeof(string))
                {
                    object value = id;
                    return (string)value != string.Empty;
                }
            }
            return false;
        }
    }
}
