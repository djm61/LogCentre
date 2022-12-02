using AutoMapper;

using LogCentre.Api.Attributes;
using LogCentre.Data.Entities;
using LogCentre.Model;
using LogCentre.Services.Exceptions;
using LogCentre.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace LogCentre.Api.Controllers
{
    /// <summary>
    /// Controller for Log Source
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LogSourceController : BaseApiController<LogSourceController>
    {
        private const string IncludeTables = "Host,Provider";

        private readonly ILogSourceService _logSourceService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for Log Source
        /// </summary>
        /// <param name="logger">Logger implementation</param>
        /// <param name="logSourceService">Service for Log Sources</param>
        /// <param name="mapper">AutoMapper properties</param>
        /// <exception cref="ArgumentNullException">Throws if something is null</exception>
        public LogSourceController(ILogger<LogSourceController> logger,
            ILogSourceService logSourceService,
            IMapper mapper)
            : base(logger)
        {
            _logSourceService = logSourceService ?? throw new ArgumentNullException(nameof(_logSourceService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Get

        /// <summary>
        /// Gets a single Log Source line
        /// </summary>
        /// <param name="id">Id of Log Source</param>
        /// <returns>Log Source</returns>
        [HttpGet("{id:long}", Name = nameof(GetLogSourceById)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LogSourceModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLogSourceById([FromRoute] long id)
        {
            Logger.LogDebug("GetLogSourceById() | Id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entities = await _logSourceService.GetAsync(a => a.Id == id, null, IncludeTables);
                var entity = entities.FirstOrDefault();
                if (entity == null)
                {
                    return HandleNotFoundRequest("Log Source not found", $"Log Source with Id[{id}] was not found");
                }

                var model = _mapper.Map<LogSource, LogSourceModel>(entity);
                return Ok(model);
            }
            catch (LogSourceException lse)
            {
                return HandleBadRequest("Invalid Log Source Id", lse.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("And error has occurred",
                    $"GetLogSourceById() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetLogSourceById took [{0}]", stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Gets all Log Source lines
        /// </summary>
        /// <returns>List of <see cref="LogSourceModel">Log Sources</see></returns>
        [HttpGet("all", Name = nameof(GetLogSources)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<LogSourceModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLogSources()
        {
            Logger.LogDebug("GetLogSources()");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entries = await _logSourceService.GetAsync(null, a => a.OrderBy(b => b.Name),
                    IncludeTables);
                var items = entries.ToList();

                var models = _mapper.Map<IList<LogSource>, IList<LogSourceModel>>(items);
                return Ok(models);
            }
            catch (LogSourceException ale)
            {
                return HandleBadRequest("Error getting Log Source entries", ale.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred",
                    $"GetLogSources() produced an exception [{ex.Message}]",
                    ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetLogSources took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion
    }
}
