namespace IdentityService.API.DTOs
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public Guid RefreshToken { get; set; }
        public UserTokenDTO UserToken { get; set; } = null!;
        public double ExpiresIn { get; set; }
    }
}
