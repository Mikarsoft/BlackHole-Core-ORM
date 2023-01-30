using System.Linq.Expressions;

namespace BlackHole.Entities
{
    public class ExpressionsData
    {
        public BinaryExpression? operation { get; set; }
        public MemberExpression? leftMember { get; set; }
        public MemberExpression? rightMember { get; set; }
        public ExpressionType expressionType { get; set; }
        public object? memberValue { get; set; }
        public bool rightChecked { get; set; }
        public bool leftChecked { get; set; }
        public int parentIndex { get; set; }
        public string sqlCommand { get; set; } = "";
    }
}
