using EA.CommonLib.MessageBus.Integration.DeleteCustomer;
using EA.CommonLib.Responses;
using IdentityService.API.DTOs;

namespace IdentityService.API.Services
{
    public interface IAuthenticationService
    {
        Task<Response<LoginResponseDTO>> RegisterAsync(RegisterUserDTO dto);
        Task<Response<LoginResponseDTO>> LoginAsync(LoginUserDTO dto);
        Task<Response<ChangeUserPasswordDTO>> ChangePasswordAsync(ChangeUserPasswordDTO dto);
        Task<Response<DeleteCustomerIntegrationEvent>> DeleteAsync(Guid id);
    }
}
