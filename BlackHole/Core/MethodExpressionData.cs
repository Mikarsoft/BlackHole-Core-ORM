using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    internal class MethodExpressionData
    {
        internal List<object?> MethodArguments { get; set; } = new List<object?>();
        internal Expression? CastedOn { get; set; }
        internal string MethodName { get; set; } = string.Empty;
    }
}
