using BlackHoleDIBench.Entities;
using Microsoft.AspNetCore.Mvc;
using Mikarsoft.BlackHoleCore;

namespace BlackHoleDIBench.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly IBHTable<TestModel> _table;

        public TestController(IBHTable<TestModel> table)
        {
            _table = table;
        }

        [HttpGet]
        public async Task TestingMethod()
        {
            await _table.Select<TestModelDTO>()
                .Map(x =>
                {
                    x.MapProperty(x => x.Name, z => z.Name);
                    x.MapProperty(x => x.Description, z => z.Description);
                    x.MapProperty<int>(x => x.Id, z => z.Id);
                })
                .OrderByDescending(x => x.Name)
                .TakeWithOffset(5, 10)
                .ToListAsync();
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
