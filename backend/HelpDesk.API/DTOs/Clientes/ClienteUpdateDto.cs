namespace HelpDesk.API.DTOs.Clientes;

public class ClienteUpdateDto
{
    public string DscCliente { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? CodClienteERP { get; set; }
    public string Ativo { get; set; } = "S";
}