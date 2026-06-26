namespace HelpDesk.API.DTOs.Produtos;

public class ProdutoResponseDto
{
    public int IdEmpresa { get; set; }
    public int IdProduto { get; set; }
    public string DscProduto { get; set; } = string.Empty;
    public string Ativo { get; set; } = "S";
}