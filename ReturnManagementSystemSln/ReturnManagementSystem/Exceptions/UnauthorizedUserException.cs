using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    public class UnauthorizedUserException : Exception
    {
        public UnauthorizedUserException()
        {
        }

        public UnauthorizedUserException(string? message) : base(message)
        {
        }

        public UnauthorizedUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnauthorizedUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}