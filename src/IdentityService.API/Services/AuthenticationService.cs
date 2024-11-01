using IdentityService.API.DTOs;
using IdentityService.API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.API.Services
{
    public class AuthenticationService(IOptions<JsonWebTokenData> appSettings,
                          UserManager<IdentityUser> userManager) : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly JsonWebTokenData _appSettings = appSettings.Value;

        public string EncodingToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.ValidAt,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiresIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        public async Task<LoginResponseDTO> JwtGenerator(IdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);

            var identityClaims = await GetUserClaims(claims, user);
            var encodedToken = EncodingToken(identityClaims);

            return GetTokenResponse(encodedToken, user, claims);
        }

        public async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

        public LoginResponseDTO GetTokenResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
        {
            return new LoginResponseDTO
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiresIn).TotalSeconds,
                UserToken = new UserTokenDTO
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Claims = claims.Select(c => new ClaimDTO { Type = c.Type, Value = c.Value })
                }
            };
        }
        private static long ToUnixEpochDate(DateTime date)
           => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
