using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException(string? message) : base(message)
        {
        }

    }
}