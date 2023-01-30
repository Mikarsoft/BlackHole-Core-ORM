
namespace BlackHole.Interfaces
{
    internal interface IBHTableBuilder
    {

        /// <summary>
        /// Build a Table using a List of BlazarEntity or BlazarEntityWithActivator Types. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableTypes"></param>
        void BuildMultipleTables(List<Type> TableTypes);

        /// <summary>
        /// Build a Table using a BlazarEntity or BlazarEntityWithActivator Type. 
        /// Constraints Are Handled Automatically. If the Table Already Exists it gets Ignored or Updated.
        /// </summary>
        /// <param name="TableType"></param>
        void BuildTable(Type TableType);

        /// <summary>
        /// Insert a List of all Entities you want to use and if there are Tables in the Database from a Previous version
        /// that are not Included in your new List of Entities, it automatically drops those tables from the Database.
        /// </summary>
        /// <param name="UsedTables"></param>
        void DropUnusedTables(List<Type> UsedTables);
    }
}
