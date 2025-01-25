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
        public DbSet<ComprobantePagoModel> ComprobantesPago { get; set; }
        public DbSet<CancelacionesPresentadorModel> CancelacionesPresentador { get; set; }
        public DbSet<ServicioExtraCatalogoModel> ServiciosExtraCatalogo { get; set; }
        public DbSet<ServicioExtraModel> ServiciosExtra { get; set; }

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
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con Terapeuta
                entity.HasOne(c => c.Terapeuta)
                    .WithMany()
                    .HasForeignKey(c => c.TerapeutaId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con Presentador
                entity.HasOne(c => c.Presentador)
                    .WithMany()
                    .HasForeignKey(c => c.PresentadorId)
                    .OnDelete(DeleteBehavior.NoAction);

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
                    .WithMany(t => t.TerapeutasPresentadores)
                    .HasForeignKey(tp => tp.TerapeutaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(tp => tp.Presentador)
                    .WithMany(p => p.TerapeutasPresentadores)
                    .HasForeignKey(tp => tp.PresentadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(tp => tp.Estado)
                    .IsRequired();
            });

            // Configurar la relación ServiciosTerapeutas
            builder.Entity<ServiciosTerapeutasModel>(entity =>
            {
                // Cambiar la clave primaria
                entity.HasKey(st => st.Id);

                // Agregar el índice único compuesto
                entity.HasIndex(st => new { st.ServicioId, st.TerapeutaId })
                     .IsUnique();

                // Relación con Servicio
                entity.HasOne(st => st.Servicio)
                    .WithMany(s => s.ServiciosTerapeutas)
                    .HasForeignKey(st => st.ServicioId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con Terapeuta
                entity.HasOne(st => st.Terapeuta)
                    .WithMany()
                    .HasForeignKey(st => st.TerapeutaId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Mantener configuraciones de geografía
                entity.Property(st => st.UbicacionInicio).HasColumnType("geography");
                entity.Property(st => st.UbicacionFin).HasColumnType("geography");

                // Mantener configuraciones decimales
                entity.Property(st => st.MontoTerapeuta).HasPrecision(10, 2);

                // Mantener constraint de validación
                entity.HasCheckConstraint(
                    "CK_ServiciosTerapeutas_GastosTransporte",
                    "([GastosTransporte] IS NULL AND EXISTS (SELECT 1 FROM Servicios s WHERE s.Id = ServicioId AND s.TipoUbicacion = 'CONSULTORIO')) OR " +
                    "([GastosTransporte] IS NOT NULL AND EXISTS (SELECT 1 FROM Servicios s WHERE s.Id = ServicioId AND s.TipoUbicacion = 'DOMICILIO'))");
            });

            // Agregar configuración para ComprobantesPago
            builder.Entity<ComprobantePagoModel>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.TipoComprobante)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(c => c.NumeroOperacion)
                    .HasMaxLength(50);

                entity.Property(c => c.UrlComprobante)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(c => c.Estado)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(c => c.NotasComprobante)
                    .HasMaxLength(500);

                // Relación con ServiciosTerapeutas
                entity.HasOne(c => c.ServicioTerapeuta)
                    .WithMany(st => st.ComprobantesPago)
                    .HasForeignKey(c => c.ServicioTerapeutaId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con Usuario
                entity.HasOne(c => c.UsuarioRegistro)
                    .WithMany()
                    .HasForeignKey(c => c.IdUsuarioRegistro)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice único para NumeroOperacion
                entity.HasIndex(c => c.NumeroOperacion)
                    .IsUnique()
                    .HasFilter("[NumeroOperacion] IS NOT NULL");

                entity.Property(cp => cp.Monto)
                    .HasPrecision(10, 2);
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

                    entity.Property(s => s.GastosTransporte).HasPrecision(10, 2);
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

            builder.Entity<CancelacionesPresentadorModel>(entity =>
            {
                entity.HasKey(e => e.PresentadorId);

                entity.HasOne(e => e.Presentador)
                    .WithMany()
                    .HasForeignKey(e => e.PresentadorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(e => e.NombrePresentador)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            // Configuración para ServicioExtraCatalogo
            builder.Entity<ServicioExtraCatalogoModel>(entity =>
            {
                entity.ToTable("ServiciosExtraCatalogo", "SistemaVIP");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);
            });

            // Configuración para ServicioExtra
            builder.Entity<ServicioExtraModel>(entity =>
            {
                entity.ToTable("ServiciosExtra", "SistemaVIP");

                entity.Property(e => e.Monto)
                    .HasPrecision(10, 2);

                entity.Property(e => e.Notas)
                    .HasMaxLength(500);

                entity.HasOne(e => e.ServicioTerapeuta)
                    .WithMany(st => st.ServiciosExtra)
                    .HasForeignKey(e => e.ServicioTerapeutaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ServicioExtraCatalogo)
                    .WithMany()
                    .HasForeignKey(e => e.ServicioExtraCatalogoId)
                    .OnDelete(DeleteBehavior.Restrict);
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


//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity;
//using SistemaVIP.Core.Models;

//namespace SistemaVIP.Infrastructure.Persistence.Context
//{
//    public class ApplicationDbContext : IdentityDbContext<ApplicationUserModel>
//    {
//        public DbSet<TerapeutaModel> Terapeutas { get; set; } = default!;
//        public DbSet<PresentadorModel> Presentadores { get; set; } = default!;
//        public DbSet<TerapeutasPresentadoresModel> TerapeutasPresentadores { get; set; } = default!;
//        public DbSet<ServiciosModel> Servicios { get; set; } = default!;
//        public DbSet<ServiciosTerapeutasModel> ServiciosTerapeutas { get; set; } = default!;
//        public DbSet<ComisionesModel> Comisiones { get; set; } = default!;
//        public DbSet<BlacklistModel> Blacklist { get; set; } = default!;
//        public DbSet<BitacoraModel> Bitacora { get; set; } = default!;
//        public DbSet<ComprobantePagoModel> ComprobantesPago { get; set; } = default!;

//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//        {
//        }

//        protected override void OnModelCreating(ModelBuilder builder)
//        {
//            builder.HasDefaultSchema("SistemaVIP");
//            base.OnModelCreating(builder);

//            // Configuración para ComisionesModel
//            builder.Entity<ComisionesModel>(entity =>
//            {
//                entity.HasKey(e => e.Id);

//                entity.Property(c => c.MontoTotal).HasPrecision(18, 2);
//                entity.Property(c => c.MontoTerapeuta).HasPrecision(18, 2);
//                entity.Property(c => c.MontoComisionTotal).HasPrecision(18, 2);
//                entity.Property(c => c.MontoComisionEmpresa).HasPrecision(18, 2);
//                entity.Property(c => c.MontoComisionPresentador).HasPrecision(18, 2);
//                entity.Property(c => c.PorcentajeAplicadoEmpresa).HasPrecision(5, 2);
//                entity.Property(c => c.PorcentajeAplicadoPresentador).HasPrecision(5, 2);
//            });

//            // Configuración para ComprobantePagoModel
//            builder.Entity<ComprobantePagoModel>(entity =>
//            {
//                entity.HasKey(c => c.Id);
//                entity.Property(c => c.Monto).HasPrecision(18, 2);
//            });

//            // Configuración para ServiciosModel
//            builder.Entity<ServiciosModel>(entity =>
//            {
//                entity.Property(s => s.MontoTotal).HasPrecision(18, 2);
//                entity.Property(s => s.GastosTransporte).HasPrecision(18, 2);
//            });

//            // Configuración para ServiciosTerapeutasModel
//            builder.Entity<TerapeutasPresentadoresModel>(entity =>
//            {
//                entity.HasNoKey();

//                entity.HasOne(tp => tp.Terapeuta)
//                    .WithMany()
//                    .HasForeignKey(tp => tp.TerapeutaId)
//                    .OnDelete(DeleteBehavior.NoAction);

//                entity.HasOne(tp => tp.Presentador)
//                    .WithMany()
//                    .HasForeignKey(tp => tp.PresentadorId)
//                    .OnDelete(DeleteBehavior.NoAction);
//            });


//            // Configuración para PresentadorModel
//            builder.Entity<PresentadorModel>(entity =>
//            {
//                entity.Property(p => p.PorcentajeComision).HasPrecision(5, 2);
//            });

//            // Configuración para TerapeutaModel
//            builder.Entity<TerapeutaModel>(entity =>
//            {
//                entity.Property(t => t.TarifaBase).HasPrecision(18, 2);
//                entity.Property(t => t.TarifaExtra).HasPrecision(18, 2);
//            });

//            // Configuración adicional para Identity
//            builder.Entity<ApplicationUserModel>().ToTable("AspNetUsers", "SistemaVIP");
//            builder.Entity<IdentityRole>().ToTable("AspNetRoles", "SistemaVIP");
//            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "SistemaVIP");
//            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "SistemaVIP");
//            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "SistemaVIP");
//            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "SistemaVIP");
//            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "SistemaVIP");
//        }
//    }
//}

