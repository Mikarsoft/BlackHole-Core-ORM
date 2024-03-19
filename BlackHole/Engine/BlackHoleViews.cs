namespace BlackHole.Engine
{
    internal static class BlackHoleViews
    {
        internal static List<JoinsData> Stored { get; set; } = new List<JoinsData>();
    }

    internal static class BlackHoleStoredProcedures
    {
        internal static List<SPData> Stored { get; set; } = new List<SPData>();
    }
}
