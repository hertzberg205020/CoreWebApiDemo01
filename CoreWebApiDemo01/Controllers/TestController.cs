using EFCoreBooks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApiDemo01.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TestController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetCount()
        {
            var count = await _context.Set<Book>().CountAsync();
            throw new Exception("test");
            return Ok(count);
        }
    }
}
