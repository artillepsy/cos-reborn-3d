using System;

namespace Foyer.Network.WebSocket
{
    public class WebSocketException : Exception
    {
        public WebSocketException()
        {
        }

        public WebSocketException(string message) : base(message)
        {
        }
    }
}