namespace LogCentre.Web.Middleware
{
    /// <summary>
    /// Trace ID Middleware
    /// Adds a Trace-Id header
    /// </summary>
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructore
        /// </summary>
        /// <param name="next">next action to run</param>
        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// What happens when this is invoked
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Nothing</returns>
        public async Task Invoke(HttpContext context)
        {
            //context.TraceIdentifier = Guid.NewGuid().ToString();
            var date = DateTime.UtcNow;
            var id = $"{date:yyyy-MM-dd-HH-mm-ss-fff}--{context.Connection.Id}";
            context.TraceIdentifier = id;
            //string id = context.TraceIdentifier;
            context.Response.Headers["x-trace-id"] = id;
            await _next(context);
        }
    }
}
