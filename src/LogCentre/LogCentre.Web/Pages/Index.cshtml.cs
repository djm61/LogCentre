using LogCentre.ApiClient;
using LogCentre.Model.Search;
using LogCentre.Web.Models;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Diagnostics;

namespace LogCentre.Web.Pages
{
    public class IndexModel : PageModelBase<IndexModel>
    {
        public IndexModel(ILogger<IndexModel> logger,
           ILogCentreApiClient client,
           IRazorRenderService renderService,
           IConfiguration configuration)
           : base(logger, client, renderService, configuration)
        {
            DistinctLevels = new SelectList(new List<string>());
        }

        public SelectList DistinctLevels { get; set; }

        public async Task OnGet()
        {
            var levelSelectList = await GetLevelSelectList();
            DistinctLevels = levelSelectList;
        }

        public async Task<JsonResult> OnGetPerformSearchAsync(SearchModel searchModel)
        {
            Logger.LogDebug("OnGetPerformSearchAsync() | searchModel[{searchModel}]", searchModel);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var results = await ApiClient.GetItemsForSearchingAsync(searchModel);
                var html = await RenderService.ToStringAsync("_Results", results);
                return new JsonResult(new { isValid = true, html = html });
            }
            catch (Exception ex)
            {
                Logger.LogError($"OnGetPerformSearchAsync had an error [{ex}]", ex);
                return new JsonResult(new { isValid = false, message = ex.Message });
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** OnGetPerformSearchAsync took [{0}]", stopwatch.Elapsed);
            }
        }

        public async Task<JsonResult> OnGetLogFileAsync(long id)
        {
            return new JsonResult(new { isValid = true, html = id.ToString() });
        }

        private async Task<SelectList> GetLevelSelectList()
        {
            Logger.LogDebug("GetLevelSelectList()");

            var items = await ApiClient.GetDistinctLevelsAsync();
            var list = new SelectList(items);
            return list;
        }
    }
}