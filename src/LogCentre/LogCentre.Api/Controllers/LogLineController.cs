using AutoMapper;

using LogCentre.Api.Attributes;
using LogCentre.Data;
using LogCentre.Data.Entities;
using LogCentre.Data.Entities.Log;
using LogCentre.Model;
using LogCentre.Model.Log;
using LogCentre.Services.Exceptions;
using LogCentre.Services.Interfaces;
using LogCentre.Services.Interfaces.Log;
using LogCentre.Services.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace LogCentre.Api.Controllers
{
    /// <summary>
    /// Controller for Line
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LogLineController : BaseApiController<LogLineController>
    {
        private const string IncludeTables = "Line.Host,Line.Provider";

        private readonly ILineService _lineService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for Line
        /// </summary>
        /// <param name="logger">Logger implementation</param>
        /// <param name="lineService">Service for Lines</param>
        /// <param name="mapper">AutoMapper properties</param>
        /// <exception cref="ArgumentNullException">Throws if something is null</exception>
        public LogLineController(ILogger<LogLineController> logger,
            ILineService lineService,
            IMapper mapper)
            : base(logger)
        {
            _lineService = lineService ?? throw new ArgumentNullException(nameof(lineService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Get

        /// <summary>
        /// Gets a single Log Line
        /// </summary>
        /// <param name="id">Id of Log Line</param>
        /// <returns>Log Line</returns>
        [HttpGet("{id:long}", Name = nameof(GetLineById)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LineModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLineById([FromRoute] long id)
        {
            Logger.LogDebug("GetLineById() | Id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entities = await _lineService.GetAsync(a => a.Id == id, null, IncludeTables);
                var entity = entities.FirstOrDefault();
                if (entity == null)
                {
                    return HandleNotFoundRequest("Log Line not found", $"Log Line with Id[{id}] was not found");
                }

                var model = _mapper.Map<Line, LineModel>(entity);
                return Ok(model);
            }
            catch (LineException lse)
            {
                return HandleBadRequest("Invalid Log Line Id", lse.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("And error has occurred",
                    $"GetLineById() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetLineById took [{0}]", stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Gets all Log Line lines
        /// </summary>
        /// <returns>List of <see cref="LineModel">Log Lines</see></returns>
        [HttpGet("all", Name = nameof(GetLines)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<LineModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLines()
        {
            Logger.LogDebug("GetLines()");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entries = await _lineService.GetAsync(null, a => a.OrderByDescending(b => b.RowVersion),
                    IncludeTables);
                var items = entries.ToList();

                var models = _mapper.Map<IList<Line>, IList<LineModel>>(items);
                return Ok(models);
            }
            catch (LineException ale)
            {
                return HandleBadRequest("Error getting Log Line entries", ale.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred",
                    $"GetLines() produced an exception [{ex.Message}]",
                    ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetLines took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Creates a new Log Line
        /// </summary>
        /// <param name="apiVersion">The route supplied API version</param>
        /// <param name="model">The <see cref="LineModel">Log Line</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="LineModel">Log Line</see></returns>
        [HttpPost, Benchmark]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LineModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAsync(ApiVersion apiVersion, [FromBody] LineModel model)
        {
            Logger.LogDebug("CreateAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Line entity;
                if (model.Id > 0)
                {
                    if (_lineService.TryGet(model.Id, out entity))
                    {
                        if (entity != null && entity.Deleted == DataLiterals.Yes)
                        {
                            return HandleConflictRequest("Log Line already exists", $"Log Line with Id [{entity.Id}] already exists");
                        }
                    }
                }

                entity = _mapper.Map<LineModel, Line>(model);
                entity = await _lineService.CreateAsync(entity);

                model = _mapper.Map<Line, LineModel>(entity);
                return CreatedAtAction(nameof(GetLineById), new { id = entity.Id, version = $"{apiVersion}" }, model);
            }
            catch (LineException he)
            {
                return HandleBadRequest("Error creating new Line", he.Message);
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
        /// Update an existing Line
        /// </summary>
        /// <param name="model">The <see cref="LineModel">Line</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="LineModel">Line</see></returns>
        [HttpPut, Benchmark]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReplaceAsync([FromBody] LineModel model)
        {
            Logger.LogDebug("ReplaceAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!_lineService.TryGet(model.Id, out var entity))
                {
                    return HandleNotFoundRequest("Invalid Line", "The Line cannot be located by Id");
                }

                entity.LogSourceId = model.LogSourceId;
                entity.LogLine = model.LogLine;
                entity.Active = model.Active;
                entity.Deleted = model.Deleted;
                entity.LastUpdatedBy = model.LastUpdatedBy;

                await _lineService.UpdateAsync(entity);

                return NoContent();
            }
            catch (LineException he)
            {
                return HandleBadRequest("Error updating Line", he.Message);
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
        /// Delete a Line
        /// </summary>
        /// <param name="id">The Line id</param>
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
                if (!_lineService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("Line not found", $"Unable to locate Line with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.Yes)
                {
                    return HandleNotFoundRequest("Line not found", $"Line already deleted with Id [{id}]");
                }

                await _lineService.DeleteAsync(entity);

                return NoContent();
            }
            catch (LineException he)
            {
                return HandleBadRequest("Error deleting Line", he.Message);
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
        /// Remove a Line and all its dependencies if it has been soft deleted
        /// </summary>
        /// <param name="id">The Line id</param>
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
                if (!_lineService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("Line not found", $"Unable to locate Line with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.No)
                {
                    return HandleNotFoundRequest("Line not found", $"Line not soft deleted with Id [{id}]");
                }

                await _lineService.RemoveAsync(entity);

                return NoContent();
            }
            catch (LineException he)
            {
                return HandleBadRequest("Unable to remove Line", he.Message);
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
