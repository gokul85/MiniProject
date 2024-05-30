using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    public class InvalidReturnRequest : Exception
    {

        public InvalidReturnRequest(string? message) : base(message)
        {
        }
    }
}