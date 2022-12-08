using LogCentre.ApiClient;
using LogCentre.Model;
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

        public async Task<JsonResult> OnGetPerformSearchAsync(string searchValue)
        {
            Logger.LogDebug("OnGetPerformSearchAsync() | searchValue[{searchValue}]", searchValue);
            if (searchValue.Length < 3)
            {
                return new JsonResult(new { isValid = false, results = "Search value not long enough" });
            }

            return new JsonResult(new { isValid = true, results = "asdf" });
        }
    }
}