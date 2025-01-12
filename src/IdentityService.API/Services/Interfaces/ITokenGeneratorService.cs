﻿using IdentityService.API.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityService.API.Services.Interfaces
{
    public interface ITokenGeneratorService
    {
        Task<LoginResponseDTO> JwtGenerator(IdentityUser email);
        Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user);
        string EncodingToken(ClaimsIdentity identityClaims);
        LoginResponseDTO GetTokenResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims);
    }
}
