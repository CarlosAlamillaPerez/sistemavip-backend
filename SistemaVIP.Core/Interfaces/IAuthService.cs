using SistemaVIP.Core.DTOs.Auth;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<bool> ValidateAuthenticationAsync(string userId);
        Task LogoutAsync(string userId);
        Task<UserDto> GetCurrentUserAsync(string userId);
    }
}