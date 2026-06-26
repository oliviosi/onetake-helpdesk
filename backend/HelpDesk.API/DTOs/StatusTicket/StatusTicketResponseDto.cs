namespace HelpDesk.API.DTOs.StatusTicket;

public class StatusTicketResponseDto
{
    public int IdEmpresa { get; set; }
    public int IdStatusTicket { get; set; }
    public string DscStatusTicket { get; set; } = string.Empty;
    public string? TipoTicket { get; set; }
}