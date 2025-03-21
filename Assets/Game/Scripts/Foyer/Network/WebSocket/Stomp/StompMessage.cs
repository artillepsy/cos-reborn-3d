using System.Collections.Generic;

namespace Game.Scripts.Foyer.Network.WebSocket.Stomp
{
    public class StompMessage
    {
        public string Body { get; }

        public string Command { get; }

        public Dictionary<string, string> Headers { get; }

        public StompMessage(string command)
            : this(command, string.Empty)
        {
        }

        public StompMessage(string command, string body)
            : this(command, body, new Dictionary<string, string>())
        {
        }

        internal StompMessage(string command, string body, Dictionary<string, string> headers)
        {
            Command = command;
            Body = body;
            Headers = headers;

            this["content-length"] = body.Length.ToString();
        }

        public string this[string header]
        {
            get => Headers.ContainsKey(header) ? Headers[header] : string.Empty;
            set => Headers[header] = value;
        }
    }
}