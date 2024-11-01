namespace IdentityService.API.DTOs
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public UserTokenDTO UserToken { get; set; } = null!;
        public double ExpiresIn { get; set; }
    }
}
