using IdentityService.API.DTOs;
using SharedLib.Domain.Messages.Integration.DeletedUser;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Response<LoginResponseDTO>> RegisterAsync(RegisterUserDTO dto);
        Task<Response<LoginResponseDTO>> LoginAsync(LoginUserDTO dto);
        Task<Response<ChangeUserPasswordDTO>> ChangePasswordAsync(ChangeUserPasswordDTO dto);
        Task<Response<DeletedUserIntegrationEvent>> DeleteAsync(Guid id);
    }
}
