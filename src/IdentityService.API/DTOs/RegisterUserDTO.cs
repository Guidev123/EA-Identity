using Microsoft.AspNetCore.Identity;

namespace IdentityService.API.DTOs
{
    public class RegisterUserDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public static IdentityUser MapToIdentity(RegisterUserDTO dto) => new()
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true
        };
    }
}
