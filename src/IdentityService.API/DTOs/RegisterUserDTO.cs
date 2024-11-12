using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.DTOs
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "The {0} field is required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "The {0} field is required")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "The {0} field is required")]
        [EmailAddress(ErrorMessage = "The {0} field is in an invalid format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The {0} field is required")]
        [StringLength(100, ErrorMessage = "The {0} field must be between {2} and {1} characters", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public static IdentityUser MapToIdentity(RegisterUserDTO dto) => new()
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true
        };
    }
}
