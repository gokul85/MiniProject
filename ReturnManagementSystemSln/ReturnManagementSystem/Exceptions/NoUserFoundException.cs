using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    public class NoUserFoundException : Exception
    {
        public NoUserFoundException(string? message) : base(message)
        {
        }
    }
}