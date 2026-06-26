namespace HelpDesk.API.DTOs.StatusTicket;

public class StatusTicketCreateDto
{
    public string DscStatusTicket { get; set; } = string.Empty;
    public string? TipoTicket { get; set; }
}