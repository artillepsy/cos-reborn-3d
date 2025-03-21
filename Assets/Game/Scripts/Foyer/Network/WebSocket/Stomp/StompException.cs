namespace Game.Scripts.Foyer.Network.WebSocket.Stomp
{
    public class StompException : WebSocketException
    {
        public StompException()
        {
        }

        public StompException(string message) : base(message)
        {
        }
    }
}