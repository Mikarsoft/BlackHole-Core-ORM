namespace BlackHole.Internal
{
    public class SqLiteTableInfo
    {
        public int cid { get; set; }
        public string name { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public bool notnull { get; set; }
        public string dflt_value { get; set; } = string.Empty;
        public bool pk { get; set; }
    }
}
