namespace HelpDesk.API.DTOs.Tickets;

public class TicketDetalheDto : TicketResumoDto
{
    public string? DscTicket { get; set; }
    public int? IdTecnicoFinalizacao { get; set; }
    public string? NomeTecnicoFinalizacao { get; set; }
    public List<TicketInteracaoDto> Interacoes { get; set; } = new();
}