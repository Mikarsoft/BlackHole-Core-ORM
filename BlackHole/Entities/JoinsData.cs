using Dapper;

namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public class JoinsData<Dto>
    {
        internal Type? BaseTable { get; set; }
        internal List<PropertyOccupation> OccupiedDtoProps { get;set; } = new List<PropertyOccupation>();
        internal List<TableLetters> TablesToLetters { get; set; } = new List<TableLetters>();
        internal List<string?> Letters { get; set; } = new List<string?>();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal DynamicParameters DynamicParams { get; set; } = new DynamicParameters();
        internal int HelperIndex { get; set; }
        internal bool isMyShit { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JoinsData
    {
        internal Type? DtoType { get; set; }
        internal Type? BaseTable { get; set; }
        internal List<PropertyOccupation> OccupiedDtoProps { get; set; } = new List<PropertyOccupation>();
        internal List<TableLetters> TablesToLetters { get; set; } = new List<TableLetters>();
        internal List<string?> Letters { get; set; } = new List<string?>();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal DynamicParameters DynamicParams { get; set; } = new DynamicParameters();
        internal int HelperIndex { get; set; }
        internal bool isMyShit { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public class JoinsData<Dto,Tsource, TOther>
    {
        internal Type? BaseTable { get; set; }
        internal List<PropertyOccupation> OccupiedDtoProps { get; set; } = new List<PropertyOccupation>();
        internal List<TableLetters> TablesToLetters { get; set; } = new List<TableLetters>();
        internal List<string?> Letters { get; set; } = new List<string?>();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal DynamicParameters DynamicParams { get; set; } = new DynamicParameters();
        internal int HelperIndex { get; set; }
        internal bool isMyShit { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
    }

    internal class TableLetters
    {
        internal Type? Table { get; set; }
        internal string? Letter { get; set; }
    }

    internal class PropertyOccupation
    {
        internal string PropName { get; set; } = string.Empty;
        internal Type? PropType { get; set; }
        internal bool Occupied { get; set; }
        internal string? TableLetter { get; set; }
        internal string? TableProperty { get; set; } = string.Empty;
        internal Type? TablePropertyType { get; set; }
        internal int WithCast { get; set; }
    }
}
