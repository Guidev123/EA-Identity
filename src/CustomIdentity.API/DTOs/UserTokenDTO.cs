namespace CustomIdentity.API.DTOs
{
    public partial class UserDTO
    {
        public class UserTokenDTO
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public IEnumerable<ClaimDTO> Claims { get; set; } = [];
        }
    }
}
