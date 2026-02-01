using Microsoft.EntityFrameworkCore;
using PetCare.Web.Models;

namespace PetCare.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Nome).IsRequired().HasMaxLength(150);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(500);

                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.UpdatedAt).IsRequired(false);
            });
        }
    }
}
