using LogCentre.Api.Attributes;

using Microsoft.AspNetCore.Mvc;

namespace LogCentre.Api.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Index page
        /// </summary>
        /// <returns>Empty Json result</returns>
        [HttpGet(Name = nameof(Index)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Index()
        {
            return Ok("Ping");
        }
    }
}
