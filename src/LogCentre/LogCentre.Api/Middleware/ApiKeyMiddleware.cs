namespace LogCentre.Api.Middleware
{
    /// <summary>
    /// Api Key Middleware
    /// Found at: https://www.c-sharpcorner.com/article/using-api-key-authentication-to-secure-asp-net-core-web-api/
    /// </summary>
    public class ApiKeyMiddleware
    {
        private const string APIKEY = "XApiKey";
        private readonly RequestDelegate _next;

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="next">Next action</param>
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invoke this action and if successful, move on to the next action
        /// </summary>
        /// <param name="context">Current HttpContext</param>
        /// <returns>Nothing</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEY, out
                    var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided ");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEY);
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }

            await _next(context);
        }
    }
}
