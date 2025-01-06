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
                .Include(x => x.Children).Match(x => x.Id , z => z.Id)
                .Where(x => x.Id == 5)                
                .GroupBy(x => new { x.Name, x.Description })
                .Map(m => new TestModelDTO()
                    {
                        Description = m.First.Description,
                        Name = m.Where(x => x.Name == "mpla").First.Name,
                        Id = m.Select.Max(x => x.Id)
                    })
                .OrderByDescending(x => x.Name)
                .Take(10)
                .ToListAsync();
        }
    }
}
