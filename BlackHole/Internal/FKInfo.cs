
namespace BlackHole.Internal
{
    internal class FKInfo
    {
        internal string PropertyName { get; set; } = string.Empty;
        internal string ReferencedTable { get; set; } = string.Empty;
        internal string ReferencedColumn { get; set; } = string.Empty;
        internal bool IsNullable { get; set; }
    }
}
