using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs.MinhaConta;

public class AlterarMinhaSenhaDto
{
    [Required(ErrorMessage = "Informe a senha atual.")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    public string NovaSenha { get; set; } = string.Empty;
}