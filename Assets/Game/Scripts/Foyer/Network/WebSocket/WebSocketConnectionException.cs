
namespace Foyer.Network.WebSocket
{
    public class WebSocketConnectionException : WebSocketException
    {
        public WebSocketConnectionException()
        {
        }

        public WebSocketConnectionException(string message) : base(message)
        {
        }
    }
}