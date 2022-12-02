using AutoMapper;

using LogCentre.Api.Attributes;
using LogCentre.Data.Entities;
using LogCentre.Model;
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
    }
}
