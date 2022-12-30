using LogCentre.Api.Attributes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace LogCentre.Api.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class HomeController : BaseApiController<HomeController>
    {
        /// <summary>
        /// Constructor for Home
        /// </summary>
        /// <param name="logger">Logger implementation for home</param>
        public HomeController(ILogger<HomeController> logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns>Empty Json result</returns>
        [HttpGet(Name = nameof(Index)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Index()
        {
            return Ok("Ping");
        }

        /// <summary>
        /// Default hello endpoint
        /// </summary>
        /// <returns>string</returns>
        [HttpGet("say-hello"), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult SayHello()
        {
            return Ok("Hello, everybody!");
        }

        /// <summary>
        /// Returns environmental data
        /// </summary>
        /// <returns>JSON environment details</returns>
        [HttpGet("env"), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Environment()
        {
            IWebHostEnvironment? hostEnvironment = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
            if (hostEnvironment == null)
            {
                return BadRequest("Invalid Contnext");
            }

            var thisEnv = new
            {
                ApplicationName = hostEnvironment.ApplicationName,
                Environment = hostEnvironment.EnvironmentName,
            };

            var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
            //await context.Response.WriteAsJsonAsync(thisEnv, jsonSerializerOptions);
            return new JsonResult(thisEnv, jsonSerializerOptions);
        }
    }
}
