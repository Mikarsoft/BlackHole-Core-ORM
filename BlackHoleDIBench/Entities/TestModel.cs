using Mikarsoft.BlackHoleCore.Entities;

namespace BlackHoleDIBench.Entities
{
    public class TestModel : BHEntityAI<TestModel, Int>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public BHCollection<TestChildModel> Children { get; set; }
    }
}
