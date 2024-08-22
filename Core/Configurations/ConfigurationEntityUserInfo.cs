using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations
{
    public class ConfigurationEntityUserInfo : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {

            builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        }
    }
}