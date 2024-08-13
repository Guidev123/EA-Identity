namespace CustomIdentity.API.DTOs
{
    public partial class UserDTO
    {
        public class LoginResponseDTO
        {
            public string AccessToken { get; set; } = string.Empty;
            public double ExpiresIn { get; set; }
            public UserTokenDTO UserToken { get; set; } = null!;
        }
    }
}
