using salhoubapi.Models;
using salhoubapi.Models.Dtos;
namespace salhoubapi.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

    }
}
