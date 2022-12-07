using LogCentre.ApiClient;
using LogCentre.Model;
using LogCentre.Model.Log;
using LogCentre.Web.Models;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using System.Diagnostics;

namespace LogCentre.Web.Areas.Admin.Pages.LogFile
{
    public class IndexModel : PageModelBase<IndexModel>
    {
        public IndexModel(ILogger<IndexModel> logger,
            ILogCentreApiClient client,
            IRazorRenderService renderService,
            IConfiguration configuration)
            : base(logger, client, renderService, configuration)
        {
            LogFiles = new List<FileModel>();
        }

        public IEnumerable<FileModel> LogFiles { get; set; }

        public void OnGet()
        {
            Logger.LogDebug("OnGet()");
        }

        public async Task<PartialViewResult> OnGetViewAllPartialAsync()
        {
            Logger.LogDebug("OnGetViewAllPartialAsync()");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await ApiClient.GetLogFilesAsync();
                response = response.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
                LogFiles = response;

                return new PartialViewResult
                {
                    ViewName = "_ViewAll",
                    ViewData = new ViewDataDictionary<IEnumerable<FileModel>>(ViewData, LogFiles)
                };
            }
            catch (Exception ex)
            {
                Logger.LogError($"ONGetViewAllPartialAsync() | Error [{ex}]", ex);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("*** OnGetViewAllPartialAsync took [{0}]", stopwatch.Elapsed);
            }
        }

        public async Task<JsonResult> OnPostDeleteAsync(long id)
        {
            Logger.LogDebug("OnPostDeleteAsync() | id [{0}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await ApiClient.DeleteLogFileAsync(id);
                return await LoadAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"OnPostDeleteAsync() error [{ex}]");
                return await LoadAsync(false, ex.Message);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("*** OnPostDeleteAsync took [{0}]", stopwatch.Elapsed);
            }
        }

        private async Task<JsonResult> LoadAsync(bool isValid = true, string message = "")
        {
            Logger.LogDebug("LoadAsync() | isValid[{isValid}], message[{message}]", isValid, message);
            var html = string.Empty;

            try
            {
                var response = await ApiClient.GetLogFilesAsync();
                response = response.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
                LogFiles = response;
                html = await RenderService.ToStringAsync("_ViewAll", LogFiles);
                return new JsonResult(new { isValid = isValid, message = message, html = html });
            }
            catch (Exception ex)
            {
                html = await RenderService.ToStringAsync("_ViewAll", LogFiles);
                return new JsonResult(new { isValid = false, message = ex.Message, html = html });
            }
        }
    }
}
