using JewShop.Shared.Dtos;

namespace JewShop.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(UserRegisterRequest request);
        Task<AuthResponse> LoginAsync(UserLoginRequest request);
    }
}