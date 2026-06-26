using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Entities;

[Table("Usuario")]
public class Usuario
{
    public int IdEmpresa { get; set; }

    public int IdUsuario { get; set; }

    public string NomeUsuario { get; set; } = string.Empty;

    public int? IdCliente { get; set; }

    public string Ativo { get; set; } = "S";

    public string Senha { get; set; } = string.Empty;

    public string? NomeCompleto { get; set; }

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string EhAdm { get; set; } = "N";

    public Empresa? Empresa { get; set; }
}