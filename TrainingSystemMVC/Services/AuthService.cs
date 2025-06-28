using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using TrainingSystemMVC.Models;
using TrainingSystemMVC.ViewModel;

namespace TrainingSystemMVC.Services
{
    public class AuthService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthService(HttpClient _httpClient, IConfiguration _configuration, IHttpContextAccessor accessor) 
        {
            
            httpClient = _httpClient;
            configuration = _configuration;
            httpContextAccessor = accessor;
        }
        private void AttachToken() 
        {
            var token = httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

        }
        public async Task<string> RegisterAsync(RegisterViewModel registerVM) 
        {
            var response = await httpClient.PostAsJsonAsync("register", registerVM);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse<string>>();
                return result?.Message ?? "Registration succeeded";
            }
            var error = await response.Content.ReadAsStringAsync();
            return $"Registration failed: {error}";

        }
        public async Task<LoginResultsViewModel> LoginAsync(LoginViewModel loginVM) 
        {
            AttachToken();
            var response = await httpClient.PostAsJsonAsync("login", loginVM);
            if (response.IsSuccessStatusCode) 
            {
                var result = await response.Content.ReadFromJsonAsync<GeneralResponse<LoginResultsViewModel>>();
                return result?.Data;
            }
            return null;
        }
        
        public async Task<string> LogoutAsync() 
        {
            var response = await httpClient.PostAsync("logout", null);
            return response.IsSuccessStatusCode ? "Logged out successfully" : "Logout failed";
        }
    }
}
