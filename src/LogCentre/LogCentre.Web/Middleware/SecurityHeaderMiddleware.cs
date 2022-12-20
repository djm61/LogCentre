using LogCentre.Web.Helpers;

namespace LogCentre.Web.Middleware
{
    /// <summary>
    /// Security Header Middleware
    /// Found: https://www.thecodebuzz.com/middleware-accessing-config-settings-aspnet-core/
    /// </summary>
    public class SecurityHeaderMiddleware
    {
        public const string XFrameOptionsHeaderKey = "X-Frame-Options";
        public const string XFrameOptionsHeaderValue = "SAMEORIGIN";
        public const string ContentSecurityPolicyHeaderKey = "Content-Security-Policy";
        public const string XContentTypeOptionsHeaderKey = "X-Content-Type-Options";

        private readonly RequestDelegate _next;
        private readonly SecurityHeaders _securityHeaders;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">Next action to invoke</param>
        /// <param name="securityHeaders">Injected <see cref="SecurityHeaders">security header object</see> from config</param>
        /// <exception cref="ArgumentNullException">Throws if security headers is null</exception>
        public SecurityHeaderMiddleware(RequestDelegate next, SecurityHeaders securityHeaders)
        {
            _next = next;
            _securityHeaders = securityHeaders ?? throw new ArgumentNullException(nameof(securityHeaders));
        }

        /// <summary>
        /// Invoke the action
        /// </summary>
        /// <param name="context">Current Http context</param>
        /// <returns>Nothing</returns>
        public async Task Invoke(HttpContext context)
        {
            if (_securityHeaders.XFrameOptionsAsBool)
            {
                context.Response.Headers.Add(XFrameOptionsHeaderKey, XFrameOptionsHeaderValue);
            }

            if (_securityHeaders.ContentSecurityPolicyAsBool)
            {
                context.Response.Headers.Add(ContentSecurityPolicyHeaderKey, "object-src 'self'; connect-src *; " +
                    "default-src 'self' 'unsafe-inline' 'unsafe-eval' " +
                    "blob: data: https://cdn.datatables.net" +
                    ";");
            }

            if (_securityHeaders.XContentTypeOptionsAsBool &&
                _securityHeaders.ContentSecurityPolicyOptions != null &&
                !string.IsNullOrWhiteSpace(_securityHeaders.ContentSecurityPolicyOptions.ContentTypeOptions))
            {
                context.Response.Headers.Add(XContentTypeOptionsHeaderKey, _securityHeaders.ContentSecurityPolicyOptions.ContentTypeOptions);
            }

            await _next.Invoke(context);
        }
    }
}
