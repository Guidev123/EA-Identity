using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedLib.Tokens.Core.Models;
using SharedLib.Tokens.EntityFramework;
using System.Reflection;

namespace IdentityService.API.Data;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
           : IdentityDbContext(options), ISecurityKeyContext
{
    public DbSet<KeyMaterial> SecurityKeys { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
