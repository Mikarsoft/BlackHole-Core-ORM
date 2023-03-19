using System.Linq.Expressions;

namespace BlackHole.Core
{
    internal class ExpressionsData
    {
        internal BinaryExpression? operation { get; set; }
        public MemberExpression? leftMember { get; set; }
        internal MemberExpression? rightMember { get; set; }
        internal ExpressionType expressionType { get; set; }
        internal object? memberValue { get; set; }
        internal bool rightChecked { get; set; }
        internal bool leftChecked { get; set; }
        internal int parentIndex { get; set; }
        internal string sqlCommand { get; set; } = "";
    }
}
