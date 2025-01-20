using IdentityService.API.DTOs;

namespace IdentityService.API.Services;

public interface IEmailService 
{
    Task SendAsync(EmailMessageDTO email);
}
