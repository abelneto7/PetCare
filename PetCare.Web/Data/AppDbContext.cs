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
        public DbSet<Tutor> Tutores => Set<Tutor>();
        public DbSet<Pet> Pets => Set<Pet>();
        public DbSet<RegistroVacina> RegistrosVacinas => Set<RegistroVacina>();

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

            modelBuilder.Entity<Tutor>(entity =>
            {
                entity.Property(t => t.Nome).IsRequired().HasMaxLength(150);
                entity.Property(t => t.Telefone).IsRequired().HasMaxLength(20);
                entity.Property(t => t.Email).HasMaxLength(200);
                entity.Property(t => t.Endereco).HasMaxLength(300);

                entity.Property(t => t.CreatedAt).IsRequired();
                entity.Property(t => t.UpdatedAt).IsRequired(false);
            });

            modelBuilder.Entity<Pet>(entity =>
            {
                entity.Property(p => p.Nome).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Raca).HasMaxLength(150);

                entity.Property(p => p.Especie).IsRequired();

                entity.Property(p => p.CreatedAt).IsRequired();
                entity.Property(p => p.UpdatedAt).IsRequired(false);

                entity.HasOne(p => p.Tutor)
                      .WithMany()
                      .HasForeignKey(p => p.TutorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RegistroVacina>(entity =>
            {
                entity.Property(v => v.NomeVacina).IsRequired().HasMaxLength(150);
                entity.Property(v => v.Observacao).HasMaxLength(500);

                entity.Property(v => v.DataAplicacao).IsRequired();
                entity.Property(v => v.IntervaloDias).IsRequired();
                entity.Property(v => v.ProximaDose).IsRequired();

                entity.Property(v => v.CreatedAt).IsRequired();
                entity.Property(v => v.UpdatedAt).IsRequired(false);

                entity.HasOne(v => v.Pet)
                    .WithMany(p => p.Vacinas)
                    .HasForeignKey(v => v.PetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAudit()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                }
            }
        }
    }
}
