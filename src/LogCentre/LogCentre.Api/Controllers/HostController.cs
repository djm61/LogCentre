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
    /// Host controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class HostController : BaseApiController<HostController>
    {
        private readonly IHostService _hostService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for Host
        /// </summary>
        /// <param name="logger">Logger implementation for hosts</param>
        /// <param name="hostService">Service for Hosts</param>
        /// <param name="mapper">Automapper service</param>
        /// <exception cref="ArgumentNullException">Throws if the host service is null</exception>
        public HostController(ILogger<HostController> logger,
            IHostService hostService,
            IMapper mapper)
            : base(logger)
        {
            _hostService = hostService ?? throw new ArgumentNullException(nameof(hostService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Get

        [HttpGet("{id:long}", Name = nameof(GetHostById)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HostModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHostById([FromRoute] long id)
        {
            Logger.LogDebug("GetActionCodeById() | Id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entities = await _hostService.GetAsync(a => a.Id == id);
                var entity = entities.FirstOrDefault();
                if (entity == null)
                {
                    return HandleNotFoundRequest("Host not found", $"Host with Id[{id}] was not found");
                }

                var model = _mapper.Map<Data.Entities.Host, HostModel>(entity);
                return Ok(model);
            }
            catch (HostException he)
            {
                return HandleBadRequest("Invalid Host Id", he.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("And error has occurred",
                    $"{nameof(GetHostById)} produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** {0} took [{1}]", nameof(GetHostById), stopwatch.Elapsed);
            }
        }

        [HttpGet("all", Name = nameof(GetHosts)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<HostModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHosts()
        {
            Logger.LogDebug("GetHosts()");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entries = await _hostService.GetAsync(null, a => a.OrderBy(b => b.Name));
                var items = entries.ToList();
                var models = _mapper.Map<IList<Data.Entities.Host>, IList<HostModel>>(items);
                return Ok(models);
            }
            catch (HostException he)
            {
                return HandleBadRequest("Error getting Host entries", he.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", $"GetHosts() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetHosts took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Creates a new Host
        /// </summary>
        /// <param name="apiVersion">The route supplied API version</param>
        /// <param name="model">The <see cref="HostModel">Host</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="HostModel">Host</see></returns>
        [HttpPost, Benchmark]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(HostModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAsync(ApiVersion apiVersion, [FromBody] HostModel model)
        {
            Logger.LogDebug("CreateAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Data.Entities.Host entity;
                if (model.Id > 0)
                {
                    if (_hostService.TryGet(model.Id, out entity))
                    {
                        if (entity != null && entity.Deleted == DataLiterals.Yes)
                        {
                            return HandleConflictRequest("Host already exists", $"Host with Id [{entity.Id}] already exists");
                        }
                    }
                }

                entity = _mapper.Map<HostModel, Data.Entities.Host>(model);
                entity = await _hostService.CreateAsync(entity);

                model = _mapper.Map<Data.Entities.Host, HostModel>(entity);
                return CreatedAtAction(nameof(GetHostById), new { id = entity.Id, version = $"{apiVersion}" }, model);
            }
            catch (HostException he)
            {
                return HandleBadRequest("Error creating new Host", he.Message);
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
        /// Update an existing Host
        /// </summary>
        /// <param name="model">The <see cref="HostModel">Host</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="HostModel">Host</see></returns>
        [HttpPut, Benchmark]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReplaceAsync([FromBody] HostModel model)
        {
            Logger.LogDebug("ReplaceAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!_hostService.TryGet(model.Id, out var entity))
                {
                    return HandleNotFoundRequest("Invalid Host", "The Host cannot be located by Id");
                }

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Active = model.Active;
                entity.Deleted = model.Deleted;
                entity.LastUpdatedBy = model.LastUpdatedBy;

                await _hostService.UpdateAsync(entity);
                return NoContent();
            }
            catch (EntityException ee)
            {
                return HandleBadRequest("Error updating Host", ee.Message);
            }
            catch (HostException he)
            {
                return HandleBadRequest("Error updating Host", he.Message);
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
        /// Delete a Host
        /// </summary>
        /// <param name="id">The Host id</param>
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
                if (!_hostService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("Host not found", $"Unable to locate Host with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.Yes)
                {
                    return HandleNotFoundRequest("Host not found", $"Host already deleted with Id [{id}]");
                }

                await _hostService.DeleteAsync(entity);
                return NoContent();
            }
            catch (HostException he)
            {
                return HandleBadRequest("Error deleting Host", he.Message);
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
        /// Remove a Host and all its dependencies if it has been soft deleted
        /// </summary>
        /// <param name="id">The Host id</param>
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
                if (!_hostService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("Host not found", $"Unable to locate Host with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.No)
                {
                    return HandleNotFoundRequest("Host not found", $"Host not soft deleted with Id [{id}]");
                }

                await _hostService.RemoveAsync(entity);

                return NoContent();
            }
            catch (HostException he)
            {
                return HandleBadRequest("Unable to remove Host", he.Message);
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
