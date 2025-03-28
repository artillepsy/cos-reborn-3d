namespace Foyer.Network.WebSocket.Stomp
{
    public static class StompCommand
    {
        public const string Connect = "CONNECT";
        public const string Disconnect = "DISCONNECT";
        public const string Subscribe = "SUBSCRIBE";
        public const string Unsubscribe = "UNSUBSCRIBE";
        public const string Send = "SEND";
        
        public const string Connected = "CONNECTED";
        public const string Message = "MESSAGE";
        public const string Receipt = "RECEIPT";
        public const string Error = "ERROR";
    }
}