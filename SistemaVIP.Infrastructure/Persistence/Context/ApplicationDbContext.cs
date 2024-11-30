using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SistemaVIP.Core.Models;

namespace SistemaVIP.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<TerapeutaModel> Terapeutas { get; set; }
        public DbSet<PresentadorModel> Presentadores { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Crear esquema
            builder.HasDefaultSchema("SistemaVIP");

            base.OnModelCreating(builder);

            // Mover todas las tablas de Identity al esquema SistemaVIP
            builder.Entity<IdentityUser>().ToTable("AspNetUsers", "SistemaVIP");
            builder.Entity<IdentityRole>().ToTable("AspNetRoles", "SistemaVIP");
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "SistemaVIP");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "SistemaVIP");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "SistemaVIP");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "SistemaVIP");
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "SistemaVIP");
        }
    }
}