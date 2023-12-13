
namespace BlackHole.CoreSupport
{
    /// <summary>
    /// Stores All the required data to perform the Joins in this class
    /// To get the result you need to Execute this object.
    /// </summary>
    /// <typeparam name="Dto">Returning Data Transfer Object</typeparam>
    public class JoinsData<Dto>
    {
        internal Type? BaseTable { get; set; }
        internal List<PropertyOccupation> OccupiedDtoProps { get; set; } = new List<PropertyOccupation>();
        internal List<TableLetters> TablesToLetters { get; set; } = new List<TableLetters>();
        internal List<string?> Letters { get; set; } = new List<string?>();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal List<BlackHoleParameter> DynamicParams { get; set; } = new List<BlackHoleParameter>();
        internal int HelperIndex { get; set; }
        internal bool isMyShit { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
    }

    internal class JoinsData
    {
        internal JoinsData(Type dtoType)
        {
            DtoType = dtoType;
        }

        internal Type DtoType { get; set; }
        internal Type? BaseTable { get; set; }
        internal List<PropertyOccupation> OccupiedDtoProps { get; set; } = new List<PropertyOccupation>();
        internal List<TableLetters> TablesToLetters { get; set; } = new List<TableLetters>();
        internal List<string?> Letters { get; set; } = new List<string?>();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal List<BlackHoleParameter> DynamicParams { get; set; } = new List<BlackHoleParameter>();
        internal int HelperIndex { get; set; }
        internal bool isMyShit { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
        internal string OrderByOptions { get; set; } = string.Empty;
    }

    /// <summary>
    /// Stores All the required data to perform the Joins in this class
    /// To get the result you need to Execute this object.
    /// </summary>
    /// <typeparam name="Dto">Data Transfer Object</typeparam>
    /// <typeparam name="Tsource">First Entity</typeparam>
    /// <typeparam name="TOther">Second Entity</typeparam>
    public class JoinsData<Dto, Tsource, TOther>
    {
        internal Type? BaseTable { get; set; }
        internal List<PropertyOccupation> OccupiedDtoProps { get; set; } = new List<PropertyOccupation>();
        internal List<TableLetters> TablesToLetters { get; set; } = new List<TableLetters>();
        internal List<string?> Letters { get; set; } = new List<string?>();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal List<BlackHoleParameter> DynamicParams { get; set; } = new List<BlackHoleParameter>();
        internal int HelperIndex { get; set; }
        internal bool isMyShit { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
    }

    internal class TableLetters
    {
        internal Type? Table { get; set; }
        internal string? Letter { get; set; }
        internal bool IsOpenEntity { get; set; }
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
