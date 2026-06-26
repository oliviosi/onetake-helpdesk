using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("StatusTicket")]
public class StatusTicket
{
    public int IdEmpresa { get; set; }

    public int IdStatusTicket { get; set; }

    public string DscStatusTicket { get; set; } = string.Empty;

    public string? TipoTicket { get; set; }

    public Empresa? Empresa { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}