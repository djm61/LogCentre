using LogCentre.ApiClient;
using LogCentre.Web.Models;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc;

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

        public async Task<JsonResult> OnGetPerformSearchAsync(DateTime fromDate, DateTime endDate, string searchText)
        {
            Logger.LogDebug("OnGetPerformSearchAsync() | searchText[{searchText}]", searchText);
            if (searchText.Length < 3)
            {
                return new JsonResult(new { isValid = false, results = "Search value not long enough" });
            }

            var results = await ApiClient.GetItensForSearchingAsync(searchText);
            return new JsonResult(new { isValid = true, results = results });
        }
    }
}