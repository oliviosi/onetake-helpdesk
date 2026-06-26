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
    public DbSet<Cliente> Cliente => Set<Cliente>();
    public DbSet<Produto> Produto => Set<Produto>();
    public DbSet<ProdutoXCliente> ProdutoXCliente => Set<ProdutoXCliente>();
    public DbSet<TipoOcorrencia> TipoOcorrencia => Set<TipoOcorrencia>();
    public DbSet<StatusTicket> StatusTicket => Set<StatusTicket>();
    public DbSet<Tecnico> Tecnico => Set<Tecnico>();
    public DbSet<Ticket> Ticket => Set<Ticket>();
    public DbSet<TicketInteracao> TicketInteracao => Set<TicketInteracao>();
    public DbSet<TicketDocumento> TicketDocumento => Set<TicketDocumento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurarEmpresa(modelBuilder);
        ConfigurarUsuario(modelBuilder);
        ConfigurarCliente(modelBuilder);
        ConfigurarProduto(modelBuilder);
        ConfigurarProdutoXCliente(modelBuilder);
        ConfigurarTipoOcorrencia(modelBuilder);
        ConfigurarStatusTicket(modelBuilder);
        ConfigurarTecnico(modelBuilder);
        ConfigurarTicket(modelBuilder);
        ConfigurarTicketInteracao(modelBuilder);
        ConfigurarTicketDocumento(modelBuilder);
    }

    private static void ConfigurarEmpresa(ModelBuilder modelBuilder)
    {
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

            entity.Property(e => e.CodigoAcesso)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Ativo)
                .HasMaxLength(1)
                .HasDefaultValue("S");

            entity.HasIndex(e => e.CodigoAcesso)
                .IsUnique();
        });
    }

    private static void ConfigurarUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");

            entity.HasKey(e => new { e.IdEmpresa, e.IdUsuario });

            entity.Property(e => e.IdUsuario)
                .ValueGeneratedNever();

            entity.Property(e => e.NomeUsuario)
                .HasMaxLength(100)
                .IsRequired();

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

            entity.HasOne(e => e.Cliente)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdCliente })
                .IsRequired(false);

            entity.HasIndex(e => new { e.IdEmpresa, e.NomeUsuario })
                .IsUnique();
        });
    }

    private static void ConfigurarCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");

            entity.HasKey(e => new { e.IdEmpresa, e.IdCliente });

            entity.Property(e => e.IdCliente)
                .ValueGeneratedNever();

            entity.Property(e => e.DscCliente)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasMaxLength(200);

            entity.Property(e => e.Ativo)
                .HasMaxLength(1)
                .HasDefaultValue("S");

            entity.Property(e => e.CodClienteERP)
                .HasMaxLength(100);

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);
        });
    }

    private static void ConfigurarProduto(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("Produto");

            entity.HasKey(e => new { e.IdEmpresa, e.IdProduto });

            entity.Property(e => e.IdProduto)
                .ValueGeneratedNever();

            entity.Property(e => e.DscProduto)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Ativo)
                .HasMaxLength(1)
                .HasDefaultValue("S");

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);
        });
    }

    private static void ConfigurarProdutoXCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProdutoXCliente>(entity =>
        {
            entity.ToTable("ProdutoXCliente");

            entity.HasKey(e => new { e.IdEmpresa, e.IdProduto, e.IdCliente });

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);

            entity.HasOne(e => e.Produto)
                .WithMany(e => e.ClientesXProduto)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdProduto });

            entity.HasOne(e => e.Cliente)
                .WithMany(e => e.ProdutosXCliente)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdCliente });
        });
    }

    private static void ConfigurarTipoOcorrencia(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoOcorrencia>(entity =>
        {
            entity.ToTable("TipoOcorrencia");

            entity.HasKey(e => new { e.IdEmpresa, e.IdTipoOcorrencia });

            entity.Property(e => e.IdTipoOcorrencia)
                .ValueGeneratedNever();

            entity.Property(e => e.DscTipoOcorrencia)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.FiltroChamado)
                .HasMaxLength(200);

            entity.Property(e => e.EmailCopiaChamado)
                .HasMaxLength(300);

            entity.Property(e => e.Ativo)
                .HasMaxLength(1)
                .HasDefaultValue("S");

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);
        });
    }

    private static void ConfigurarStatusTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatusTicket>(entity =>
        {
            entity.ToTable("StatusTicket");

            entity.HasKey(e => new { e.IdEmpresa, e.IdStatusTicket });

            entity.Property(e => e.IdStatusTicket)
                .ValueGeneratedNever();

            entity.Property(e => e.DscStatusTicket)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.TipoTicket)
                .HasMaxLength(50);

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);
        });
    }

    private static void ConfigurarTecnico(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tecnico>(entity =>
        {
            entity.ToTable("Tecnico");

            entity.HasKey(e => new { e.IdEmpresa, e.IdTecnico });

            entity.Property(e => e.IdTecnico)
                .ValueGeneratedNever();

            entity.Property(e => e.Nome)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.NotificarNovosChamados)
                .HasMaxLength(1)
                .HasDefaultValue("N");

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);

            entity.HasOne(e => e.Usuario)
                .WithMany(e => e.Tecnicos)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdUsuario })
                .IsRequired(false);
        });
    }

    private static void ConfigurarTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.HasKey(e => new { e.IdEmpresa, e.IdTicket });

            entity.Property(e => e.IdTicket)
                .ValueGeneratedNever();

            entity.Property(e => e.DtHrAbertura)
                .IsRequired();

            entity.Property(e => e.Assunto)
                .HasMaxLength(300)
                .IsRequired();

            entity.Property(e => e.DscTicket);

            entity.Property(e => e.Prioridade)
                .HasMaxLength(50)
                .HasDefaultValue("Normal");

            entity.Property(e => e.TicketCancelado)
                .HasMaxLength(1)
                .HasDefaultValue("N");

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);

            entity.HasOne(e => e.Cliente)
                .WithMany(e => e.Tickets)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdCliente });

            entity.HasOne(e => e.Usuario)
                .WithMany(e => e.TicketsAbertos)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdUsuario });

            entity.HasOne(e => e.TipoOcorrencia)
                .WithMany(e => e.Tickets)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdTipoOcorrencia });

            entity.HasOne(e => e.StatusTicket)
                .WithMany(e => e.Tickets)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdStatusTicket });

            entity.HasOne(e => e.TecnicoFinalizacao)
                .WithMany()
                .HasForeignKey(e => new { e.IdEmpresa, e.IdTecnicoFinalizacao })
                .IsRequired(false);

            entity.HasIndex(e => new { e.IdEmpresa, e.IdCliente });
            entity.HasIndex(e => new { e.IdEmpresa, e.IdStatusTicket });
            entity.HasIndex(e => new { e.IdEmpresa, e.DtHrAbertura });
        });
    }

    private static void ConfigurarTicketInteracao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketInteracao>(entity =>
        {
            entity.ToTable("TicketInteracao");

            entity.HasKey(e => new { e.IdEmpresa, e.IdTicketInteracao });

            entity.Property(e => e.IdTicketInteracao)
                .ValueGeneratedNever();

            entity.Property(e => e.DtHrInteracao)
                .IsRequired();

            entity.Property(e => e.DscInteracao)
                .IsRequired();

            entity.Property(e => e.Privativo)
                .HasMaxLength(1)
                .HasDefaultValue("N");

            entity.Property(e => e.AguardarInteracaoUsuario)
                .HasMaxLength(1)
                .HasDefaultValue("N");

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);

            entity.HasOne(e => e.Ticket)
                .WithMany(e => e.Interacoes)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdTicket });

            entity.HasOne(e => e.Tecnico)
                .WithMany()
                .HasForeignKey(e => new { e.IdEmpresa, e.IdTecnico })
                .IsRequired(false);

            entity.HasOne(e => e.Usuario)
                .WithMany(e => e.Interacoes)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdUsuario })
                .IsRequired(false);
        });
    }

    private static void ConfigurarTicketDocumento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketDocumento>(entity =>
        {
            entity.ToTable("TicketDocumento");

            entity.HasKey(e => new { e.IdEmpresa, e.IdTicketDocumento });

            entity.Property(e => e.IdTicketDocumento)
                .ValueGeneratedNever();

            entity.Property(e => e.NomeArquivo)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.CaminhoArquivo)
                .HasMaxLength(500);

            entity.Property(e => e.ContentType)
                .HasMaxLength(100);

            entity.Property(e => e.DtHrUpload)
                .IsRequired();

            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.IdEmpresa);

            entity.HasOne(e => e.Ticket)
                .WithMany(e => e.Documentos)
                .HasForeignKey(e => new { e.IdEmpresa, e.IdTicket });
        });
    }
}