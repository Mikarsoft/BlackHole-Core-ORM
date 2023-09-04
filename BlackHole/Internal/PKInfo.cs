
namespace BlackHole.Internal
{
    internal class PKInfo
    {
        internal string MainPrimaryKey { get; set; } = string.Empty;
        internal bool HasAutoIncrement { get; set; }
        internal List<string> PKPropertyNames { get; set; } = new();
    }
}
