namespace Game.Scripts.Foyer.Network.Http
{
    public static class HttpConfig
    {
        public const string ApplicationJsonContentType = "application/json";
        public const string ContentTypeHeader = "Content-Type";
        public const string AuthorizationHeader = "Authorization";
        public const string BearerJwtPrefix = "Bearer ";
        public static string BaseUrl { get; set; } = "http://localhost:8080";
        public static string AuthenticationToken { get; set; } = null;
    }
}