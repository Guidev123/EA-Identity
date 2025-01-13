namespace IdentityService.API.Models;

public class RefreshToken
{
    public RefreshToken(string userIdentification, DateTime expirationDate)
    {
        Id = Guid.NewGuid();
        Token = Guid.NewGuid();
        UserIdentification = userIdentification;
        ExpirationDate = expirationDate;
    }

    public Guid Id { get; set; }
    public string UserIdentification { get; set; } = string.Empty;
    public Guid Token { get; set; }
    public DateTime ExpirationDate { get; set; }
}
