
namespace Mikarsoft.BlackHoleCore.Connector.Statements
{
    public class WhereStatement
    {
        public WhereStatement(BHExpressionPart[] parts, byte tableCode)
        {
            ExpressionParts = parts;
            TableCode = tableCode;
        }

        public BHExpressionPart[] ExpressionParts { get; private set; }
        internal byte TableCode { get; private set; }
    }
}
