using BlackHoleDIBench.Entities;
using Microsoft.AspNetCore.Mvc;
using Mikarsoft.BlackHoleCore;
using Mikarsoft.BlackHoleCore.Entities;

namespace BlackHoleDIBench.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly IBHTable<TestModel, Int> _table;

        public TestController(IBHTable<TestModel, Int> table)
        {
            _table = table;
        }

        [HttpGet]
        public async Task TestingMethod()
        {
            var test = await _table.Select<TestModelDTO>()
                .Include(x => x.Children)
                .GroupBy(x => new { x.Name, x.Description })
                .Map(m => new TestModelDTO()
                {
                    Id = m.Select.Max(m => m.Id),
                    Name = m.Key.Name,
                    Description = m.Key.Description,
                    ChildrenList = m.First.ChildrenList
                })
                .OrderByAscending(x => x.Name)
                .TakeWithOffset(10, 20)
                .ToListAsync();
                //.Map(x =>
                //{
                //    x.MapProperty(x => x.Name, z => z.Name);
                //    x.MapProperty(x => x.Description, z => z.Description);
                //    x.MapProperty<int>(x => x.Id, z => z.Id);
                //})
                //.OrderByDescending(x => x.Name)
                //.TakeWithOffset(5, 10)
                //.ToListAsync();

            var test2 = new TestModel();

            _table.UpdateById(test2).Execute();

            _table.GetById(5);

            _table.Update(test2).Where(x => x.Id == 6)
                .UseColumns(x =>
                {
                    x.Use(x => x.Name);
                    x.Use(x => x.Description);
                    x.Use(x => x.Id);
                })
                .Execute();

                //.InnerJoin<TestModel2>().On(x => x.Id.ToString(), x => x.Id.ToString())
                //.Include(x => x.ChildrenList).Match(x => x.Id.ToString(), x => x.Id.ToString())
                //.Where(x=> x.Id == 5).ToListAsync();
                //.InnerJoin<TestModel2>().On(x=>x.Id , x => x.Id).ToListAsync();
                //.Match(x => x.Id , d => d.Id)
                //.Include(x => x.Children).Match(x => x.Id , z => z.Id)
                //.Where(x => x.Id == 5)                
                //.GroupBy(x => new { x.Name, x.Description })
                //.Map(m => new TestModelDTO()
                //    {
                //        Description = m.First.Description,
                //        Name = m.Where(x => x.Name == "mpla").First.Name,
                //        Id = m.Select.Max(x => x.Id)
                //    })
                //.OrderByDescending(x => x.Name)
                //.Take(10)
                //.ToListAsync();
        }
    }
}
