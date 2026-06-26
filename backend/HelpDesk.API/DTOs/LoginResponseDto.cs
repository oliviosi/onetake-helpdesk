namespace HelpDesk.API.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UsuarioLogadoDto Usuario { get; set; } = new();
}

public class UsuarioLogadoDto
{
    public int IdEmpresa { get; set; }
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string? NomeCompleto { get; set; }
    public string? Email { get; set; }
    public string EhAdm { get; set; } = "N";
}