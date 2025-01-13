using IdentityService.API.DTOs;
using IdentityService.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityService.API.Services.Interfaces
{
    public interface ITokenGeneratorService
    {
        Task<LoginResponseDTO> JwtGenerator(string email);
        Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user);
        Task<string> EncodingToken(ClaimsIdentity identityClaims);
        LoginResponseDTO GetTokenResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims, RefreshToken refreshToken);
        Task<RefreshToken> GenerateRefreshToken(string email);
        Task<RefreshToken?> GetRefreshToken(Guid refreshToken);
    }
}
