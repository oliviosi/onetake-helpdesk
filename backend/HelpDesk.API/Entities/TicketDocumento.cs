using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("TicketDocumento")]
public class TicketDocumento
{
    public int IdEmpresa { get; set; }

    public int IdTicketDocumento { get; set; }

    public int IdTicket { get; set; }

    public string NomeArquivo { get; set; } = string.Empty;

    public string? CaminhoArquivo { get; set; }

    public string? ContentType { get; set; }

    public long? TamanhoBytes { get; set; }

    public DateTime DtHrUpload { get; set; }

    public Empresa? Empresa { get; set; }

    public Ticket? Ticket { get; set; }
}