namespace HelpDesk.API.DTOs.TipoOcorrencia;

public class TipoOcorrenciaUpdateDto
{
    public string DscTipoOcorrencia { get; set; } = string.Empty;
    public string? FiltroChamado { get; set; }
    public string? EmailCopiaChamado { get; set; }
    public string Ativo { get; set; } = "S";
}