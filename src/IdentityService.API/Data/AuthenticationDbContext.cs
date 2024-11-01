using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API.Data
{
    public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : IdentityDbContext(options)
    {
    }
}
