
namespace BlackHole.Entities
{
    internal class CascadeTree
    {
        internal List<DataConstraints> NonNullableConstraints { get; set; } = new List<DataConstraints>();
        internal int NonNullableCount { get; set; }
        internal int CurrentBranch { get; set; }
        internal string NullableCascade { get; set; } = string.Empty;
        internal string NonNullableCascade { get; set; } = string.Empty;
        internal List<int> AffectedIds { get; set; } = new List<int>();
    }
}
