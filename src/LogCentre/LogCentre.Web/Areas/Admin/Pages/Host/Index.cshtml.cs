using LogCentre.ApiClient;
using LogCentre.Model;
using LogCentre.Web.Helpers;
using LogCentre.Web.Models;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using System.Diagnostics;

namespace LogCentre.Web.Areas.Admin.Pages.Host
{
    public class IndexModel : PageModelBase<IndexModel>
    {
        public IndexModel(ILogger<IndexModel> logger,
            ILogCentreApiClient client,
            IRazorRenderService renderService,
            IConfiguration configuration)
            : base(logger, client, renderService, configuration)
        {
            Hosts = new List<HostModel>();
        }

        public IEnumerable<HostModel> Hosts { get; set; }

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
                var response = await ApiClient.GetHostsAsync();
                response = response.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
                Hosts = response;

                return new PartialViewResult
                {
                    ViewName = "_ViewAll",
                    ViewData = new ViewDataDictionary<IEnumerable<HostModel>>(ViewData, Hosts)
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
                if (id == 0)
                {
                    return new JsonResult(new { isValid = true, html = await RenderService.ToStringAsync("_CreateOrEdit", new HostModel()) });
                }
                else
                {
                    var model = await ApiClient.GetHostByIdAsync(id);
                    if (model == null)
                    {
                        model = new HostModel();
                    }

                    return new JsonResult(new { isValid = true, html = await RenderService.ToStringAsync("_CreateOrEdit", model) });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"OnGetCreateOrEditAsync() error [{ex}]");
                return new JsonResult(new { isValid = false, html = await RenderService.ToStringAsync("_CreateOrEdit", new HostModel()), errors = ex.Message });
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("*** OnGetCreateOrEditAsync took [{0}]", stopwatch.Elapsed);
            }
        }

        public async Task<JsonResult> OnPostCreateOrEditAsync(long id, [Bind("Name,Description,Active,Deleted,Id")] HostModel host)
        {
            Logger.LogDebug("OnPostCreateOrEditAsync() | id [{0}], host [{1}]", id, host);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                ValidateHost(id, host);
                if (ModelState.IsValid)
                {
                    if (id == 0)
                    {
                        try
                        {
                            var response = await ApiClient.CreateHostAsync(host);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"OnPostCreateOrEditAsync() | creating new host error[{ex}]", ex);
                            var html = await RenderService.ToStringAsync("_CreateOrEdit", host);
                            return new JsonResult(new { isValid = false, html = html, errors = ex.Message });
                        }
                    }
                    else
                    {
                        try
                        {
                            await ApiClient.UpdateHostAsync(host);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"OnPostCreateOrEditAsync() | editing host error[{ex}]", ex);
                            var html = await RenderService.ToStringAsync("_CreateOrEdit", host);
                            return new JsonResult(new { isValid = false, html = html, errors = ex.Message });
                        }
                    }

                    return await LoadAsync();
                }
                else
                {
                    var modelStateErrors = ModelStateHelper.GetModelStateErrors(ModelState);
                    var html = await RenderService.ToStringAsync("_CreateOrEdit", host);
                    return new JsonResult(new { isValid = false, html = html, errors = modelStateErrors });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"OnPostCreateOrEditAsync() error [{ex}]");
                return new JsonResult(new { isValid = false, html = await RenderService.ToStringAsync("_CreateOrEdit", host) });
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
                await ApiClient.DeleteHostAsync(id);
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
                var response = await ApiClient.GetHostsAsync();
                response = response.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
                Hosts = response;
                html = await RenderService.ToStringAsync("_ViewAll", Hosts);
                return new JsonResult(new { isValid = isValid, message = message, html = html });
            }
            catch (Exception ex)
            {
                html = await RenderService.ToStringAsync("_ViewAll", Hosts);
                return new JsonResult(new { isValid = false, message = ex.Message, html = html });
            }
        }

        private void ValidateHost(long id, HostModel host)
        {
            Logger.LogDebug("ValidateHost() | id[{id}], host[{host}]", id, host);
            if (!ModelState.IsValid)
            {
                return;
            }

            if (id != 0 && id != host.Id)
            {
                ModelState.AddModelError("Id", "Invalid Host Id");
            }

            if (string.IsNullOrWhiteSpace(host.Name))
            {
                ModelState.AddModelError("Name", "Invalid Host Name");
            }
        }
    }
}
