using CoreWebApiDemo01.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoreWebApiDemo01.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public LoginResponse Login(LoginRequest req)
        {
            if (req.UserName == "admin" && req.Password == "123456")
            {
                var items = Process.GetProcesses().Select(p => 
                new ProcessInfo(p.Id,
                    p.ProcessName,
                    p.WorkingSet64));
                return new LoginResponse(true, items.ToArray());
            }
            return new LoginResponse(false, null);
        }
    }
}
