using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Connector
{
    public class BHExpressionPart
    {
        public BHExpressionPart(byte letter)
        {
            Letter = letter;
        }

        public byte Letter { get;}
        public string? LeftName { get; set; }
        public string? RightName { get; set; }
        public object? Value { get; set; }
        public BHStatement? LeftMethod { get; set; }
        public BHStatement? RightMethod { get; set; }
        public int OpenParenthesis { get; set; }
        public int CLoseParenthesis { get; set; }
        public ExpressionType ExpType { get; set; }

        public bool IsConnector
        {
            get
            {
                return ExpType == ExpressionType.AndAlso || ExpType == ExpressionType.OrElse;
            }
        }

        public bool HasLeftMethod
        {
            get
            {
                return LeftMethod != null;
            }
        }

        public bool HasRightMethod
        {
            get
            {
                return RightMethod != null;
            }
        }
    }
}
