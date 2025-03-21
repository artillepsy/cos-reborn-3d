using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Game.Scripts.Foyer.Network.Http;
using Game.Scripts.Gameplay.Shared.Util;
using UnityEngine;

namespace Game.Scripts.Foyer.Network.Sse
{
    public class SseClient
    {
        public event Action<string> OnMessageEvent;
        public event Action<string> OnErrorEvent;
        public event Action OnCompleteEvent;
        public event Action OnEndEvent;
        public event Action OnBeginEvent;
        
        private const long DefaultReconnectSamplingTimeMillis = 1L * 1000L;
        private const int DefaultReconnectSamplingAttemptNumber = 30;
        private const long DefaultConnectivityRefreshDelaySec = 1L;

        private static readonly string Tag = $"[{nameof(SseClient)}]";

        private const string CacheControlHeader = "Cache-Control";
        private const string EventStreamAccept = "text/event-stream";
        private const string NoCacheControl = "no-cache";
        
        private readonly string _url;
        private readonly HttpMethod _method;
        private readonly string _body;
        private Dictionary<string, string> _headerParams;

        private StreamReader _reader;
        
        private readonly long _connectivityRefreshDelaySec;
        private long _reconnectionCount = 0;

        public SubscribeStreamStatus Status { get; private set; } = SubscribeStreamStatus.NotStarted;

        public bool ShouldRun { get; set; } = true;

        public SseClient(string url, long reconnectSamplingTimeMillis = DefaultReconnectSamplingTimeMillis,
            long connectivityRefreshDelaySec = DefaultConnectivityRefreshDelaySec, HttpMethod method = HttpMethod.Get,
            string body = null)
        {
            _url = url;
            _connectivityRefreshDelaySec = connectivityRefreshDelaySec;
            _method = method;
            _body = body;
        }

        public async Task StartListen()
        {
            TryCatchInvoke(OnBeginEvent);
            while (!ShouldRunBreak())
            {
                try
                {
                    await ConnectToSseService();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{Tag}: Sse connection exception: {e.Message}");
                }

                if (ShouldRunBreak())
                {
                    break;
                }
                Status = SubscribeStreamStatus.Reconnecting;
                if (_reconnectionCount >= DefaultReconnectSamplingAttemptNumber) {
                    throw new Exception("Cannot connect to sampling service. " +
                                        $"Was made {_reconnectionCount} attempts");
                }
                _reconnectionCount++;

                await Task.Delay(TimeSpan.FromSeconds(_connectivityRefreshDelaySec));
            }
            TryCatchInvoke(OnEndEvent);
        }

        public void EndListen()
        {
            ShouldRun = false;
            _reader?.Dispose();
        }

        private bool ShouldRunBreak()
        {
            return !ShouldRun || Status == SubscribeStreamStatus.Complete;
        }

        private async Task ConnectToSseService()
        {
            const string dataPrefix = "data:";
            var httpRequest = WebRequest.CreateHttp(_url);
            if (!string.IsNullOrEmpty(HttpConfig.AuthenticationToken))
            {
                httpRequest.Headers.Add(HttpRequestHeader.Authorization, 
                    HttpConfig.BearerJwtPrefix + HttpConfig.AuthenticationToken);
            }

            httpRequest.Headers.Add(CacheControlHeader, NoCacheControl);
            httpRequest.ContentType = HttpConfig.ApplicationJsonContentType;
            httpRequest.Accept = EventStreamAccept;
            httpRequest.Method = _method.ToString();

            if (!string.IsNullOrEmpty(_body))
            {
                WriteBody(httpRequest, _body);
            }
            
            
            var response = await httpRequest.GetResponseAsync();
            var stream = response.GetResponseStream();
            System.Diagnostics.Debug.Assert(stream != null, nameof(stream) + " != null");
            using (_reader = new StreamReader(stream))
            {
                Status = SubscribeStreamStatus.Success;
                try
                {
                    string line;
                    while ((line = await _reader.ReadLineAsync()) != null)
                    {
                        if (!line.StartsWith(dataPrefix))
                        {
                            continue;
                        }

                        var dataMessage = line.Substring(dataPrefix.Length);
                        var status = GetStatus(dataMessage);
                        if (status == SseStatus.End)
                        {
                            break;
                        }

                        if (status == SseStatus.Error)
                        {
                            throw new Exception("Sse status error");
                        }
                        TryCatchInvoke(OnMessageEvent, GetData(dataMessage));
                    }

                    Status = SubscribeStreamStatus.Complete;
                    TryCatchInvoke(OnCompleteEvent);
                }
                catch (Exception e)
                {
                    if (e.Message.StartsWith("Expecting chunk trailer"))
                    {
                        Status = SubscribeStreamStatus.Complete;
                        TryCatchInvoke(OnCompleteEvent);
                        return;
                    }
                    Status = SubscribeStreamStatus.Failing;
                    TryCatchInvoke(OnErrorEvent, e.Message);
                }
                
            }
        }

        private static void WriteBody(WebRequest request, string body)
        {
            using var writer = new StreamWriter(request.GetRequestStream());
            writer.Write(body);
        }
        

        private static void TryCatchInvoke(Action<string> messageDelegate, string message)
        {
            try
            {
                messageDelegate?.Invoke(message);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
            
        }

        private static void TryCatchInvoke(Action voidDelegate)
        {
            try
            {
                voidDelegate?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
            
        }
        

        private static SseStatus GetStatus(string msg)
        {
            
            const string statusPrefix = "\"status\":";
            var startIndex = msg.IndexOf(statusPrefix, StringComparison.Ordinal) + statusPrefix.Length;
            var i = startIndex;
            var length = 0;
            var doubleQuotes = 0;
            while (doubleQuotes != 2)
            {
                if (msg[i] == '"')
                {
                    doubleQuotes++;
                }
                i++;
                length++;
            }

            msg = msg.Substring(startIndex + 1, length - 2);
            var parsed = Enum.TryParse(msg, out SseStatus status);
            return status;
        }

        private static string GetData(string msg)
        {
            const string dataPrefix = "\"data\":";
            var openedBrackets = 0;
            var startIndex = msg.IndexOf(dataPrefix, StringComparison.Ordinal) + dataPrefix.Length;
            var i = startIndex;
            var length = 0; 
            while (openedBrackets != -1)
            {
                switch (msg[i])
                {
                    case '{':
                        openedBrackets++;
                        break;
                    case '}':
                        openedBrackets--;
                        break;
                        
                }
                length++;
                i++;
            }
            return msg.Substring(startIndex, length - 1);;
        }

        private enum SseStatus
        {
            Normal, End, Error
        }
    }
}