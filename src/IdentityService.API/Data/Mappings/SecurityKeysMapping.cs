using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLib.Tokens.Core.Models;

namespace IdentityService.API.Data.Mappings
{
    public class SecurityKeysMapping : IEntityTypeConfiguration<KeyMaterial>
    {
        public void Configure(EntityTypeBuilder<KeyMaterial> builder)
        {
            builder.ToTable("SecurityKeys");
            builder.HasKey(k => k.Id);
            builder.Property(k => k.KeyId).HasColumnType("VARCHAR").HasMaxLength(200);
            builder.Property(k => k.Type).HasColumnType("VARCHAR").HasMaxLength(160);
            builder.Property(k => k.Parameters).HasColumnType("VARCHAR").HasMaxLength(160);
            builder.Property(k => k.RevokedReason).HasColumnType("VARCHAR").HasMaxLength(160);
        }
    }
}
