using Mikarsoft.BlackHoleCore.Entities;

namespace BlackHoleDIBench.Entities
{
    public class TestModelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public BHCollection<TestChildModel> ChildrenList { get; set; }
    }
}
