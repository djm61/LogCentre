using LogCentre.ApiClient;
using LogCentre.Model.Search;
using LogCentre.Web.Models;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc;

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

        public void OnGet()
        {
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
    }
}