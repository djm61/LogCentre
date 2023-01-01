using LogCentre.ApiClient;
using LogCentre.Model;
using LogCentre.Web.Models;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LogCentre.Web.Helpers;
using LogCentre.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LogCentre.Web.Areas.Admin.Pages.LogSource
{
    public class IndexModel : PageModelBase<IndexModel>
    {
        public IndexModel(ILogger<IndexModel> logger,
            ILogCentreApiClient client,
            IRazorRenderService renderService,
            IConfiguration configuration)
            : base(logger, client, renderService, configuration)
        {
            LogSources = new List<LogSourceModel>();
        }

        public IEnumerable<LogSourceModel> LogSources { get; set; }

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
                var response = await ApiClient.GetLogSourcesAsync();
                response = response.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
                LogSources = response;

                return new PartialViewResult
                {
                    ViewName = "_ViewAll",
                    ViewData = new ViewDataDictionary<IEnumerable<LogSourceModel>>(ViewData, LogSources)
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

        public async Task<JsonResult> OnGetCreateOrEditAsync(long id = 0)
        {
            Logger.LogDebug("OnGetCreateOrEditAsync() | id [{0}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var hostSelectList = await GetHostsSelectListAsync();
                var providerSelectList = await GetProvidersSelectListAsync();
                LogSourceModel model;
                if (id == 0)
                {
                    model = new LogSourceModel();
                }
                else
                {
                    model = await ApiClient.GetLogSourceByIdAsync(id);
                    if (model == null)
                    {
                        model = new LogSourceModel();
                    }
                }

                var viewModel = new LogSourceViewModel
                {
                    Id = model.Id,
                    HostId = model.HostId,
                    ProviderId = model.ProviderId,
                    Name = model.Name,
                    Path = model.Path,
                    Host = model.Host,
                    Provider = model.Provider
                };

                viewModel.HostSelectList = hostSelectList;
                viewModel.ProviderSelectList = providerSelectList;
                return new JsonResult(new { isValid = true, html = await RenderService.ToStringAsync("_CreateOrEdit", viewModel) });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"OnGetCreateOrEditAsync() error [{ex}]");
                return new JsonResult(new { isValid = false, html = await RenderService.ToStringAsync("_CreateOrEdit", new LogSourceViewModel()), errors = ex.Message });
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("*** OnGetCreateOrEditAsync took [{0}]", stopwatch.Elapsed);
            }
        }

        public async Task<JsonResult> OnPostCreateOrEditAsync(long id, [Bind("HostId,ProviderId,Name,Path,Active,Deleted,Id")] LogSourceViewModel logSource)
        {
            Logger.LogDebug("OnPostCreateOrEditAsync() | id [{0}], logSource [{1}]", id, logSource);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                ValidateLogSource(id, logSource);
                if (ModelState.IsValid)
                {
                    var logSourceModel = new LogSourceModel
                    {
                        Id = logSource.Id,
                        HostId = logSource.HostId,
                        ProviderId = logSource.ProviderId,
                        Name = logSource.Name,
                        Path = logSource.Path,
                        Active = logSource.Active,
                        Deleted = logSource.Deleted
                    };

                    if (id == 0)
                    {
                        try
                        {
                            var response = await ApiClient.CreateLogSourceAsync(logSourceModel);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"OnPostCreateOrEditAsync() | creating new logSource error[{ex}]", ex);
                            var html = await RenderService.ToStringAsync("_CreateOrEdit", logSource);
                            return new JsonResult(new { isValid = false, html = html, errors = ex.Message });
                        }
                    }
                    else
                    {
                        try
                        {
                            await ApiClient.UpdateLogSourceAsync(logSourceModel);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"OnPostCreateOrEditAsync() | editing logSource error[{ex}]", ex);
                            var html = await RenderService.ToStringAsync("_CreateOrEdit", logSource);
                            return new JsonResult(new { isValid = false, html = html, errors = ex.Message });
                        }
                    }

                    return await LoadAsync();
                }
                else
                {
                    var hostSelectList = await GetHostsSelectListAsync(logSource.HostId);
                    var providerSelectList = await GetProvidersSelectListAsync(logSource.ProviderId);
                    logSource.HostSelectList = hostSelectList;
                    logSource.ProviderSelectList = providerSelectList;
                    var modelStateErrors = ModelStateHelper.GetModelStateErrors(ModelState);
                    var html = await RenderService.ToStringAsync("_CreateOrEdit", logSource);
                    return new JsonResult(new { isValid = false, html = html, errors = modelStateErrors });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"OnPostCreateOrEditAsync() error [{ex}]");
                return new JsonResult(new { isValid = false, html = await RenderService.ToStringAsync("_CreateOrEdit", logSource) });
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("*** OnPostCreateOrEditAsync took [{0}]", stopwatch.Elapsed);
            }
        }

        public async Task<JsonResult> OnPostDeleteAsync(long id)
        {
            Logger.LogDebug("OnPostDeleteAsync() | id [{0}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await ApiClient.DeleteLogSourceAsync(id);
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
                var response = await ApiClient.GetLogSourcesAsync();
                response = response.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
                LogSources = response;
                html = await RenderService.ToStringAsync("_ViewAll", LogSources);
                return new JsonResult(new { isValid = isValid, message = message, html = html });
            }
            catch (Exception ex)
            {
                html = await RenderService.ToStringAsync("_ViewAll", LogSources);
                return new JsonResult(new { isValid = false, message = ex.Message, html = html });
            }
        }

        private async Task<SelectList> GetHostsSelectListAsync(long? selectedId = null)
        {
            Logger.LogDebug("GetHostSelectListAsync() | selectedId[{selectedId}]", selectedId);
            var items = await ApiClient.GetHostsAsync();
            items = items.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
            var list = new SelectList(items, "Id", "Name", selectedId);
            return list;
        }

        private async Task<SelectList> GetProvidersSelectListAsync(long? selectedId = null)
        {
            Logger.LogDebug("GetProvidersSelectListAsync() | selectedId[{selectedId}]", selectedId);
            var items = await ApiClient.GetProvidersAsync();
            items = items.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
            var list = new SelectList(items, "Id", "Name", selectedId);
            return list;
        }

        private void ValidateLogSource(long id, LogSourceViewModel logSource)
        {
            Logger.LogDebug("ValidateHost() | id[{id}], logSource[{logSource}]", id, logSource);
            if (!ModelState.IsValid)
            {
                return;
            }

            if (id != 0 && id != logSource.Id)
            {
                ModelState.AddModelError("Id", "Invalid LogSource Id");
            }

            if (logSource.HostId <= 0)
            {
                ModelState.AddModelError("HostId", "Invalid Host Id");
            }

            if (logSource.ProviderId <= 0)
            {
                ModelState.AddModelError("ProviderId", "Invalid Provider Id");
            }

            if (string.IsNullOrWhiteSpace(logSource.Name))
            {
                ModelState.AddModelError("Name", "Invalid LogSource Name");
            }
        }
    }
}
