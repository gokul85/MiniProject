using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    internal class UsernameAlreadyExistException : Exception
    {
        public UsernameAlreadyExistException()
        {
        }

        public UsernameAlreadyExistException(string? message) : base(message)
        {
        }

        public UsernameAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UsernameAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}