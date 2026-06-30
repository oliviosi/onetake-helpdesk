using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs.Usuarios;

public class UsuarioUpdateDto
{
    [Required(ErrorMessage = "Informe o nome do usuário.")]
    public string NomeUsuario { get; set; } = string.Empty;

    public string? NomeCompleto { get; set; }

    public string? Email { get; set; }

    public string EhAdm { get; set; } = "N";

    public string Ativo { get; set; } = "S";
}