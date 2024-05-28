using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    internal class NoUserFoundException : Exception
    {
        public NoUserFoundException()
        {
        }

        public NoUserFoundException(string? message) : base(message)
        {
        }

        public NoUserFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoUserFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}