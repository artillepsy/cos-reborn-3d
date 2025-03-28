using System;

namespace Foyer.Network.WebSocket.Stomp
{
    public class StompSubscriptionMessageListener
    {

        public event Action<string> OnMessage;
        
        private readonly StompWebSocket _stompWebSocket;
        private readonly string _subscriptionUrl;
        private string _sessionId = string.Empty;
        

        public StompSubscriptionMessageListener(StompWebSocket stompWebSocket, string subscriptionUrl)
        {
            _stompWebSocket = stompWebSocket;
            _subscriptionUrl = subscriptionUrl;
        }

        public void Subscribe()
        {
            if (_sessionId != string.Empty)
            {
                throw new StompException("The listener is already listening to some subscription channel");
            }

            _stompWebSocket.OnStompMessage += OnStompMessageReceived;
            _sessionId = _stompWebSocket.Subscribe(_subscriptionUrl);
        }

        public void Unsubscribe()
        {
            _stompWebSocket.Unsubscribe(_sessionId);
            _stompWebSocket.OnStompMessage -= OnStompMessageReceived;
            _sessionId = string.Empty;
        }

        private void OnStompMessageReceived(StompMessage stompMessage)
        {
            if (stompMessage["subscription"] != _sessionId)
            {
                return;
            }
            OnMessage?.Invoke(stompMessage.Body);
        }
        
    }
}