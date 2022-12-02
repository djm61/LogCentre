using LogCentre.Api.Models;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace LogCentre.Api.Controllers
{
    /// <summary>
    /// Base API Controller
    /// </summary>
    [ApiController]
    public class BaseApiController<T> : ControllerBase where T : BaseApiController<T>
    {
        /// <summary>
        /// Logger Factory
        /// </summary>
        protected ILoggerFactory _loggerFactory;

        /// <summary>
        /// Typed logger
        /// </summary>
        protected ILogger<T> _logger;

        /// <summary>
        /// Configuration
        /// </summary>
        protected IConfiguration _configuration;

        /// <summary>
        /// Constructor with a single <see cref="ILoggerFactory"/>
        /// </summary>
        /// <param name="loggerFactory">Logger Factory</param>
        /// <exception cref="ArgumentNullException">Throws if Logger Factory is null</exception>
        public BaseApiController(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<T>();
        }

        /// <summary>
        /// Constructer with a typed <see cref="ILogger"/>
        /// </summary>
        /// <param name="logger">Typed logger</param>
        /// <exception cref="ArgumentNullException">THrows if Logger is null</exception>
        public BaseApiController(ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles a default API error
        /// </summary>
        /// <returns>Status Code 500</returns>
        protected virtual IActionResult HandleDefaultApiError()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature != null)
            {
                var path = exceptionFeature.Path;
                var error = exceptionFeature.Error;
                Logger.LogError("{ControllerName}Controller.Error: Error in [{path}], info: [{error}]", ControllerName, path, error);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Handles a not found error
        /// </summary>
        /// <param name="message">Message to return</param>
        /// <param name="logMessage">Message to log</param>
        /// <returns>Status Code 404</returns>
        protected virtual IActionResult HandleNotFoundRequest(string message, string logMessage)
        {
            Logger.LogWarning("{logMessage}. Returning Not Found 404 Request.", logMessage);
            Response.ContentType = "application/json";
            var error = new ErrorResponse(StatusCodes.Status404NotFound, message);
            return NotFound(JsonSerializer.Serialize(error));
        }

        /// <summary>
        /// Handles a bad request error - 400 Bad Request
        /// </summary>
        /// <param name="message">Message to return</param>
        /// <param name="logMessage">Message to log</param>
        /// <returns>Status Code 400</returns>
        protected virtual IActionResult HandleBadRequest(string message, string logMessage)
        {
            Logger.LogWarning("{logMessage}. Returning Bad 400 Request.", logMessage);
            Response.ContentType = "application/json";
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, message);
            return BadRequest(JsonSerializer.Serialize(error));
        }

        /// <summary>
        /// Handles a conflict error
        /// </summary>
        /// <param name="message">Message to return</param>
        /// <param name="logMessage">Message to log</param>
        /// <returns>Status Code 409</returns>
        protected virtual IActionResult HandleConflictRequest(string message, string logMessage)
        {
            Logger.LogWarning("{logMessage}. Returning Bad 409 Conflict.", logMessage);
            Response.ContentType = "application/json";
            var error = new ErrorResponse(StatusCodes.Status409Conflict, message);
            return Conflict(JsonSerializer.Serialize(error));
        }

        /// <summary>
        /// Handles a server error - 400 Bad Request
        /// </summary>
        /// <param name="message">Message to return</param>
        /// <param name="logMessage">Message to log</param>
        /// <returns>Status Code 500</returns>
        protected virtual IActionResult HandleServerError(string message, string logMessage, Exception exception)
        {
            var traceId = Guid.NewGuid().ToString();
            Logger.LogError(exception, "{logMessage}. Returning Bad 400 Request. [TraceId = {traceId}]", logMessage, traceId);
            Response.ContentType = "application/json";
            var error = new ErrorResponse(StatusCodes.Status400BadRequest, message, traceId, exception, false);
            return BadRequest(JsonSerializer.Serialize(error));
        }

        /// <summary>
        /// Logger Factory property
        /// </summary>
        protected ILoggerFactory LoggerFactory => _loggerFactory ??= HttpContext?.RequestServices.GetService<ILoggerFactory>();

        /// <summary>
        /// Typed logger property
        /// </summary>
        protected ILogger<T> Logger => _logger ??= HttpContext?.RequestServices.GetService<ILogger<T>>();

        /// <summary>
        /// Configuration property
        /// </summary>
        protected IConfiguration Configuration => _configuration ??= HttpContext?.RequestServices.GetService<IConfiguration>();

        /// <summary>
        /// Controller name
        /// </summary>
        protected string ControllerName => ControllerContext?.ActionDescriptor?.ControllerName ?? GetType().Name;
    }
}
