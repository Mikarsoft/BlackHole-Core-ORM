namespace BlackHole.Entities
{
    /// <summary>
    /// Defines the primary key Column of the Entity
    /// and it is used only internaly to the 'BlackHoleEntity' and 'BlackHoleEntityG'
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class PrimaryKey : Attribute
    {
        public bool IsPrimaryKey = true;
    }
}
