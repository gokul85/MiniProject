using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    public class UserNotActiveException : Exception
    {

        public UserNotActiveException(string? message) : base(message)
        {
        }
    }
}