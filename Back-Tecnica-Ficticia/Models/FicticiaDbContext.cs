using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Tecnica_FicticiaSA.Models;

public partial class FicticiaDbContext : DbContext
{
    public FicticiaDbContext()
    {
    }

    public FicticiaDbContext(DbContextOptions<FicticiaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AtributosAdicionale> AtributosAdicionales { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AtributosAdicionale>(entity =>
        {
            entity.HasKey(e => e.AaId).HasName("PK__ATRIBUTO__D9B919EA478DFB22");

            entity.ToTable("ATRIBUTOS_ADICIONALES");

            entity.Property(e => e.AaId).HasColumnName("aa_id");
            entity.Property(e => e.AaAtributo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("aa_atributo");
            entity.Property(e => e.AaClienteId).HasColumnName("aa_cliente_id");

            entity.HasOne(d => d.AaCliente).WithMany(p => p.AtributosAdicionales)
                .HasForeignKey(d => d.AaClienteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AA_Cliente");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__CLIENTES__47E34D64CE143030");

            entity.ToTable("CLIENTES");

            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.ClienteDiabetico).HasColumnName("cliente_diabetico");
            entity.Property(e => e.ClienteEdad).HasColumnName("cliente_edad");
            entity.Property(e => e.ClienteEstado).HasColumnName("cliente_estado");
            entity.Property(e => e.ClienteGenero)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cliente_Genero");
            entity.Property(e => e.ClienteIdentificacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cliente_identificacion");
            entity.Property(e => e.ClienteLentes).HasColumnName("cliente_lentes");
            entity.Property(e => e.ClienteManeja).HasColumnName("cliente_maneja");
            entity.Property(e => e.ClienteNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cliente_nombre");
            entity.Property(e => e.ClienteOtros).HasColumnName("cliente_otros");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__USUARIOS__2ED7D2AFB55A4130");

            entity.ToTable("USUARIOS");

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.UsuarioClave)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("usuario_clave");
            entity.Property(e => e.UsuarioCorreo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usuario_correo");
            entity.Property(e => e.UsuarioNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usuario_nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
