using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs.Usuarios;

public class UsuarioAlterarSenhaDto
{
    [Required(ErrorMessage = "Informe a nova senha.")]
    public string NovaSenha { get; set; } = string.Empty;
}