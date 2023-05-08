using System.Linq.Expressions;

namespace BlackHole.CoreSupport
{
    internal class ExpressionsData
    {
        internal BinaryExpression? operation { get; set; }
        internal MethodCallExpression? leftMethodMember { get; set; }
        internal MethodCallExpression? rightMethodMember { get; set; }
        public MemberExpression? leftMember { get; set; }
        internal MemberExpression? rightMember { get; set; }
        internal List<MethodExpressionData> methodData { get; set; } = new List<MethodExpressionData>();
        internal ExpressionType expressionType { get; set; }
        internal object? memberValue { get; set; }
        internal bool methodChecked { get; set; }
        internal bool rightChecked { get; set; }
        internal bool leftChecked { get; set; }
        internal int parentIndex { get; set; }
        internal string sqlCommand { get; set; } = "";
    }
}
