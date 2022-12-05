namespace LogCentre.Api.Middleware
{
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

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
