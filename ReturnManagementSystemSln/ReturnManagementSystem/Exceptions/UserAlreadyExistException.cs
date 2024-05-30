using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException(string? message) : base(message)
        {
        }
    }
}