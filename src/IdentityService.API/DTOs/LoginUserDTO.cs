using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.DTOs
{
    public class LoginUserDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public static IdentityUser MapToIdentity(LoginUserDTO dto) => new()
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true
        };
    }
}
