using IdentityService.API.Models;
using KeyPairJWT.Core.Models;
using KeyPairJWT.EntityFramework;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IdentityService.API.Data;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
           : IdentityDbContext(options), ISecurityKeyContext
{
    public DbSet<KeyMaterial> SecurityKeys { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
