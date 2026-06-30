using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs.Usuarios;

public class UsuarioCreateDto
{
    [Required(ErrorMessage = "Informe o nome do usuário.")]
    public string NomeUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    public string Senha { get; set; } = string.Empty;

    public string? NomeCompleto { get; set; }

    public string? Email { get; set; }

    public string EhAdm { get; set; } = "N";
}