using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthBearer.Controllers
{
    [Authorize]
    [Route("api/test")]
    public class TestController : Controller
    {
        
        [HttpGet]
        public string Get()
        {
            return "OK";
        }
    }
}