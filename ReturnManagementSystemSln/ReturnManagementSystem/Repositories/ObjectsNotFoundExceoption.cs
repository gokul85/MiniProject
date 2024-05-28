using System.Runtime.Serialization;

namespace ReturnManagementSystem.Repositories
{
    [Serializable]
    internal class ObjectsNotFoundExceoption : Exception
    {
        public ObjectsNotFoundExceoption()
        {
        }

        public ObjectsNotFoundExceoption(string? message) : base(message)
        {
        }

        public ObjectsNotFoundExceoption(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ObjectsNotFoundExceoption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}