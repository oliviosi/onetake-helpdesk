using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.TipoOcorrencia;
using HelpDesk.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TipoOcorrenciaController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public TipoOcorrenciaController(HelpDeskDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var idEmpresa = ObterIdEmpresa();

        var tipos = await _context.TipoOcorrencia
            .Where(t => t.IdEmpresa == idEmpresa)
            .OrderBy(t => t.DscTipoOcorrencia)
            .Select(t => new TipoOcorrenciaResponseDto
            {
                IdEmpresa = t.IdEmpresa,
                IdTipoOcorrencia = t.IdTipoOcorrencia,
                DscTipoOcorrencia = t.DscTipoOcorrencia,
                FiltroChamado = t.FiltroChamado,
                EmailCopiaChamado = t.EmailCopiaChamado,
                Ativo = t.Ativo
            })
            .ToListAsync();

        return Ok(ResponseDto<List<TipoOcorrenciaResponseDto>>.Ok(tipos));
    }

    [HttpGet("{idTipoOcorrencia:int}")]
    public async Task<IActionResult> BuscarPorId(int idTipoOcorrencia)
    {
        var idEmpresa = ObterIdEmpresa();

        var tipo = await _context.TipoOcorrencia
            .Where(t => t.IdEmpresa == idEmpresa && t.IdTipoOcorrencia == idTipoOcorrencia)
            .Select(t => new TipoOcorrenciaResponseDto
            {
                IdEmpresa = t.IdEmpresa,
                IdTipoOcorrencia = t.IdTipoOcorrencia,
                DscTipoOcorrencia = t.DscTipoOcorrencia,
                FiltroChamado = t.FiltroChamado,
                EmailCopiaChamado = t.EmailCopiaChamado,
                Ativo = t.Ativo
            })
            .FirstOrDefaultAsync();

        if (tipo == null)
            return NotFound(ResponseDto<object>.Error(404, "Tipo de ocorrência não encontrado."));

        return Ok(ResponseDto<TipoOcorrenciaResponseDto>.Ok(tipo));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TipoOcorrenciaCreateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.DscTipoOcorrencia))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a descrição do tipo de ocorrência."));

        var descricao = request.DscTipoOcorrencia.Trim();

        var jaExiste = await _context.TipoOcorrencia.AnyAsync(t =>
            t.IdEmpresa == idEmpresa &&
            t.DscTipoOcorrencia == descricao);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe um tipo de ocorrência com essa descrição."));

        var proximoId = await ObterProximoIdTipoOcorrencia(idEmpresa);

        var tipo = new Entities.TipoOcorrencia
        {
            IdEmpresa = idEmpresa,
            IdTipoOcorrencia = proximoId,
            DscTipoOcorrencia = descricao,
            FiltroChamado = request.FiltroChamado?.Trim(),
            EmailCopiaChamado = request.EmailCopiaChamado?.Trim(),
            Ativo = "S"
        };

        _context.TipoOcorrencia.Add(tipo);
        await _context.SaveChangesAsync();

        var response = new TipoOcorrenciaResponseDto
        {
            IdEmpresa = tipo.IdEmpresa,
            IdTipoOcorrencia = tipo.IdTipoOcorrencia,
            DscTipoOcorrencia = tipo.DscTipoOcorrencia,
            FiltroChamado = tipo.FiltroChamado,
            EmailCopiaChamado = tipo.EmailCopiaChamado,
            Ativo = tipo.Ativo
        };

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { idTipoOcorrencia = tipo.IdTipoOcorrencia },
            ResponseDto<TipoOcorrenciaResponseDto>.Ok(response, "Tipo de ocorrência criado com sucesso.")
        );
    }

    [HttpPut("{idTipoOcorrencia:int}")]
    public async Task<IActionResult> Atualizar(
        int idTipoOcorrencia,
        [FromBody] TipoOcorrenciaUpdateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.DscTipoOcorrencia))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a descrição do tipo de ocorrência."));

        var tipo = await _context.TipoOcorrencia
            .FirstOrDefaultAsync(t =>
                t.IdEmpresa == idEmpresa &&
                t.IdTipoOcorrencia == idTipoOcorrencia);

        if (tipo == null)
            return NotFound(ResponseDto<object>.Error(404, "Tipo de ocorrência não encontrado."));

        var descricao = request.DscTipoOcorrencia.Trim();

        var jaExiste = await _context.TipoOcorrencia.AnyAsync(t =>
            t.IdEmpresa == idEmpresa &&
            t.IdTipoOcorrencia != idTipoOcorrencia &&
            t.DscTipoOcorrencia == descricao);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe outro tipo de ocorrência com essa descrição."));

        tipo.DscTipoOcorrencia = descricao;
        tipo.FiltroChamado = request.FiltroChamado?.Trim();
        tipo.EmailCopiaChamado = request.EmailCopiaChamado?.Trim();
        tipo.Ativo = request.Ativo == "N" ? "N" : "S";

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            tipo.IdEmpresa,
            tipo.IdTipoOcorrencia
        }, "Tipo de ocorrência atualizado com sucesso."));
    }

    [HttpPatch("{idTipoOcorrencia:int}/ativar")]
    public async Task<IActionResult> Ativar(int idTipoOcorrencia)
    {
        var idEmpresa = ObterIdEmpresa();

        var tipo = await _context.TipoOcorrencia
            .FirstOrDefaultAsync(t =>
                t.IdEmpresa == idEmpresa &&
                t.IdTipoOcorrencia == idTipoOcorrencia);

        if (tipo == null)
            return NotFound(ResponseDto<object>.Error(404, "Tipo de ocorrência não encontrado."));

        tipo.Ativo = "S";
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            tipo.IdEmpresa,
            tipo.IdTipoOcorrencia
        }, "Tipo de ocorrência ativado com sucesso."));
    }

    [HttpPatch("{idTipoOcorrencia:int}/desativar")]
    public async Task<IActionResult> Desativar(int idTipoOcorrencia)
    {
        var idEmpresa = ObterIdEmpresa();

        var tipo = await _context.TipoOcorrencia
            .FirstOrDefaultAsync(t =>
                t.IdEmpresa == idEmpresa &&
                t.IdTipoOcorrencia == idTipoOcorrencia);

        if (tipo == null)
            return NotFound(ResponseDto<object>.Error(404, "Tipo de ocorrência não encontrado."));

        tipo.Ativo = "N";
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            tipo.IdEmpresa,
            tipo.IdTipoOcorrencia
        }, "Tipo de ocorrência desativado com sucesso."));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }

    private async Task<int> ObterProximoIdTipoOcorrencia(int idEmpresa)
    {
        var ultimoId = await _context.TipoOcorrencia
            .Where(t => t.IdEmpresa == idEmpresa)
            .MaxAsync(t => (int?)t.IdTipoOcorrencia);

        return (ultimoId ?? 0) + 1;
    }
}