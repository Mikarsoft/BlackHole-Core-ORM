
namespace BlackHole.Entities
{
    internal class LiteTableInfo
    {
        internal int cid { get; set; }
        internal string name { get; set; } = string.Empty;
        internal string type { get; set; } = string.Empty;
        internal bool notnull { get; set; }
        internal string dflt_value { get; set; } = string.Empty;
        internal bool pk { get; set; }
    }
}
