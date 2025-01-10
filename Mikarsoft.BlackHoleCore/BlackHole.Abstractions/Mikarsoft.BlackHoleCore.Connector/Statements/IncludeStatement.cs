

namespace Mikarsoft.BlackHoleCore.Connector.Statements
{

    internal class IncludeStatement
    {
        public string ParentProperty { get; set; }

        public string ChildProperty { get; set; }

        public IncludeStatement(string parentProp, string childProp)
        {
            ParentProperty = parentProp;
            ChildProperty = childProp;
        }
    }
}
