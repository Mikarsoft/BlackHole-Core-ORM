namespace BlackHole.Entities
{
    /// <summary>
    /// Defines the primary key Column of the Entity
    /// and it is used only internally to the 'BlackHoleEntity' and 'BlackHoleEntityG'
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class PrimaryKey : Attribute
    {
        public bool IsPrimaryKey = true;
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableDisplayName : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public TableDisplayName(string tableName)
        {
            TableName = tableName;
        }
    }

    /// <summary>
    /// Creates unique constraint using this column, alone or
    /// with other columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Unique : Attribute
    {
        /// <summary>
        /// The Identifier of the group 
        /// of the Unique columns combination
        /// </summary>
        public int UniqueGroupId { get; }

        /// <summary>
        /// Creates unique constraint using this column, alone or
        /// with other columns. 
        /// </summary>
        public Unique()
        {
            UniqueGroupId = 0;
        }

        /// <summary>
        /// Creates unique constraint using this column, alone or
        /// with other columns.
        /// </summary>
        /// <param name="groupId">The unique columns group Id</param>
        public Unique(int groupId)
        {
            if (groupId < 1)
            {
                groupId = 1;
            }
            UniqueGroupId = groupId;
        }
    }

    /// <summary>
    /// Using this over a class, then the Column isActive of the entity will
    /// be used and instead of deleting the Entries, it will be setting them
    /// as inactive ,every time you preform a delete.
    /// Inactive entries are ignored by all commands
    /// and can only be accessed with the methods 
    /// 'GetAllInactiveEntries' and 'DeleteInactiveEntryById'
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UseActivator : Attribute
    {
        /// <summary>
        /// use inactive column as flag
        /// </summary>
        public bool useActivator = true;
    }

    /// <summary>
    /// Specifies the Size of a Varchar column in the database
    /// The default size is 255
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class VarCharSize : Attribute
    {
        /// <summary>
        /// Character Length of the char column
        /// </summary>
        public int Charlength { get; set; }

        /// <summary>
        /// Specifies the Size of a Varchar column in the database
        /// The default size is 255
        /// </summary>
        /// <param name="Characters">The number of Characters. Varchar(n)</param>
        public VarCharSize(int Characters)
        {
            Charlength = Characters;
        }

        /// <summary>
        /// Specifies the Size of a Varchar column in the database
        /// to the default size 255
        /// </summary>
        public VarCharSize()
        {
            Charlength = 255;
        }
    }

    /// <summary>
    /// Sets Foreign Key for this Column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignKey : Attribute
    {
        /// <summary>
        /// Name of the Foreign Table
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Name of the column
        /// </summary>
        public string Column { get; set; }


        /// <summary>
        /// This Overload of the Constructor Sets by Default the corresponding column
        /// on the Primary Table as Id. You Can choose the Primary Table and
        /// if the Foreign Key is Nullable
        /// </summary>
        /// <param name="table">Type of the parent Table</param>
        public ForeignKey(Type table)
        {
            TableName = table.Name;
            Column = "Id";

            //if (isNullable)
            //{
            //    CascadeInfo = "on delete set null";
            //}
            //else
            //{
            //    CascadeInfo = "on delete cascade";
            //}
        }

        /// <summary>
        /// Set the Column as Foreign Key that points to specific Table and Column.
        /// </summary>
        /// <param name="table">Type of the other Table</param>
        /// <param name="columnName">Name of the other Table's Column</param>
        public ForeignKey(Type table, string columnName)
        {
            TableName = table.Name;
            Column = columnName;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnDisplayName : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        public ColumnDisplayName(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
