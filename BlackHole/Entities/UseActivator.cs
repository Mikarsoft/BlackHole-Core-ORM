namespace BlackHole.Entities
{
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
}
