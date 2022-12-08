using LogCentre.Api.Attributes;
using LogCentre.Api.Models;
using LogCentre.Model;
using LogCentre.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace LogCentre.Api.Controllers
{
    /// <summary>
    /// Cache Search Controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CacheSearchController : BaseApiController<CacheSearchController>
    {
        private readonly ICacheSearchService _cacheSearchService;

        /// <summary>
        /// Cache Search Controller
        /// </summary>
        /// <param name="logger">Implementation of the logger</param>
        /// <param name="cacheSearchService">Cache Search Service</param>
        /// <exception cref="ArgumentNullException">Throws is anything is null</exception>
        public CacheSearchController(ILogger<CacheSearchController> logger,
            ICacheSearchService cacheSearchService)
            : base(logger)
        {
            _cacheSearchService = cacheSearchService ?? throw new ArgumentNullException(nameof(cacheSearchService));
        }

        #region Get

        /// <summary>
        /// Gets cache search results
        /// </summary>
        /// <param name="dataItem">JSON search string</param>
        /// <returns>list of search results</returns>
        [HttpGet("{dataItem}", Name = nameof(GetCacheResults)), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<CacheItemModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCacheResults(string dataItem)
        {
            Logger.LogDebug("GetCacheResults() | dataItem[{dataItem}]", dataItem);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entries = await _cacheSearchService.SearchAsync(dataItem);
                var items = entries.ToList();
                return Ok(items);
            }
            //catch (HostException he)
            //{
            //    return HandleBadRequest("Error getting Host entries", he.Message);
            //}
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", $"GetCacheResult() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetCacheResults took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion
    }
}
