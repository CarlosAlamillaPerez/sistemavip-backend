using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SistemaVIP.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {
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
            builder.Entity<IdentityUser>().ToTable("Users", "SistemaVIP");
            builder.Entity<IdentityRole>().ToTable("Roles", "SistemaVIP");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "SistemaVIP");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "SistemaVIP");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "SistemaVIP");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "SistemaVIP");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "SistemaVIP");
        }
    }
}