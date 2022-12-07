using LogCentre.ApiClient;
using LogCentre.Model;
using LogCentre.Web.Models;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LogCentre.Web.Helpers;

namespace LogCentre.Web.Areas.Admin.Pages.Provider
{
    public class IndexModel : PageModelBase<IndexModel>
    {
        public IndexModel(ILogger<IndexModel> logger,
            ILogCentreApiClient client,
            IRazorRenderService renderService,
            IConfiguration configuration)
            : base(logger, client, renderService, configuration)
        {
            Providers = new List<ProviderModel>();
        }

        public IEnumerable<ProviderModel> Providers { get; set; }

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
                var response = await ApiClient.GetProvidersAsync();
                response = response.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
                Providers = response;

                return new PartialViewResult
                {
                    ViewName = "_ViewAll",
                    ViewData = new ViewDataDictionary<IEnumerable<ProviderModel>>(ViewData, Providers)
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
                    return new JsonResult(new { isValid = true, html = await RenderService.ToStringAsync("_CreateOrEdit", new ProviderModel()) });
                }
                else
                {
                    var model = await ApiClient.GetProviderByIdAsync(id);
                    if (model == null)
                    {
                        model = new ProviderModel();
                    }

                    return new JsonResult(new { isValid = true, html = await RenderService.ToStringAsync("_CreateOrEdit", model) });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"OnGetCreateOrEditAsync() error [{ex}]");
                return new JsonResult(new { isValid = false, html = await RenderService.ToStringAsync("_CreateOrEdit", new ProviderModel()), errors = ex.Message });
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("*** OnGetCreateOrEditAsyn took [{0}]", stopwatch.Elapsed);
            }
        }

        public async Task<JsonResult> OnPostCreateOrEditAsync(long id, [Bind("Name,Description,Regex,Active,Deleted,Id")] ProviderModel provider)
        {
            Logger.LogDebug("OnPostCreateOrEditAsync() | id [{0}], provider [{1}]", id, provider);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                ValidateProvider(id, provider);
                if (ModelState.IsValid)
                {
                    if (id == 0)
                    {
                        try
                        {
                            var response = await ApiClient.CreateProviderAsync(provider);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"OnPostCreateOrEditAsync() | creating new provider error[{ex}]", ex);
                            var html = await RenderService.ToStringAsync("_CreateOrEdit", provider);
                            return new JsonResult(new { isValid = false, html = html, errors = ex.Message });
                        }
                    }
                    else
                    {
                        try
                        {
                            await ApiClient.UpdateProviderAsync(provider);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"OnPostCreateOrEditAsync() | editing provider error[{ex}]", ex);
                            var html = await RenderService.ToStringAsync("_CreateOrEdit", provider);
                            return new JsonResult(new { isValid = false, html = html, errors = ex.Message });
                        }
                    }

                    return await LoadAsync();
                }
                else
                {
                    var modelStateErrors = ModelStateHelper.GetModelStateErrors(ModelState);
                    var html = await RenderService.ToStringAsync("_CreateOrEdit", provider);
                    return new JsonResult(new { isValid = false, html = html, errors = modelStateErrors });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"OnPostCreateOrEditAsync() error [{ex}]");
                return new JsonResult(new { isValid = false, html = await RenderService.ToStringAsync("_CreateOrEdit", provider) });
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
                await ApiClient.DeleteProviderAsync(id);
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
                var response = await ApiClient.GetProvidersAsync();
                response = response.Where(x => x.Active == ModelLiterals.Yes && x.Deleted == ModelLiterals.No).ToList();
                Providers = response;
                html = await RenderService.ToStringAsync("_ViewAll", Providers);
                return new JsonResult(new { isValid = isValid, message = message, html = html });
            }
            catch (Exception ex)
            {
                html = await RenderService.ToStringAsync("_ViewAll", Providers);
                return new JsonResult(new { isValid = false, message = ex.Message, html = html });
            }
        }

        private void ValidateProvider(long id, ProviderModel providerr)
        {
            Logger.LogDebug("ValidateHost() | id[{id}], providerr[{providerr}]", id, providerr);
            if (!ModelState.IsValid)
            {
                return;
            }

            if (id != 0 && id != providerr.Id)
            {
                ModelState.AddModelError("Id", "Invalid Provider Id");
            }

            if (string.IsNullOrWhiteSpace(providerr.Name))
            {
                ModelState.AddModelError("Name", "Invalid Provider Name");
            }
        }
    }
}
