using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static CustomIdentity.API.DTOs.UserDTO;

namespace CustomIdentity.API.Security
{
    public interface IJasonWebToken
    {
        Task<LoginResponseDTO> JwtGenerator(string email);
        Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user);
        string EncodingToken(ClaimsIdentity identityClaims);   
        LoginResponseDTO GetTokenResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims);
    }
}
