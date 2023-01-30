namespace BlackHole.Attributes.ColumnAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class PrimaryKey : Attribute
    {
        public bool IsPrimaryKey = true;
    }
}
