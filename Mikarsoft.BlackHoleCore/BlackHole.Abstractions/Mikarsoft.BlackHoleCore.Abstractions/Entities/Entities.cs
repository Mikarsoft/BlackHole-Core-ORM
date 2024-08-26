namespace Mikarsoft.BlackHoleCore.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHistoricEntity<T> where T : BHEntity<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public void HistoricSettings();
    }

    /// <summary>
    /// Is Required to use Mapping on DTO, Views and Joins Functionality and 
    /// selecting specific columns of BHOpenEntities.
    /// </summary>
    public class BHDto
    {
    }

    /// <summary>
    /// Flexible Black Hole Entity with more options. The table in database is based on this.
    /// <para>It is Suggested for more advanced developers.</para>
    /// </summary>
    public abstract class BHEntity<Self> where Self : BHEntity<Self>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public abstract EntitySettings<Self> EntityOptions(EntityOptionsBuilder<Self> builder);

    }

    /// <summary>
    /// Is Required to use Mapping on DTO, Views and Joins Functionality
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public class BHDto<G> : BHDto where G : struct, IBHStruct
    {
        /// <summary>
        /// The Primary Key of the DTO
        /// </summary>
        public G Id { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public class BHEntityAI<T, G> : BHEntity<T> where T : BHEntityAI<T, G> where G : struct, IBHStruct
    {
        /// <summary>
        /// The Primary Key of the Entity
        /// </summary>
        public G Id { get; set; }

        internal int Inactive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override EntitySettings<T> EntityOptions(EntityOptionsBuilder<T> builder)
        {
            return builder.ProviderAutoHandlePrimaryKey(x => x.Id);
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
