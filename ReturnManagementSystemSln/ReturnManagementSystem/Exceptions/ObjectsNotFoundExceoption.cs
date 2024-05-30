using System.Runtime.Serialization;

namespace ReturnManagementSystem.Exceptions
{
    [Serializable]
    public class ObjectsNotFoundException : Exception
    {
        public ObjectsNotFoundException(string? message) : base(message)
        {
        }
    }
}