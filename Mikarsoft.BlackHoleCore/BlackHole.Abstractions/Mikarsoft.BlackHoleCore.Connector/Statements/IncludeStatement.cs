

namespace Mikarsoft.BlackHoleCore.Connector.Statements
{

    internal class IncludeStatement
    {
        public IncludeStatement(Type paretnType , Type childType)
        {
            ChildType = childType;
            ParentType = paretnType;
        }

        public Type ChildType { get; set; }

        public Type ParentType { get; set; }
    }
}
