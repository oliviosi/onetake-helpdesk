using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("Empresa")]
public class Empresa
{
    [Key]
    public int IdEmpresa { get; set; }

    public string? DscEmpresa { get; set; }

    public string? Cnpj { get; set; }

    public string? Endereco { get; set; }

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string? MensagemAtendimento { get; set; }

    public string CodigoAcesso { get; set; } = string.Empty;

    public string Ativo { get; set; } = "S";

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}