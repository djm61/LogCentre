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
                var results = await ApiClient.GetItensForSearchingAsync(searchModel);
                return new JsonResult(new { isValid = true, results = results });
            }
            catch (Exception ex)
            {
                Logger.LogError($"OnGetPerformSearchAsync had an error [{ex}]", ex);
                return new JsonResult(new { isValid = false, results = ex.Message });
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** OnGetPerformSearchAsync took [{0}]", stopwatch.Elapsed);
            }
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