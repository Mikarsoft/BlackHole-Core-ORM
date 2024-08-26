using Mikarsoft.BlackHoleCore.Connector;
using System.Linq.Expressions;


namespace Mikarsoft.BlackHoleCore
{
    internal class BHExpressionData
    {
        internal BinaryExpression? Operation { get; set; }
        internal MethodCallExpression? LeftMethodMember { get; set; }
        internal MethodCallExpression? RightMethodMember { get; set; }
        internal MemberExpression? LeftMember { get; set; }
        internal MemberExpression? RightMember { get; set; }
        internal List<MethodExpressionData> MethodData { get; set; } = new();
        internal ExpressionType OperationType { get; set; }
        internal object? MemberValue { get; set; }
        internal bool MethodChecked { get; set; }
        internal bool RightChecked { get; set; }
        internal bool LeftChecked { get; set; }
        internal int ParentIndex { get; set; } = -1;
        internal int FinalPosition { get; set; } = -1;
        internal int OpenParenthesis { get; set; }
        internal int CloseParenthesis { get; set; }
        internal bool IsNullValue { get; set; }

        internal bool IsConnector
        {
            get
            {
                return OperationType == ExpressionType.AndAlso || OperationType == ExpressionType.OrElse;
            }
        }

        internal bool IsBranch
        {
            get
            {
                return MemberValue == null && MethodData.Count == 0 && OperationType != ExpressionType.Default;
            }
        }

        internal bool IsEdge
        {
            get
            {
                return MemberValue != null || MethodData.Count > 0 || (MemberValue == null && IsNullValue);
            }
        }

        internal bool HasMethod
        {
            get
            {
                return MethodData.Count > 0;
            }
        }

        internal bool IsDoubleEdge
        {
            get
            {
                return MemberValue == null && LeftMember != null && RightMember != null && MethodData.Count == 0;
            }
        }

        public bool IsReversed
        {
            get
            {
                return LeftMember == null && RightMember != null;
            }
        }
    }
}
