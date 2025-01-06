using Mikarsoft.BlackHoleCore.Connector.Enums;

namespace Mikarsoft.BlackHoleCore.Connector.Statements
{
    public class BHRequestModel
    {
        public BHRequestModel(BHExpressionPartType command, List<JoinStatement> joins)
        {
            CommandType = command;
            Joins = joins;
        }

        public BHExpressionPartType CommandType { get; private set; }

        public List<JoinStatement> Joins { get; private set; }
    }
}
