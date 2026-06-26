namespace HelpDesk.API.DTOs.Clientes;

public class ClienteResponseDto
{
    public int IdEmpresa { get; set; }
    public int IdCliente { get; set; }
    public string DscCliente { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Ativo { get; set; } = "S";
    public string? CodClienteERP { get; set; }
}