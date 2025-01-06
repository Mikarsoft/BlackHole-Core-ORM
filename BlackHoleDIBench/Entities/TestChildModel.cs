using Mikarsoft.BlackHoleCore.Entities;

namespace BlackHoleDIBench.Entities
{
    public class TestChildModel : BHEntityAI<TestChildModel, Int>
    {
        public string ItemName { get; set; } = string.Empty;
        public int ItemCount { get; set; }

        public DateTimeOffset OrderDate { get; set; }
    }
}
