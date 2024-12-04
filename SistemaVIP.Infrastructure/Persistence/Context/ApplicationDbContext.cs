using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SistemaVIP.Core.Models;
using NetTopologySuite.Geometries;

namespace SistemaVIP.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<TerapeutaModel> Terapeutas { get; set; }
        public DbSet<PresentadorModel> Presentadores { get; set; }
        public DbSet<TerapeutasPresentadoresModel> TerapeutasPresentadores { get; set; }
        public DbSet<ServiciosModel> Servicios { get; set; }
        public DbSet<ServiciosTerapeutasModel> ServiciosTerapeutas { get; set; }
        public DbSet<ComisionesModel> Comisiones { get; set; }
        public DbSet<ConfiguracionComisionesModel> ConfiguracionComisiones { get; set; }
        public DbSet<PagosModel> Pagos { get; set; }
        public DbSet<BlacklistModel> Blacklist { get; set; }
        public DbSet<BitacoraModel> Bitacora { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Crear esquema
            builder.HasDefaultSchema("SistemaVIP");

            base.OnModelCreating(builder);

            // Configurar la relación TerapeutasPresentadores
            builder.Entity<TerapeutasPresentadoresModel>()
                .HasKey(tp => new { tp.TerapeutaId, tp.PresentadorId });

            builder.Entity<TerapeutasPresentadoresModel>()
                .HasOne(tp => tp.Terapeuta)
                .WithMany()
                .HasForeignKey(tp => tp.TerapeutaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TerapeutasPresentadoresModel>()
                .HasOne(tp => tp.Presentador)
                .WithMany()
                .HasForeignKey(tp => tp.PresentadorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar la relación ServiciosTerapeutas
            builder.Entity<ServiciosTerapeutasModel>()
                .HasKey(st => new { st.ServicioId, st.TerapeutaId });

            builder.Entity<ServiciosTerapeutasModel>()
                .HasOne(st => st.Servicio)
                .WithMany(s => s.ServiciosTerapeutas)
                .HasForeignKey(st => st.ServicioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ServiciosTerapeutasModel>()
                .HasOne(st => st.Terapeuta)
                .WithMany()
                .HasForeignKey(st => st.TerapeutaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar las columnas de geografía
            builder.Entity<ServiciosTerapeutasModel>()
                .Property(st => st.UbicacionInicio)
                .HasColumnType("geography");

            builder.Entity<ServiciosTerapeutasModel>()
                .Property(st => st.UbicacionFin)
                .HasColumnType("geography");

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