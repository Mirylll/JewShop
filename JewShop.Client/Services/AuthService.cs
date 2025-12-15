using JewShop.Shared.Dtos;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using JewShop.Client.Providers;

namespace JewShop.Client.Services
{
    // 1. INTERFACE: Khai báo tên hàm
    public interface IAuthService
    {
        Task<AuthResponse> Register(UserRegisterRequest request);
        Task<AuthResponse> Login(UserLoginRequest request);
        Task Logout();
        
        // Hai hàm mới cho quên mật khẩu
        Task<bool> ForgotPassword(string email);
        Task<bool> ResetPassword(ResetPasswordRequest request);
    }

    // 2. IMPLEMENTATION: Viết logic chi tiết
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<AuthResponse> Register(UserRegisterRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            return await result.Content.ReadFromJsonAsync<AuthResponse>() 
                   ?? new AuthResponse { Success = false, Message = "Lỗi kết nối" };
        }

        public async Task<AuthResponse> Login(UserLoginRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var response = await result.Content.ReadFromJsonAsync<AuthResponse>()
                   ?? new AuthResponse { Success = false, Message = "Lỗi kết nối" };

            if (response.Success)
            {
                await _localStorage.SetItemAsync("authToken", response.Token);
                ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.Token);
            }

            return response;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        // --- SỬA LỖI: Thêm body đầy đủ cho 2 hàm này ---
        
        public async Task<bool> ForgotPassword(string email)
        {
            // Gửi email dưới dạng JSON string
            var result = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", email);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/reset-password", request);
            return result.IsSuccessStatusCode;
        }
    }
}