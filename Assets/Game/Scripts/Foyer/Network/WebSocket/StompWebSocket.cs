using System;
using System.Threading;
using System.Threading.Tasks;
using Foyer.Network.Http;
using Foyer.Network.WebSocket.Stomp;
using Shared.Logging;
using UnityEngine;
using WebSocketSharp;

namespace Foyer.Network.WebSocket
{
    public class StompWebSocket
    {
        public event EventHandler OnSocketOpen;
        public event EventHandler OnSocketClose;    
        public event Action<StompMessage> OnStompMessage;
        public event Action<StompMessage> OnStompError;

        public static readonly StompWebSocket Instance;

        private const string SubscriptionPrefix  = "Sub-";
        private const string DisconnectReceipt = "Disconnect";

        private const string StompHeaderId = "id";
        private const string StompHeaderDestination = "destination";
        private const string StompHeaderContentType = "content-type";
        private const string StompHeaderAcceptVersion = "accept-version";
        private const string StompHeaderHost = "host";
        private const string StompHeaderHeartBeat = "heart-beat";
        private const string StompHeaderReceipt = "receipt";
        private const string StompHeaderReceiptId = "receipt-id";
        private const string StompHeaderMessage = "message";
        private const string StompCustomHeaderAuthorization = "Authorization";

        private readonly WebSocketSharp.WebSocket _webSocket;
        private readonly StompMessageSerializer _serializer = new StompMessageSerializer();
        private static long _subscriptionIndex = 0;

        public string SupportedStompVersion { get; set; } = "1.0,1.1,1.2";

        private readonly SynchronizationContext _mainSynchronizationContext;

        private SemaphoreSlim _connectSignal;

        private readonly ErrorState _errorState = new ErrorState();

        static StompWebSocket()
        {
            Instance = new StompWebSocket("ws://localhost:8080/ws");
        }

        private StompWebSocket(string url)
        {
            _webSocket = new WebSocketSharp.WebSocket(url);
            _webSocket.OnOpen += ws_OnOpen;
            _webSocket.OnClose += ws_OnClose;
            _webSocket.OnMessage += ws_OnMessage;
            _webSocket.OnError += ws_OnError;
            _mainSynchronizationContext = SynchronizationContext.Current;
        }

        public async Task Connect()
        {
            _connectSignal = new SemaphoreSlim(0, 1);
            _webSocket.ConnectAsync();
            await _connectSignal.WaitAsync();
            if (_errorState.IsError)
            {
                var message = _errorState.ErrorMessage == string.Empty
                    ? "Cannot connect to the server's socket"
                    : _errorState.ErrorMessage;
                _errorState.IsError = false;
                _errorState.ErrorMessage = string.Empty;
                throw new WebSocketConnectionException(message);
            }
        }

        public string Subscribe(string url)
        {
            var subscriptionId = $"{SubscriptionPrefix}{++_subscriptionIndex}";
            var subscribeMsg = new StompMessage(StompCommand.Subscribe)
            {
                [StompHeaderId] = subscriptionId, 
                [StompHeaderDestination] = url
            };
            _webSocket.Send(_serializer.Serialize(subscribeMsg));
            return subscriptionId;
        }

        public void Unsubscribe(string subscriptionId)
        {
            var unsubscribeMsg = new StompMessage(StompCommand.Unsubscribe)
            {
                [StompHeaderId] = subscriptionId
            };
            _webSocket.Send(_serializer.Serialize(unsubscribeMsg));
        }

        public void Send(string url, string body)
        {
            var broad = new StompMessage(StompCommand.Send, body)
            {
                [StompHeaderContentType] = "application/json;charset=utf-8", 
                [StompHeaderDestination] = url
            };
            _webSocket.Send(_serializer.Serialize(broad));
        }

        private void ConnectStomp()
        {
            var connectMsg = new StompMessage(StompCommand.Connect)
            {
                [StompHeaderAcceptVersion] = SupportedStompVersion, 
                [StompHeaderHost] = "", 
                [StompHeaderHeartBeat] = "0,10000",
                [StompCustomHeaderAuthorization] = HttpConfig.AuthenticationToken
            };
            _webSocket.Send(_serializer.Serialize(connectMsg));
        }

        public void Disconnect()
        {
            var disconnectMsg = new StompMessage(StompCommand.Disconnect)
            {
                [StompHeaderReceipt] = DisconnectReceipt
            };
            _webSocket.Send(_serializer.Serialize(disconnectMsg));
            
        }

        private void Close()
        {
            _webSocket.Close();
        }

        private void ws_OnOpen(object sender, EventArgs e)
        {
            ConnectStomp();
            _mainSynchronizationContext.Post(state =>
            {
                OnSocketOpen?.Invoke(sender, e);
            }, null);
        }

        private void ws_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = _serializer.Deserialize(e.Data);
            switch (msg.Command)
            {
                case StompCommand.Connected:
                    HandleStompConnected(msg);
                    break;
                case StompCommand.Receipt:
                    HandleStompReceipt(msg);
                    break;
                case StompCommand.Message:
                    HandleStompMessage(msg);
                    break;
                case StompCommand.Error:
                    HandleStompError(msg);
                    break;
            }
        }

        private void HandleStompConnected(StompMessage connectedMessage)
        {
            ReleaseAndDisposeConnectSignal();
        }

        private void HandleStompMessage(StompMessage message)
        {
            _mainSynchronizationContext.Post(state =>
            {
                OnStompMessage?.Invoke(message);
            }, null);
        }

        private void HandleStompError(StompMessage errorMessage)
        {
            _mainSynchronizationContext.Post(state =>
            {
                _errorState.IsError = true;
                var errorCause = errorMessage[StompHeaderMessage];
                _errorState.ErrorMessage = errorCause;
                Log.Err("Stomp", errorCause);
                ReleaseAndDisposeConnectSignal();
                OnStompError?.Invoke(errorMessage);
            }, null);
            
        }

        private void ReleaseAndDisposeConnectSignal()
        {
            if (_connectSignal == null)
            {
                return;
            }
            _connectSignal.Release();
            _connectSignal.Dispose();
            _connectSignal = null;
        }

        private void HandleStompReceipt(StompMessage stompMessage)
        {
            var stompReceiptId = stompMessage[StompHeaderReceiptId];
            if (stompReceiptId == DisconnectReceipt)
            {
                Close();
            }
        }

        private void ws_OnClose(object sender, CloseEventArgs e)
        {
            _mainSynchronizationContext.Post(state =>
            {
                Log.Err("Socket", "Socket closed");
                OnSocketClose?.Invoke(sender, e);
            }, null);
        }

        private void ws_OnError(object sender, ErrorEventArgs e)
        {
            _mainSynchronizationContext.Post(state =>
            {
                _errorState.IsError = true;
                Debug.LogError($"Socket closed with error: [{e.Exception}] - '{e.Message}'");
            }, null);
        }

        private class ErrorState
        {
            public bool IsError = false;
            public string ErrorMessage = string.Empty;
        }
    }
}