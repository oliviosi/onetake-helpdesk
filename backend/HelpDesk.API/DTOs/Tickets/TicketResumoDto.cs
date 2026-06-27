namespace HelpDesk.API.DTOs.Tickets;

public class TicketResumoDto
{
    public int IdEmpresa { get; set; }
    public int IdTicket { get; set; }
    public DateTime DtHrAbertura { get; set; }
    public int IdCliente { get; set; }
    public string DscCliente { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public int IdTipoOcorrencia { get; set; }
    public string DscTipoOcorrencia { get; set; } = string.Empty;
    public string Prioridade { get; set; } = "Normal";
    public int IdStatusTicket { get; set; }
    public string DscStatusTicket { get; set; } = string.Empty;
    public DateTime? DtHrFinalizacao { get; set; }
    public string TicketCancelado { get; set; } = "N";
}