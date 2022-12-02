using System.Runtime.Serialization;

namespace LogCentre.Services.Exceptions
{
    [Serializable]
    public class LogSourceException : Exception
    {
        public LogSourceException(string message) : base(message) { }

        public LogSourceException(string message, Exception innerException) : base(message, innerException) { }

        public LogSourceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
