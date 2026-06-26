namespace HelpDesk.API.DTOs.ProdutoXCliente;

public class ProdutoXClienteResponseDto
{
    public int IdEmpresa { get; set; }
    public int IdCliente { get; set; }
    public string DscCliente { get; set; } = string.Empty;
    public int IdProduto { get; set; }
    public string DscProduto { get; set; } = string.Empty;
}