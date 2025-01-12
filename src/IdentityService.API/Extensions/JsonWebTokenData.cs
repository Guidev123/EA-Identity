namespace IdentityService.API.Extensions
{
    public class JsonWebTokenData
    {
        public string Secret { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}
