using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Foyer.Network.Http
{
    public class HttpRequest : IDisposable
    {

        private readonly HttpMethod _method;
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        public string Body
        {
            get => _body;
            set
            {
                _body = value;
                if (value == null)
                {
                    return;
                }
                var rawData = new System.Text.UTF8Encoding().GetBytes(value);
                _request.uploadHandler = new UploadHandlerRaw(rawData);
            }
        }

        private string _body;
        private readonly UnityWebRequest _request;

        public HttpRequest(string url, HttpMethod httpMethod, string body = null)
        {
            _request = new UnityWebRequest(HttpConfig.BaseUrl + url, httpMethod.ToString());
            _request.SetRequestHeader(HttpConfig.ContentTypeHeader, HttpConfig.ApplicationJsonContentType);
            if (!string.IsNullOrEmpty(HttpConfig.AuthenticationToken))
            {
                _request.SetRequestHeader(HttpConfig.AuthorizationHeader, 
                    HttpConfig.BearerJwtPrefix + HttpConfig.AuthenticationToken);
            }
            Body = body;
            _request.downloadHandler = new DownloadHandlerBuffer();
        }

        public void AddHeader(string header, string value)
        {
            _headers.Add(header, value);
        }

        public async Task<string> SendWebRequestAsync()
        {
            ApplyCustomHeaders();
            await _request.SendWebRequest();
            if (_request.result == UnityWebRequest.Result.Success)
            {
                return _request.downloadHandler.text;
            }
            var errorMsg = _request.downloadHandler.text;
            var isErrorMsg = !string.IsNullOrEmpty(errorMsg);
            var errorMsgDescription = isErrorMsg ? ": " + errorMsg : "";
            throw new HttpRequestException($"{_request.error}{errorMsgDescription}");
        }

        private void ApplyCustomHeaders()
        {
            foreach (var (key, value) in _headers)
            {
                _request.SetRequestHeader(key, value);
            }
        }

        public void Dispose()
        {
            _request.Dispose();
        }
    }
}