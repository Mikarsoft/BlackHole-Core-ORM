using Mikarsoft.BlackHoleCore.Abstractions.Entities;
using Mikarsoft.BlackHoleCore.Entities;

namespace BlackHoleDIBench.Entities
{
    public class TestModel : BHEntityAI<TestModel, Int>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public BHCollection<TestChildModel, Int> Children { get; set; }

        public override IncludeSettings<TestModel> IncludeOptions(IncludeOptionsBuilder<TestModel> entity)
        {
            return entity.HasMany(x => x.Children).On<int>(x => x.Id, c => c.Id)
                         .HasMany(x => x.Children).On<int>(x => x.Id, c => c.Id);    
        }
    }
}