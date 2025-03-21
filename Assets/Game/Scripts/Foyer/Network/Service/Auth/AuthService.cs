using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Game.Scripts.Foyer.Network.Dto.Auth;
using Game.Scripts.Foyer.Network.Http;
using Game.Scripts.Foyer.Network.WebSocket;
using Newtonsoft.Json;

namespace Game.Scripts.Foyer.Network.Service.Auth
{
    public static class AuthService
    {
        private const string BaseUrl = "/auth";
        public static long UserId { get; private set; }

        public static async Task SignUp(SignUpUserDto signUpUserDto)
        {
            var body = JsonConvert.SerializeObject(signUpUserDto);
            using var request = new HttpRequest($"{BaseUrl}/signup", HttpMethod.Post, body);
            await request.SendWebRequestAsync();
        }

        public static async Task SignIn(SignInUserDto signInUserDto)
        {
            var body = JsonConvert.SerializeObject(signInUserDto);
            using var request = new HttpRequest($"{BaseUrl}/signin", HttpMethod.Post, body);
            var tokenDto = await request.SendWebRequestAsync();
            var jwtTokenDto = JsonConvert.DeserializeObject<JwtResponseDto>(tokenDto);
            Debug.Assert(jwtTokenDto != null, nameof(jwtTokenDto) + " != null");
            HttpConfig.AuthenticationToken = jwtTokenDto.token;
            UserId = jwtTokenDto.userId;
            await StompWebSocket.Instance.Connect();
        }

        public static async Task ConfirmRegistration(string verificationToken)
        {
            var url = $"{BaseUrl}/registration-confirm?token={verificationToken}";
            using var request = new HttpRequest(url, HttpMethod.Get);
            await request.SendWebRequestAsync();
        }

        public static async Task<bool> TestAuth()
        {
            try
            {
                const string url = "/test";
                using var request = new HttpRequest(url, HttpMethod.Get);
                var userName = await request.SendWebRequestAsync();
                return userName != string.Empty;
            }
            catch (Exception)
            {
                HttpConfig.AuthenticationToken = null;
                StompWebSocket.Instance.Disconnect();
                return false;
            }
        }

        public static async Task ForgotPassword(ForgotPasswordDto dto)
        {
            var url = $"{BaseUrl}/forgot-password";
            var jsonDto = JsonConvert.SerializeObject(dto);
            using var request = new HttpRequest(url, HttpMethod.Post, jsonDto);
            await request.SendWebRequestAsync();
        }

        public static async Task ForgotPasswordConfirm(string token)
        {
            var url = $"{BaseUrl}/forgot-password-confirm?token={token}";
            using var request = new HttpRequest(url, HttpMethod.Get);
            await request.SendWebRequestAsync();
        }

        public static async Task ChangePassword(ChangePasswordDto dto)
        {
            var url = $"{BaseUrl}/password";
            var jsonDto = JsonConvert.SerializeObject(dto);
            using var request = new HttpRequest(url, HttpMethod.Put, jsonDto);
            await request.SendWebRequestAsync();
        }

        public static void SignOut()
        {
            StompWebSocket.Instance.Disconnect();
            HttpConfig.AuthenticationToken = null;
        }
    }
}