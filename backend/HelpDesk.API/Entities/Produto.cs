using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("Produto")]
public class Produto
{
    public int IdEmpresa { get; set; }

    public int IdProduto { get; set; }

    public string DscProduto { get; set; } = string.Empty;

    public string Ativo { get; set; } = "S";

    public Empresa? Empresa { get; set; }

    public ICollection<ProdutoXCliente> ClientesXProduto { get; set; } = new List<ProdutoXCliente>();
}