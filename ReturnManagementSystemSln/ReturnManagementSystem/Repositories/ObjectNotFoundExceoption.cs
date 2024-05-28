using System.Runtime.Serialization;

namespace ReturnManagementSystem.Repositories
{
    [Serializable]
    internal class ObjectNotFoundExceoption : Exception
    {
        public ObjectNotFoundExceoption()
        {
        }

        public ObjectNotFoundExceoption(string? message) : base(message)
        {
        }

        public ObjectNotFoundExceoption(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ObjectNotFoundExceoption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}