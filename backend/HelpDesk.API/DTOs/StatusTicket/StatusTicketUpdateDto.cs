namespace HelpDesk.API.DTOs.StatusTicket;

public class StatusTicketUpdateDto
{
    public string DscStatusTicket { get; set; } = string.Empty;
    public string? TipoTicket { get; set; }
}