using AuthService.DTO;
using AuthService.Helpers;
using AuthService.Model;

namespace AuthService.Services
{
    public interface IAuthService
    {
        Task<ApiResponse> LoginAsync(LoginReq entity);
        Task<ApiResponse> RegisterAsync(User entity);
    }
}
