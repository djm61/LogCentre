using AutoMapper;
using LogCentre.Api.Attributes;
using LogCentre.Data;
using LogCentre.Data.Entities.Log;
using LogCentre.Model.Log;

using LogCentre.Services.Exceptions;
using LogCentre.Services.Interfaces.Log;
using LogCentre.Services.Services.Log;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace LogCentre.Api.Controllers
{
    /// <summary>
    /// Controller for Log File
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LogFileController : BaseApiController<LogFileController>
    {
        private const string IncludeTables = "LogSource.Host,LogSource.Provider";

        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public LogFileController(ILogger<LogFileController> logger,
            IFileService fileService,
            IMapper mapper)
            : base(logger)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Get

        /// <summary>
        /// Gets a single Log File
        /// </summary>
        /// <param name="id">Id of Log File</param>
        /// <returns>Log File</returns>
        [HttpGet("{id:long}", Name = nameof(GetFileById)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LineModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFileById([FromRoute] long id)
        {
            Logger.LogDebug("GetLineById() | Id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entities = await _fileService.GetAsync(a => a.Id == id, null, IncludeTables);
                var entity = entities.FirstOrDefault();
                if (entity == null)
                {
                    return HandleNotFoundRequest("Log Line not found", $"Log Line with Id[{id}] was not found");
                }

                var model = _mapper.Map<Data.Entities.Log.File, FileModel>(entity);
                return Ok(model);
            }
            catch (LineException lse)
            {
                return HandleBadRequest("Invalid Log File Id", lse.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("And error has occurred",
                    $"GetLineById() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetFileById took [{0}]", stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Gets all Log File
        /// </summary>
        /// <returns>List of <see cref="FileModel">Log Files</see></returns>
        [HttpGet("all", Name = nameof(GetFiles)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<FileModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFiles()
        {
            Logger.LogDebug("GetLines()");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entries = await _fileService.GetAsync(null, a => a.OrderBy(b => b.Name),
                    IncludeTables);
                var items = entries.ToList();

                var models = _mapper.Map<IList<Data.Entities.Log.File>, IList<FileModel>>(items);
                return Ok(models);
            }
            catch (LineException ale)
            {
                return HandleBadRequest("Error getting Log File entries", ale.Message);
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
        /// Creates a new Log File
        /// </summary>
        /// <param name="model">The <see cref="FileModel">Log File</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="FileModel">Log File</see></returns>
        [HttpPost, Benchmark]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FileModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAsync([FromBody] FileModel model)
        {
            Logger.LogDebug("CreateAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Data.Entities.Log.File entity;
                if (model.Id > 0)
                {
                    if (_fileService.TryGet(model.Id, out entity))
                    {
                        if (entity != null && entity.Deleted == DataLiterals.Yes)
                        {
                            return HandleConflictRequest("Log FIle already exists", $"Log File with Id [{entity.Id}] already exists");
                        }
                    }
                }

                entity = _mapper.Map<FileModel, Data.Entities.Log.File>(model);
                entity = await _fileService.CreateAsync(entity);

                model = _mapper.Map<Data.Entities.Log.File, FileModel>(entity);
                return CreatedAtAction(nameof(GetFileById), new { id = entity.Id }, model);
            }
            catch (LineException he)
            {
                return HandleBadRequest("Error creating new File", he.Message);
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
        /// Update an existing File
        /// </summary>
        /// <param name="model">The <see cref="FileModel">Log File</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="FileModel">Log File</see></returns>
        [HttpPut, Benchmark]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReplaceAsync([FromBody] FileModel model)
        {
            Logger.LogDebug("ReplaceAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!_fileService.TryGet(model.Id, out var entity))
                {
                    return HandleNotFoundRequest("Invalid File", "The File cannot be located by Id");
                }

                entity.LogSourceId = model.LogSourceId;
                entity.Name = model.Name;
                entity.Active = model.Active;
                entity.Deleted = model.Deleted;
                entity.LastUpdatedBy = model.LastUpdatedBy;

                await _fileService.UpdateAsync(entity);

                return NoContent();
            }
            catch (LineException he)
            {
                return HandleBadRequest("Error updating File", he.Message);
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
        /// Delete a File
        /// </summary>
        /// <param name="id">The File id</param>
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
                if (!_fileService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("File not found", $"Unable to locate File with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.Yes)
                {
                    return HandleNotFoundRequest("File not found", $"File already deleted with Id [{id}]");
                }

                await _fileService.DeleteAsync(entity);

                return NoContent();
            }
            catch (LineException he)
            {
                return HandleBadRequest("Error deleting File", he.Message);
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
        /// Remove a File and all its dependencies if it has been soft deleted
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
                if (!_fileService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("File not found", $"Unable to locate File with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.No)
                {
                    return HandleNotFoundRequest("File not found", $"File not soft deleted with Id [{id}]");
                }

                await _fileService.RemoveAsync(entity);

                return NoContent();
            }
            catch (LineException he)
            {
                return HandleBadRequest("Unable to remove File", he.Message);
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
