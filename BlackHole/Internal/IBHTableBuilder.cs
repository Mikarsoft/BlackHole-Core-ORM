namespace BlackHole.Internal
{
    internal interface IBHTableBuilder
    {
        /// <summary>
        /// Build many Tables using a List of BlackHole Entities. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableTypes">Entities</param>
        void BuildMultipleTables(List<Type> TableTypes);

        /// <summary>
        /// Build a Table using a BlackHole Entities. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableType">Entity</param>
        void BuildTable(Type TableType);
    }
}
