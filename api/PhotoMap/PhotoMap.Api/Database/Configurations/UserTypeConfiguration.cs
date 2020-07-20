using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoMap.Api.Database.Entities;

namespace PhotoMap.Api.Database.Configurations
{
    public class UserTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(a => a.Id);
            builder
                .Property(a => a.Name)
                .IsRequired();
            builder
                .Property(a => a.YandexDiskToken);
            builder
                .Property(a => a.YandexDiskTokenExpiresOn);
            builder.ToTable("Users");
        }
    }
}
