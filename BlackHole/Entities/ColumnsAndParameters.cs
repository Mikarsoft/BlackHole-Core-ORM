
using Dapper;

namespace BlackHole.Entities
{
    internal class ColumnsAndParameters
    {
        public string Columns { get; set; } = "";
        public DynamicParameters Parameters { get; set; } = new DynamicParameters();
        public int Count { get; set; }
    }

    internal class ColumnAndParameter
    {
        public string? Column { get; set; }
        public string? ParamName { get; set; }
        public object? Value { get; set; }
    }
}
