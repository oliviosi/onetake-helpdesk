namespace HelpDesk.API.DTOs.TipoOcorrencia;

public class TipoOcorrenciaCreateDto
{
    public string DscTipoOcorrencia { get; set; } = string.Empty;
    public string? FiltroChamado { get; set; }
    public string? EmailCopiaChamado { get; set; }
}