using LogCentre.Api.Attributes;
using LogCentre.Model.Search;
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
    public class SearchController : BaseApiController<SearchController>
    {
        private readonly ISearchService _searchService;

        /// <summary>
        /// Cache Search Controller
        /// </summary>
        /// <param name="logger">Implementation of the logger</param>
        /// <param name="searchService">Cache Search Service</param>
        /// <exception cref="ArgumentNullException">Throws is anything is null</exception>
        public SearchController(ILogger<SearchController> logger,
            ISearchService searchService)
            : base(logger)
        {
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        #region Get

        /// <summary>
        /// Gets distinct log levels
        /// </summary>
        /// <returns>List of distinct log levels</returns>
        [HttpGet("distinctlevels"), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDistinctLogLevels()
        {
            Logger.LogDebug("GetDistinctLogLevels()");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var items = await _searchService.GetDistinctLogLevelsAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", $"GetDistinctLogLevels() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetDistinctLogLevels took [{0}]", stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Returns file contents from a provided line Id
        /// </summary>
        /// <param name="apiVersion"></param>
        /// <param name="lineId"></param>
        /// <returns></returns>
        [HttpGet("line/{lineId:long}"), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<SearchResultModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetFileContentsByLineId(ApiVersion apiVersion, [FromRoute] long lineId)
        {
            Logger.LogDebug("GetFileContentsByLineId() | id[{lineId}]", lineId);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var lines = await _searchService.GetFileLinesAsync(lineId);
                return Ok(lines);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", $"GetFileContentsByLineId() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetFileContentsByLineId took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Gets search results
        /// </summary>
        /// <param name="apiVersion">The route supplied API</param>
        /// <param name="searchModel">search model</param>
        /// <returns>list of search results</returns>
        [HttpPost("search"), Benchmark]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<SearchResultModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSearchResults(ApiVersion apiVersion, [FromBody] SearchModel searchModel)
        {
            Logger.LogDebug("GetSearchResults() | searchModel[{searchModel}]", searchModel);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var entries = await _searchService.SearchAsync(searchModel);
                var items = entries.ToList();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return HandleServerError("An error has occurred", $"GetSearchResults() produced an exception [{ex.Message}]", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetSearchResults took [{0}]", stopwatch.Elapsed);
            }
        }

        #endregion
    }
}
