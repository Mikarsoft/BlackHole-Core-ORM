using System.Linq.Expressions;

namespace BlackHole.CoreSupport
{
    internal class MethodExpressionData
    {
        internal List<object?> MethodArguments { get; set; } = new List<object?>();
        internal Expression? CastedOn { get; set; }
        internal string MethodName { get; set; } = string.Empty;
        internal ExpressionType OperatorType { get; set; }
        internal object? ComparedValue { get; set; }
        internal MemberExpression? CompareProperty { get; set; }
        internal bool ReverseOperator { get; set; }
        internal string TableName { get; set; } = string.Empty;
    }
}
