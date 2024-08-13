namespace CustomIdentity.API.DTOs
{
    public partial class UserDTO
    {
        public class ClaimDTO
        {
            public string Value { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
        }
    }
}
