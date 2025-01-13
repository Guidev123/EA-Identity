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

    public Guid Id { get; private set; }
    public string UserIdentification { get; private set; }
    public Guid Token { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public void SetExpirationDate(int expirationInHours) => ExpirationDate.AddHours(expirationInHours);
}
