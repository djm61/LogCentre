using System.Runtime.Serialization;

namespace LogCentre.Services.Exceptions
{
    [Serializable]
    public class HostException : Exception
    {
        public HostException(string message) : base(message) { }

        public HostException(string message, Exception innerException) : base(message, innerException) { }

        public HostException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
