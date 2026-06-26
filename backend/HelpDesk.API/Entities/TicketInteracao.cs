using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("TicketInteracao")]
public class TicketInteracao
{
    public int IdEmpresa { get; set; }

    public int IdTicketInteracao { get; set; }

    public int IdTicket { get; set; }

    public DateTime DtHrInteracao { get; set; }

    public string DscInteracao { get; set; } = string.Empty;

    public int? IdTecnico { get; set; }

    public int? IdUsuario { get; set; }

    public string Privativo { get; set; } = "N";

    public string AguardarInteracaoUsuario { get; set; } = "N";

    public Empresa? Empresa { get; set; }

    public Ticket? Ticket { get; set; }

    public Tecnico? Tecnico { get; set; }

    public Usuario? Usuario { get; set; }
}