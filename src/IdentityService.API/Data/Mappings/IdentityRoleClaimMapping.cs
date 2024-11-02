using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.API.Data.Mappings
{
    public class IdentityRoleClaimMapping : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.ToTable("IdentityRoleClaim");
            builder.HasKey(rc => rc.Id);
            builder.Property(u => u.ClaimType).HasMaxLength(255);
            builder.Property(u => u.ClaimValue).HasMaxLength(255);
        }
    }
}
