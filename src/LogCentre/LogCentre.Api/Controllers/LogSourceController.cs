using AutoMapper;

using LogCentre.Api.Attributes;
using LogCentre.Data;
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
            _logSourceService = logSourceService ?? throw new ArgumentNullException(nameof(logSourceService));
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
            catch (LogSourceException lse)
            {
                return HandleBadRequest("Error getting Log Source entries", lse.Message);
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

        [HttpGet("host/{id:long}"), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<LogSourceModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLogSourcesForHost([FromRoute] long id)
        {
            Logger.LogDebug("GetLogSourcesForHost()");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entries = await _logSourceService.GetAsync(l => l.HostId == id, l => l.OrderBy(x => x.Name), IncludeTables);
                var items = entries.ToList();

                var models = _mapper.Map<IList<LogSource>, IList<LogSourceModel>>(items);
                return Ok(models);
            }
            catch (LogSourceException lse)
            {
                return HandleBadRequest("Error getting Log Source entries for a host", lse.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", $"GetLogSourcesForHost() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetLogSourcesForHost took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Creates a new Log Source
        /// </summary>
        /// <param name="apiVersion">The route supplied API version</param>
        /// <param name="model">The <see cref="LogSourceModel">Log Source</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="LogSourceModel">Log Source</see></returns>
        [HttpPost, Benchmark]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LogSourceModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAsync(ApiVersion apiVersion, [FromBody] LogSourceModel model)
        {
            Logger.LogDebug("CreateAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                LogSource entity;
                if (model.Id > 0)
                {
                    if (_logSourceService.TryGet(model.Id, out entity))
                    {
                        if (entity != null && entity.Deleted == DataLiterals.Yes)
                        {
                            return HandleConflictRequest("Log Source already exists", $"Log Source with Id [{entity.Id}] already exists");
                        }
                    }
                }

                entity = _mapper.Map<LogSourceModel, LogSource>(model);
                entity = await _logSourceService.CreateAsync(entity);

                model = _mapper.Map<LogSource, LogSourceModel>(entity);
                return CreatedAtAction(nameof(GetLogSourceById), new { id = entity.Id, version = $"{apiVersion}" }, model);
            }
            catch (LogSourceException he)
            {
                return HandleBadRequest("Error creating new LogSource", he.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", "CreateAsync() produced an exception: " + ex.Message, ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** CreateAsync took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion

        #region Put

        /// <summary>
        /// Update an existing LogSource
        /// </summary>
        /// <param name="model">The <see cref="LogSourceModel">LogSource</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="LogSourceModel">LogSource</see></returns>
        [HttpPut, Benchmark]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReplaceAsync([FromBody] LogSourceModel model)
        {
            Logger.LogDebug("ReplaceAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!_logSourceService.TryGet(model.Id, out var entity))
                {
                    return HandleNotFoundRequest("Invalid LogSource", "The LogSource cannot be located by Id");
                }

                entity.Name = model.Name;
                entity.HostId = model.HostId;
                entity.ProviderId = model.ProviderId;
                entity.Active = model.Active;
                entity.Deleted = model.Deleted;
                entity.LastUpdatedBy = model.LastUpdatedBy;

                await _logSourceService.UpdateAsync(entity);

                return NoContent();
            }
            catch (LogSourceException he)
            {
                return HandleBadRequest("Error updating LogSource", he.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", "ReplaceAsync() produced an exception: " + ex.Message, ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** ReplaceAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete a LogSource
        /// </summary>
        /// <param name="id">The LogSource id</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] long id)
        {
            Logger.LogDebug("DeletedAsync() | id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!_logSourceService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("LogSource not found", $"Unable to locate LogSource with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.Yes)
                {
                    return HandleNotFoundRequest("LogSource not found", $"LogSource already deleted with Id [{id}]");
                }

                await _logSourceService.DeleteAsync(entity);

                return NoContent();
            }
            catch (LogSourceException he)
            {
                return HandleBadRequest("Error deleting LogSource", he.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", "DeleteAsync() produced an exception: " + ex.Message, ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** DeleteAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Remove a LogSource and all its dependencies if it has been soft deleted
        /// </summary>
        /// <param name="id">The LogSource id</param>
        [HttpDelete("{id:long}/purge")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveAsync([FromRoute] long id)
        {
            Logger.LogDebug("RemoveAsync() | id[{0}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!_logSourceService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("LogSource not found", $"Unable to locate LogSource with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.No)
                {
                    return HandleNotFoundRequest("LogSource not found", $"LogSource not soft deleted with Id [{id}]");
                }

                await _logSourceService.RemoveAsync(entity);

                return NoContent();
            }
            catch (LogSourceException he)
            {
                return HandleBadRequest("Unable to remove LogSource", he.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", "RemoveAsync() produced an exception: " + ex.Message,
                    ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** RemoveAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion
    }
}
