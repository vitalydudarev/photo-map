using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Storage.Service.Database.Entities;

namespace Storage.Service.Database
{
    public class FileContext : DbContext
    {
        private readonly IConfiguration _configuration;
        
        public DbSet<File> Files { get; set; }

        public FileContext(IConfiguration configuration, DbContextOptions<FileContext> options)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>()
                .HasKey(a => a.Id);

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