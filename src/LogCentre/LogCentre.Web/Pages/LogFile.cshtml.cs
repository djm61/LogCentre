using LogCentre.ApiClient;
using LogCentre.Model.Search;
using LogCentre.Web.Models;
using LogCentre.Web.Services;

namespace LogCentre.Web.Pages
{
    public class LogFileModel :  PageModelBase<LogFileModel>
    {
        public LogFileModel(ILogger<LogFileModel> logger,
           ILogCentreApiClient client,
           IRazorRenderService renderService,
           IConfiguration configuration)
           : base(logger, client, renderService, configuration)
        {
        }

        public long LineId { get; set; } = 0;
        public List<SearchResultModel> LogLines { get; set; } = new List<SearchResultModel>();

        public async Task OnGet(long id)
        {
            LineId = id;
            var lines = await ApiClient.GetFileContentsFromLineIdAsync(id);
            LogLines = lines.ToList();
        }
    }
}
