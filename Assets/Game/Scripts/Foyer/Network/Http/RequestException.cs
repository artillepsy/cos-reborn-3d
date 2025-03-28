using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Foyer.Network.Http
{
    public class RequestException : Exception
    {
        public RequestException()
        {
        }

        protected RequestException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RequestException(string message) : base(message)
        {
        }

        public RequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}