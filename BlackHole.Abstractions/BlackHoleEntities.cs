using BlackHole.Identifiers;

namespace BlackHole.Entities
{
    /// <summary>
    /// Is Required to use Mapping on DTO, Views and Joins Functionality and 
    /// selecting specific columns of BHOpenEntities.
    /// </summary>
    public abstract class BHOpenDto : BHDtoIdentifier
    {
    }

    /// <summary>
    /// Flexible Black Hole Entity with more options. The table in database is based on this.
    /// <para>It is Suggested for more advanced developers.</para>
    /// </summary>
    public abstract class BHOpenEntity<Self> : BHEntityIdentifier where Self : BHOpenEntity<Self>
    {
        /// <summary>
        /// Configuration method for the BHOpenEntity.
        /// <para><b>Important</b> => The result of this method is Required. All 'builder' methods will return a Settings Object.</para>
        /// </summary>
        /// <param name="builder">Entity's Settings Builder</param>
        public abstract EntitySettings<Self> EntityOptions(EntityOptionsBuilder<Self> builder);
    }

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

    /// <summary>
    /// Black Hole Entity. The table in database is based on this
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public abstract class BlackHoleEntity<G> : BHEntityIdentifier where G : IComparable<G>
    {
        /// <summary>
        /// The Primary Key of the Entity
        /// </summary>
        [PrimaryKey]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public G Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        internal int Inactive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SetId(G? id)
        {
            if (id != null)
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

    /// <summary>
    /// Turns a Class into Value Generator, that it can be used
    /// on a BHOpenEntity, to automatically generate the value of
    /// a column on the InsertMethods.
    /// </summary>
    public interface IBHValueGenerator<T> where T : IComparable<T>
    {
        /// <summary>
        /// The method that is called on the insert and returns the
        /// generated value.
        /// </summary>
        /// <returns>Generated Value</returns>
        T GenerateValue();
    }
}
