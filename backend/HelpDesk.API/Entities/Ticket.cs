using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("Ticket")]
public class Ticket
{
    public int IdEmpresa { get; set; }

    public int IdTicket { get; set; }

    public DateTime DtHrAbertura { get; set; }

    public int IdCliente { get; set; }

    public int IdUsuario { get; set; }

    public string Assunto { get; set; } = string.Empty;

    public int IdTipoOcorrencia { get; set; }

    public string? DscTicket { get; set; }

    public string Prioridade { get; set; } = "Normal";

    public int IdStatusTicket { get; set; }

    public DateTime? DtHrFinalizacao { get; set; }

    public int? IdTecnicoFinalizacao { get; set; }

    public string TicketCancelado { get; set; } = "N";

    public Empresa? Empresa { get; set; }

    public Cliente? Cliente { get; set; }

    public Usuario? Usuario { get; set; }

    public TipoOcorrencia? TipoOcorrencia { get; set; }

    public StatusTicket? StatusTicket { get; set; }

    public Tecnico? TecnicoFinalizacao { get; set; }

    public ICollection<TicketInteracao> Interacoes { get; set; } = new List<TicketInteracao>();

    public ICollection<TicketDocumento> Documentos { get; set; } = new List<TicketDocumento>();
}