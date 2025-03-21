using System;

namespace Game.Scripts.Foyer.Network.WebSocket
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