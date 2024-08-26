
namespace Mikarsoft.BlackHoleCore.Connector.Statements
{
    public class WhereStatement
    {
        public WhereStatement(BHExpressionPart[] parts)
        {
            ExpressionParts = parts;
        }

        public BHExpressionPart[] ExpressionParts { get; set; }
    }
}
