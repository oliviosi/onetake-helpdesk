namespace HelpDesk.API.DTOs.Tecnicos;

public class TecnicoResponseDto
{
    public int IdEmpresa { get; set; }
    public int IdTecnico { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NotificarNovosChamados { get; set; } = "N";
    public int? IdUsuario { get; set; }
    public string? NomeUsuario { get; set; }
}