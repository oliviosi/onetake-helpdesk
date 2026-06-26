using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("TipoOcorrencia")]
public class TipoOcorrencia
{
    public int IdEmpresa { get; set; }

    public int IdTipoOcorrencia { get; set; }

    public string DscTipoOcorrencia { get; set; } = string.Empty;

    public string? FiltroChamado { get; set; }

    public string? EmailCopiaChamado { get; set; }

    public string Ativo { get; set; } = "S";

    public Empresa? Empresa { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}