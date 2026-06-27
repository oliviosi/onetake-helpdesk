namespace HelpDesk.API.DTOs.Tickets;

public class TicketCreateDto
{
    public int IdCliente { get; set; }
    public int IdUsuario { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public int IdTipoOcorrencia { get; set; }
    public string DscTicket { get; set; } = string.Empty;
    public string Prioridade { get; set; } = "Normal";
}