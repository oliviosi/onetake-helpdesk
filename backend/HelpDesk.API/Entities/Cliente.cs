using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("Cliente")]
public class Cliente
{
    public int IdEmpresa { get; set; }

    public int IdCliente { get; set; }

    public string DscCliente { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string Ativo { get; set; } = "S";

    public string? CodClienteERP { get; set; }

    public Empresa? Empresa { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public ICollection<ProdutoXCliente> ProdutosXCliente { get; set; } = new List<ProdutoXCliente>();

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}