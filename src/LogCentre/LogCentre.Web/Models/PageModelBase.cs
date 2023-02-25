using LogCentre.ApiClient;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LogCentre.Web.Models
{
    public class PageModelBase<T> : PageModel where T : PageModel
    {
        protected readonly ILogger<T> Logger;
        protected readonly ILogCentreApiClient ApiClient;
        protected readonly IRazorRenderService RenderService;

        internal PageModelBase(ILogger<T> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApiClient = null;
            RenderService = null;

            SoftwareRelease = string.Empty;
            SoftwareVersion = string.Empty;
        }

        internal PageModelBase(ILogger<T> logger, IConfiguration configuration)
            : this(logger)
        {
            SoftwareRelease = configuration?.GetSection("SoftwareRelease")?.Value ?? throw new ArgumentNullException("SoftwareRelease");
            SoftwareVersion = configuration?.GetSection("SoftwareVersion")?.Value ?? throw new ArgumentNullException("SoftwareVersion");
        }

        internal PageModelBase(ILogger<T> logger, ILogCentreApiClient apiClient, IConfiguration configuration)
            : this(logger, configuration)
        {
            ApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        internal PageModelBase(ILogger<T> logger, ILogCentreApiClient apiClient,
            IRazorRenderService renderService,
            IConfiguration configuration)
            : this(logger, apiClient, configuration)
        {
            RenderService = renderService ?? throw new ArgumentNullException(nameof(renderService));
        }

        public string SoftwareRelease { get; }
        public string SoftwareVersion { get; }
    }
}
