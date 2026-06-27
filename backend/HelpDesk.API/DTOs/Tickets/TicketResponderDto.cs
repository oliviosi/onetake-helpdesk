namespace HelpDesk.API.DTOs.Tickets;

public class TicketResponderDto
{
    public string Mensagem { get; set; } = string.Empty;
    public int? IdTecnico { get; set; }
    public int? IdUsuario { get; set; }
    public string Privativo { get; set; } = "N";
    public string AguardarInteracaoUsuario { get; set; } = "N";
}