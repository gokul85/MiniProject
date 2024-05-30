using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    public class InvalidSerialNumber : Exception
    {
        public InvalidSerialNumber(string? message) : base(message)
        {
        }

    }
}