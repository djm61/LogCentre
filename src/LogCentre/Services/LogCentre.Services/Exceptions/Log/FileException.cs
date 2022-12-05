using System.Runtime.Serialization;

namespace LogCentre.Services.Exceptions
{
    [Serializable]
    public class FileException : Exception
    {
        public FileException(string message) : base(message) { }

        public FileException(string message, Exception innerException) : base(message, innerException) { }

        public FileException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
