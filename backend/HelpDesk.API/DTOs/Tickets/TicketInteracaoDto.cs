namespace HelpDesk.API.DTOs.Tickets;

public class TicketInteracaoDto
{
    public int IdEmpresa { get; set; }
    public int IdTicketInteracao { get; set; }
    public int IdTicket { get; set; }
    public DateTime DtHrInteracao { get; set; }
    public string DscInteracao { get; set; } = string.Empty;
    public int? IdTecnico { get; set; }
    public string? NomeTecnico { get; set; }
    public int? IdUsuario { get; set; }
    public string? NomeUsuario { get; set; }
    public string Privativo { get; set; } = "N";
    public string AguardarInteracaoUsuario { get; set; } = "N";
}