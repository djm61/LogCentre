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
    }
}
