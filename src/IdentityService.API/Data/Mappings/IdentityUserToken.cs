using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API.Data.Mappings
{
    public class IdentityUserTokenMapping : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder.ToTable("IdentityUserToken");
            builder.HasKey(t => t.UserId);
            builder.Property(t => t.LoginProvider).HasMaxLength(120);
            builder.Property(t => t.Name).HasMaxLength(180);
        }
    }
}
