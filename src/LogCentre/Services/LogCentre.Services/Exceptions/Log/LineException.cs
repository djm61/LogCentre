using System.Runtime.Serialization;

namespace LogCentre.Services.Exceptions
{
    [Serializable]
    public class LineException : Exception
    {
        public LineException(string message) : base(message) { }

        public LineException(string message, Exception innerException) : base(message, innerException) { }

        public LineException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
