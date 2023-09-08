namespace BlackHole.Entities
{
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
