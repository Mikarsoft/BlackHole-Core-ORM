

namespace Mikarsoft.BlackHoleCore.Tools
{
    internal class BHIncludeModel
    {
        public string ParentProperty { get; set; } = string.Empty;

        public string ChildProperty { get; set; } = string.Empty;

        internal GetIncludeAction IncludeAction { get; set; }

        public BHIncludeModel(GetIncludeAction includeAction)
        {
            IncludeAction = includeAction;
        }

        internal void Match(string parentProp, string childProp)
        {
            ParentProperty = parentProp;
            ChildProperty = childProp;
        }
    }
}
