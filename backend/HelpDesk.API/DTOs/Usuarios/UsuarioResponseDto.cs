namespace HelpDesk.API.DTOs.Usuarios;

public class UsuarioResponseDto
{
    public int IdEmpresa { get; set; }
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string? NomeCompleto { get; set; }
    public string? Email { get; set; }
    public string EhAdm { get; set; } = "N";
    public string Ativo { get; set; } = "S";
}