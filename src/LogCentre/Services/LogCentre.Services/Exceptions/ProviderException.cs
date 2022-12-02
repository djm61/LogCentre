using System.Runtime.Serialization;

namespace LogCentre.Services.Exceptions
{
    [Serializable]
    public class ProviderException : Exception
    {
        public ProviderException(string message) : base(message) { }

        public ProviderException(string message, Exception innerException) : base(message, innerException) { }

        public ProviderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
