using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    public class UsernameAlreadyExistException : Exception
    {

        public UsernameAlreadyExistException(string? message) : base(message)
        {
        }

        
    }
}