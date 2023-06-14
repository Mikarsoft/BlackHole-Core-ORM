
namespace BlackHole.Internal
{
    internal class ColumnScanResult
    {
        internal bool UnidentifiedColumn { get; set; }
        internal string UsingReference { get; set; } = string.Empty;
        internal string PropertyNameForColumn { get; set; } = string.Empty;
    }
}
