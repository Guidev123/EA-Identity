namespace IdentityService.API.Extensions
{
    public class JsonWebTokenData
    {
        public string Secret { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string ValidAt { get; set; } = string.Empty;
    }
}
