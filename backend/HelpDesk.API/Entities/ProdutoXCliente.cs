using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("ProdutoXCliente")]
public class ProdutoXCliente
{
    public int IdEmpresa { get; set; }

    public int IdProduto { get; set; }

    public int IdCliente { get; set; }

    public Empresa? Empresa { get; set; }

    public Produto? Produto { get; set; }

    public Cliente? Cliente { get; set; }
}