
using System.Linq.Expressions;

namespace BlackHole.Core
{
    internal class ColumnsAndParameters
    {
        internal string Columns { get; set; } = "";
        internal BHParameters Parameters { get; set; } = new BHParameters();
        internal int Count { get; set; }
        internal List<MethodCallExpression> InvokeMethods { get; set; } = new List<MethodCallExpression>();
    }

    internal class ColumnAndParameter
    {
        internal string? Column { get; set; }
        internal string? ParamName { get; set; }
        internal object? Value { get; set; }
        internal MethodCallExpression? InvokeMethod { get; set; }
    }
}
