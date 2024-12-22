using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Connector
{
    public class MethodExpressionData
    {
        public List<object?> MethodArguments { get; set; } = [];
        public Expression? CastedOn { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public ExpressionType OperatorType { get; set; }
        public object? ComparedValue { get; set; }
        public MemberExpression? CompareProperty { get; set; }
        public bool ReverseOperator { get; set; }
        public string? TableName { get; set; } = string.Empty;
    }
}
