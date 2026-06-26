namespace HelpDesk.API.DTOs.Clientes;

public class ClienteCreateDto
{
    public string DscCliente { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? CodClienteERP { get; set; }
}