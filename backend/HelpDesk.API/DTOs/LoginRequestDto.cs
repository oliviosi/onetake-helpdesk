namespace HelpDesk.API.DTOs;

public class LoginRequestDto
{
    public string CodigoAcesso { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}