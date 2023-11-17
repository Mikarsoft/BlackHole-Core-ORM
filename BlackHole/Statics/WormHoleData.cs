namespace BlackHole.Statics
{
    internal static class WormHoleData
    {
        internal static string[]? ConnectionStrings {  get; set; }
        internal static string[]? DbSchemas { get; set; }
        internal static Type[]? Entities { get; set; }
        internal static int[]? Databases { get; set; }
        internal static int[][,]? EntitiesMap { get; set; }
    }
}
