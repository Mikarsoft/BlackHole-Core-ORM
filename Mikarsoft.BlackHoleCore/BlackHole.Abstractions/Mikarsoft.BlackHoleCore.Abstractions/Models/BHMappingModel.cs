

namespace Mikarsoft.BlackHoleCore.Abstractions.Models
{
    internal class BHMappingModel
    {
        internal BHMappingModel(string sourceName, string targetName)
        {
            SourcePropertyName = sourceName;
            TargetPropertyName = targetName;
        }

        internal string SourcePropertyName { get; set; }

        internal string TargetPropertyName { get; set; }
    }
}
