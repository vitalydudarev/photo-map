using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PhotoMap.Api.Database.Configurations;
using PhotoMap.Api.Database.Entities;

namespace PhotoMap.Api.Database
{
    public class PhotoMapContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public PhotoMapContext(IConfiguration configuration, DbContextOptions<PhotoMapContext> options)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserTypeConfiguration());

            modelBuilder.Entity<Photo>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Photo>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            SetDateTimeConverters(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration == null || optionsBuilder.IsConfigured)
                return;

            optionsBuilder.UseNpgsql(_configuration["ConnectionString"]);
        }

        /// <summary>
        /// Set converters for DateTime properties so that all values will be written to and read from database as UTC.
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder object.</param>
        private static void SetDateTimeConverters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .Property<DateTime>(property.Name)
                            .HasConversion(
                                dt => dt.ToUniversalTime(),
                                dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .Property<DateTime?>(property.Name)
                            .HasConversion(
                                dt => dt.HasValue ? dt.Value.ToUniversalTime() : dt,
                                dt => dt.HasValue ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : dt);
                    }
                }
            }
        }
    }
}
