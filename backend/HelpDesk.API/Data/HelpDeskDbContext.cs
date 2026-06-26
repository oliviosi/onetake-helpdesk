using HelpDesk.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Data;

public class HelpDeskDbContext : DbContext
{
    public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options)
        : base(options)
    {
    }

    public DbSet<Empresa> Empresa => Set<Empresa>();
    public DbSet<Usuario> Usuario => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("Empresa");

            entity.HasKey(e => e.IdEmpresa);

            entity.Property(e => e.IdEmpresa)
                .ValueGeneratedNever();

            entity.Property(e => e.DscEmpresa)
                .HasMaxLength(200);

            entity.Property(e => e.Cnpj)
                .HasMaxLength(20);

            entity.Property(e => e.Endereco)
                .HasMaxLength(300);

            entity.Property(e => e.Telefone)
                .HasMaxLength(30);

            entity.Property(e => e.Email)
                .HasMaxLength(200);

            entity.Property(e => e.MensagemAtendimento);

            entity.Property(e => e.CodigoAcesso)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Ativo)
                .HasMaxLength(1)
                .HasDefaultValue("S");

            entity.HasIndex(e => e.CodigoAcesso)
                .IsUnique();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");

            entity.HasKey(e => new { e.IdEmpresa, e.IdUsuario });

            entity.Property(e => e.IdEmpresa)
                .IsRequired();

            entity.Property(e => e.IdUsuario)
                .ValueGeneratedNever();

            entity.Property(e => e.NomeUsuario)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.IdCliente);

            entity.Property(e => e.Ativo)
                .HasMaxLength(1)
                .HasDefaultValue("S");

            entity.Property(e => e.Senha)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.NomeCompleto)
                .HasMaxLength(200);

            entity.Property(e => e.Telefone)
                .HasMaxLength(30);

            entity.Property(e => e.Email)
                .HasMaxLength(200);

            entity.Property(e => e.EhAdm)
                .HasMaxLength(1)
                .HasDefaultValue("N");

            entity.HasOne(e => e.Empresa)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(e => e.IdEmpresa);

            entity.HasIndex(e => new { e.IdEmpresa, e.NomeUsuario })
                .IsUnique();
        });
    }
}