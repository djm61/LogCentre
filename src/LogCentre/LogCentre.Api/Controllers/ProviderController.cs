using AutoMapper;

using LogCentre.Api.Attributes;
using LogCentre.Data;
using LogCentre.Data.Entities;
using LogCentre.Model;
using LogCentre.Services;
using LogCentre.Services.Exceptions;
using LogCentre.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

using System.Diagnostics;

namespace LogCentre.Api.Controllers
{
    /// <summary>
    /// Provider controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProviderController : BaseApiController<ProviderController>
    {
        private readonly IProviderService _providerService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for Provider
        /// </summary>
        /// <param name="logger">Logger implementation for providers</param>
        /// <param name="providerService">Service for Providers</param>
        /// <param name="mapper">Automapper service</param>
        /// <exception cref="ArgumentNullException">Throws if the provider service is null</exception>
        public ProviderController(ILogger<ProviderController> logger,
            IProviderService providerService,
            IMapper mapper)
            : base(logger)
        {
            _providerService = providerService ?? throw new ArgumentNullException(nameof(providerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Get

        /// <summary>
        /// Gets a single Provider
        /// </summary>
        /// <param name="id">Id of Provider</param>
        /// <returns>Provider</returns>
        [HttpGet("{id:long}", Name = nameof(GetProviderById)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProviderModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProviderById([FromRoute] long id)
        {
            Logger.LogDebug("GetProviderById() | Id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entities = await _providerService.GetAsync(a => a.Id == id);
                var entity = entities.FirstOrDefault();
                if (entity == null)
                {
                    return HandleNotFoundRequest("Provider not found", $"Provider with Id[{id}] was not found");
                }

                var model = _mapper.Map<Provider, ProviderModel>(entity);
                return Ok(model);
            }
            catch (ProviderException pe)
            {
                return HandleBadRequest("Invalid Provider Id", pe.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("And error has occurred",
                    $"GetProviderById() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetProviderById took [{0}]", stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Gets all Provider lines
        /// </summary>
        /// <returns>List of <see cref="ProviderModel">Providers</see></returns>
        [HttpGet("all", Name = nameof(GetProviders)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ProviderModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProviders()
        {
            Logger.LogDebug("GetProviders()");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entries = await _providerService.GetAsync(null, a => a.OrderBy(b => b.Name));
                var items = entries.ToList();

                var models = _mapper.Map<IList<Provider>, IList<ProviderModel>>(items);
                return Ok(models);
            }
            catch (ProviderException pe)
            {
                return HandleBadRequest("Error getting Provider entries", pe.Message);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred",
                    $"GetProviders() produced an exception [{ex.Message}]",
                    ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetProviders took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Creates a new Provider
        /// </summary>
        /// <param name="apiVersion">The route supplied API version</param>
        /// <param name="model">The <see cref="ProviderModel">Provider</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="ProviderModel">Provider</see></returns>
        [HttpPost, Benchmark]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProviderModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAsync(ApiVersion apiVersion, [FromBody] ProviderModel model)
        {
            Logger.LogDebug("CreateAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Data.Entities.Provider entity;
                if (model.Id > 0)
                {
                    if (_providerService.TryGet(model.Id, out entity))
                    {
                        if (entity != null && entity.Deleted == DataLiterals.Yes)
                        {
                            return HandleConflictRequest("Provider already exists", $"Provider with Id [{entity.Id}] already exists");
                        }
                    }
                }

                entity = _mapper.Map<ProviderModel, Data.Entities.Provider>(model);
                entity = await _providerService.CreateAsync(entity);

                model = _mapper.Map<Data.Entities.Provider, ProviderModel>(entity);
                return CreatedAtAction(nameof(GetProviderById), new { id = entity.Id, version = $"{apiVersion}" }, model);
            }
            catch (ProviderException he)
            {
                return HandleBadRequest("Error creating new Provider", he.Message);
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
        /// Update an existing Provider
        /// </summary>
        /// <param name="model">The <see cref="ProviderModel">Provider</see> to be created</param>
        /// <returns>The url at which to retrieve the newly created <see cref="ProviderModel">Provider</see></returns>
        [HttpPut, Benchmark]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReplaceAsync([FromBody] ProviderModel model)
        {
            Logger.LogDebug("ReplaceAsync() | model[{model}]", model);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!_providerService.TryGet(model.Id, out var entity))
                {
                    return HandleNotFoundRequest("Invalid Provider", "The Provider cannot be located by Id");
                }

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Active = model.Active;
                entity.Deleted = model.Deleted;
                entity.LastUpdatedBy = model.LastUpdatedBy;

                await _providerService.UpdateAsync(entity);

                return NoContent();
            }
            catch (ProviderException he)
            {
                return HandleBadRequest("Error updating Provider", he.Message);
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
        /// Delete a Provider
        /// </summary>
        /// <param name="id">The Provider id</param>
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
                if (!_providerService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("Provider not found", $"Unable to locate Provider with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.Yes)
                {
                    return HandleNotFoundRequest("Provider not found", $"Provider already deleted with Id [{id}]");
                }

                await _providerService.DeleteAsync(entity);

                return NoContent();
            }
            catch (ProviderException he)
            {
                return HandleBadRequest("Error deleting Provider", he.Message);
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
        /// Remove a Provider and all its dependencies if it has been soft deleted
        /// </summary>
        /// <param name="id">The Provider id</param>
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
                if (!_providerService.TryGet(id, out var entity))
                {
                    return HandleNotFoundRequest("Provider not found", $"Unable to locate Provider with Id [{id}]");
                }

                if (entity.Deleted == DataLiterals.No)
                {
                    return HandleNotFoundRequest("Provider not found", $"Provider not soft deleted with Id [{id}]");
                }

                await _providerService.RemoveAsync(entity);

                return NoContent();
            }
            catch (ProviderException he)
            {
                return HandleBadRequest("Unable to remove Provider", he.Message);
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
