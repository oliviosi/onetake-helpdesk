using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("Tecnico")]
public class Tecnico
{
    public int IdEmpresa { get; set; }

    public int IdTecnico { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string NotificarNovosChamados { get; set; } = "N";

    public int? IdUsuario { get; set; }

    public Empresa? Empresa { get; set; }

    public Usuario? Usuario { get; set; }
}