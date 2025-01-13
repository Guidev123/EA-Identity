using IdentityService.API.Data;
using IdentityService.API.DTOs;
using IdentityService.API.Extensions;
using IdentityService.API.Models;
using IdentityService.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLib.Tokens.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityService.API.Services
{
    public class TokenGeneratorService(UserManager<IdentityUser> userManager,
                                       IHttpContextAccessor accessor,
                                       IJwtService jwksService,
                                       IOptions<AppTokenSettings> tokenSettings,
                                       AuthenticationDbContext context) : ITokenGeneratorService
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IHttpContextAccessor _accessor = accessor;
        private readonly IJwtService _jwksService = jwksService;
        private readonly AppTokenSettings _tokenSettings = tokenSettings.Value;
        private readonly AuthenticationDbContext _context = context;    

        public async Task<string> EncodingToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            string currentIssuer = $"{_accessor.HttpContext?.Request.Scheme}://{_accessor.HttpContext?.Request.Host}";

            var key = await _jwksService.GetCurrentSigningCredentials();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = key
            });

            return tokenHandler.WriteToken(token);
        }

        public async Task<LoginResponseDTO> JwtGenerator(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user!);

            var identityClaims = await GetUserClaims(claims, user!);
            var encodedToken = await EncodingToken(identityClaims);

            var refreshToken = await GenerateRefreshToken(email);

            return GetTokenResponse(encodedToken, user!, claims, refreshToken);
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

        public LoginResponseDTO GetTokenResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims, RefreshToken refreshToken) =>
            new()
            {
                AccessToken = encodedToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
                UserToken = new UserTokenDTO
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Claims = claims.Select(c => new ClaimDTO { Type = c.Type, Value = c.Value })
                }
            };

        private static long ToUnixEpochDate(DateTime date)
           => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        public async Task<RefreshToken> GenerateRefreshToken(string email)
        {
            var refreshToken = new RefreshToken(email, DateTime.Now);
            refreshToken.SetExpirationDate(_tokenSettings.RefreshTokenExpirationInHours);

            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(rt => rt.UserIdentification == email));
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken?> GetRefreshToken(Guid refreshToken)
        {
            var token = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            return token is not null
                && token.ExpirationDate > DateTime.Now
                ? token
                : null;
        }
    }
}
