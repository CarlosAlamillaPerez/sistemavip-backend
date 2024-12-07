using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SistemaVIP.Core.Models;
using NetTopologySuite.Geometries;

namespace SistemaVIP.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserModel>
    {
        public DbSet<TerapeutaModel> Terapeutas { get; set; }
        public DbSet<PresentadorModel> Presentadores { get; set; }
        public DbSet<TerapeutasPresentadoresModel> TerapeutasPresentadores { get; set; }
        public DbSet<ServiciosModel> Servicios { get; set; }
        public DbSet<ServiciosTerapeutasModel> ServiciosTerapeutas { get; set; }
        public DbSet<ComisionesModel> Comisiones { get; set; }
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

            // Configurar ComisionesModel
            builder.Entity<ComisionesModel>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Relación con Servicio
                entity.HasOne(c => c.Servicio)
                    .WithMany()
                    .HasForeignKey(c => c.ServicioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Terapeuta
                entity.HasOne(c => c.Terapeuta)
                    .WithMany()
                    .HasForeignKey(c => c.TerapeutaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Presentador
                entity.HasOne(c => c.Presentador)
                    .WithMany()
                    .HasForeignKey(c => c.PresentadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relaciones con usuarios para confirmación y liquidación
                entity.HasOne(c => c.UsuarioConfirmacion)
                    .WithMany()
                    .HasForeignKey(c => c.IdUsuarioConfirmacion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.UsuarioLiquidacion)
                    .WithMany()
                    .HasForeignKey(c => c.IdUsuarioLiquidacion)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configurar las propiedades decimales
                entity.Property(c => c.MontoTotal).HasPrecision(10, 2);
                entity.Property(c => c.MontoTerapeuta).HasPrecision(10, 2);
                entity.Property(c => c.MontoComisionTotal).HasPrecision(10, 2);
                entity.Property(c => c.MontoComisionEmpresa).HasPrecision(10, 2);
                entity.Property(c => c.MontoComisionPresentador).HasPrecision(10, 2);
                entity.Property(c => c.PorcentajeAplicadoEmpresa).HasPrecision(5, 2);
                entity.Property(c => c.PorcentajeAplicadoPresentador).HasPrecision(5, 2);

                // Configurar las propiedades requeridas
                entity.Property(c => c.Estado)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(c => c.NumeroTransaccion)
                    .HasMaxLength(50);

                entity.Property(c => c.ComprobanteUrl)
                    .HasMaxLength(500);

                entity.Property(c => c.NotasPago)
                    .HasMaxLength(500);
            });

            // Configurar la relación TerapeutasPresentadores
            builder.Entity<TerapeutasPresentadoresModel>(entity =>
            {
                entity.HasKey(tp => new { tp.TerapeutaId, tp.PresentadorId });

                entity.HasOne(tp => tp.Terapeuta)
                    .WithMany()
                    .HasForeignKey(tp => tp.TerapeutaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(tp => tp.Presentador)
                    .WithMany()
                    .HasForeignKey(tp => tp.PresentadorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar la relación ServiciosTerapeutas
            builder.Entity<ServiciosTerapeutasModel>(entity =>
            {
                entity.HasKey(st => new { st.ServicioId, st.TerapeutaId });

                entity.HasOne(st => st.Servicio)
                    .WithMany(s => s.ServiciosTerapeutas)
                    .HasForeignKey(st => st.ServicioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(st => st.Terapeuta)
                    .WithMany()
                    .HasForeignKey(st => st.TerapeutaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(st => st.PresentadorConfirmaPago)
                    .WithMany()
                    .HasForeignKey(st => st.IdPresentadorConfirmaPago)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configurar las columnas de geografía
                entity.Property(st => st.UbicacionInicio).HasColumnType("geography");
                entity.Property(st => st.UbicacionFin).HasColumnType("geography");

                // Configurar propiedades decimales
                entity.Property(st => st.MontoTerapeuta).HasPrecision(10, 2);
            });

            // Configurar ServiciosModel
            builder.Entity<ServiciosModel>(entity =>
            {
                entity.HasOne(s => s.Presentador)
                    .WithMany()
                    .HasForeignKey(s => s.PresentadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.UsuarioCancelacion)
                    .WithMany()
                    .HasForeignKey(s => s.IdUsuarioCancelacion)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configurar propiedades decimales
                entity.Property(s => s.MontoTotal).HasPrecision(10, 2);
            });

            // Configurar PresentadorModel
            builder.Entity<PresentadorModel>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configurar propiedades decimales
                entity.Property(p => p.PorcentajeComision).HasPrecision(5, 2);
            });

            // Configurar TerapeutaModel
            builder.Entity<TerapeutaModel>(entity =>
            {
                entity.HasOne(t => t.User)
                    .WithMany()
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configurar propiedades decimales
                entity.Property(t => t.TarifaBase).HasPrecision(10, 2);
                entity.Property(t => t.TarifaExtra).HasPrecision(10, 2);
            });

            // Configurar BitacoraModel
            builder.Entity<BitacoraModel>()
                .HasOne(b => b.Usuario)
                .WithMany()
                .HasForeignKey(b => b.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar BlacklistModel
            builder.Entity<BlacklistModel>()
                .HasOne(b => b.UsuarioRegistro)
                .WithMany()
                .HasForeignKey(b => b.IdUsuarioRegistro)
                .OnDelete(DeleteBehavior.Restrict);

            // Mover todas las tablas de Identity al esquema SistemaVIP
            builder.Entity<ApplicationUserModel>().ToTable("AspNetUsers", "SistemaVIP");
            builder.Entity<IdentityRole>().ToTable("AspNetRoles", "SistemaVIP");
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "SistemaVIP");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "SistemaVIP");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "SistemaVIP");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "SistemaVIP");
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "SistemaVIP");
        }
    }
}