namespace LogCentre.Api.Models
{
    public class ErrorResponse
    {
        private const string DefaultTraceId = "0000";

        private readonly Exception _exception;

        public ErrorResponse(Exception exception)
            : this(exception, true)
        {
        }

        public ErrorResponse(Exception exception, bool includeFullExceptionInfo)
            : this(500, exception?.Message ?? string.Empty, DefaultTraceId, exception, includeFullExceptionInfo)
        {
        }

        public ErrorResponse(int statusCode, string message, string traceId, Exception exception,
            bool includeFullExceptionInfo)
            : this(statusCode, message, traceId)
        {
            if (includeFullExceptionInfo)
            {
                _exception = exception;
            }
        }

        public ErrorResponse(int statusCode, string message, string traceId)
            : this(statusCode, message)
        {
            Trace = string.IsNullOrWhiteSpace(traceId) ? DefaultTraceId : traceId;
        }

        public ErrorResponse(int statusCode, string message)
            : this(statusCode)
        {
            Message = message ?? string.Empty;
        }

        public ErrorResponse(int statusCode)
        {
            Status = statusCode;
            Message = string.Empty;
            Trace = DefaultTraceId;

            _exception = null;
        }

        public int Status { get; }
        public string Message { get; }
        public string Trace { get; }
        public string Type => _exception?.GetType().Name ?? string.Empty;
        public override string ToString()
        {
            return $"Status[{Status}] Type[{Type}] Trace[{Trace}]: {Message}";
        }
    }
}
