namespace HelpDesk.API.DTOs.Tecnicos;

public class TecnicoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string NotificarNovosChamados { get; set; } = "N";
    public int? IdUsuario { get; set; }
}