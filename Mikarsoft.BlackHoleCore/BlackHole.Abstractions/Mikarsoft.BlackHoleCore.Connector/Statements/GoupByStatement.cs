using Mikarsoft.BlackHoleCore.Connector.Enums;

namespace Mikarsoft.BlackHoleCore.Connector.Statements
{
    internal class GoupByStatement
    {
        public GoupByStatement(string column , OrderByDirection direction)
        {
            Column = column;
            Direction = direction;
        }

        public OrderByDirection Direction { get; set; }
        public string Column {  get; set; }
    }
}
