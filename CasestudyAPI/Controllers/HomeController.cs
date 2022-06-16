using Microsoft.AspNetCore.Mvc;

namespace CasestudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Index()
        {
            return "The server is up and running!";
        }

    }
}
