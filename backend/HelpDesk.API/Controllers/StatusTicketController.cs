using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.StatusTicket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatusTicketController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public StatusTicketController(HelpDeskDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var idEmpresa = ObterIdEmpresa();

        var status = await _context.StatusTicket
            .Where(s => s.IdEmpresa == idEmpresa)
            .OrderBy(s => s.IdStatusTicket)
            .Select(s => new StatusTicketResponseDto
            {
                IdEmpresa = s.IdEmpresa,
                IdStatusTicket = s.IdStatusTicket,
                DscStatusTicket = s.DscStatusTicket,
                TipoTicket = s.TipoTicket
            })
            .ToListAsync();

        return Ok(ResponseDto<List<StatusTicketResponseDto>>.Ok(status));
    }

    [HttpGet("{idStatusTicket:int}")]
    public async Task<IActionResult> BuscarPorId(int idStatusTicket)
    {
        var idEmpresa = ObterIdEmpresa();

        var status = await _context.StatusTicket
            .Where(s =>
                s.IdEmpresa == idEmpresa &&
                s.IdStatusTicket == idStatusTicket)
            .Select(s => new StatusTicketResponseDto
            {
                IdEmpresa = s.IdEmpresa,
                IdStatusTicket = s.IdStatusTicket,
                DscStatusTicket = s.DscStatusTicket,
                TipoTicket = s.TipoTicket
            })
            .FirstOrDefaultAsync();

        if (status == null)
            return NotFound(ResponseDto<object>.Error(404, "Status do ticket não encontrado."));

        return Ok(ResponseDto<StatusTicketResponseDto>.Ok(status));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] StatusTicketCreateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.DscStatusTicket))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a descrição do status."));

        var descricao = request.DscStatusTicket.Trim();

        var jaExiste = await _context.StatusTicket.AnyAsync(s =>
            s.IdEmpresa == idEmpresa &&
            s.DscStatusTicket == descricao);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe um status com essa descrição."));

        var proximoId = await ObterProximoIdStatusTicket(idEmpresa);

        var status = new Entities.StatusTicket
        {
            IdEmpresa = idEmpresa,
            IdStatusTicket = proximoId,
            DscStatusTicket = descricao,
            TipoTicket = request.TipoTicket?.Trim()
        };

        _context.StatusTicket.Add(status);
        await _context.SaveChangesAsync();

        var response = new StatusTicketResponseDto
        {
            IdEmpresa = status.IdEmpresa,
            IdStatusTicket = status.IdStatusTicket,
            DscStatusTicket = status.DscStatusTicket,
            TipoTicket = status.TipoTicket
        };

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { idStatusTicket = status.IdStatusTicket },
            ResponseDto<StatusTicketResponseDto>.Ok(response, "Status criado com sucesso.")
        );
    }

    [HttpPut("{idStatusTicket:int}")]
    public async Task<IActionResult> Atualizar(
        int idStatusTicket,
        [FromBody] StatusTicketUpdateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.DscStatusTicket))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a descrição do status."));

        var status = await _context.StatusTicket
            .FirstOrDefaultAsync(s =>
                s.IdEmpresa == idEmpresa &&
                s.IdStatusTicket == idStatusTicket);

        if (status == null)
            return NotFound(ResponseDto<object>.Error(404, "Status do ticket não encontrado."));

        var descricao = request.DscStatusTicket.Trim();

        var jaExiste = await _context.StatusTicket.AnyAsync(s =>
            s.IdEmpresa == idEmpresa &&
            s.IdStatusTicket != idStatusTicket &&
            s.DscStatusTicket == descricao);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe outro status com essa descrição."));

        status.DscStatusTicket = descricao;
        status.TipoTicket = request.TipoTicket?.Trim();

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            status.IdEmpresa,
            status.IdStatusTicket
        }, "Status atualizado com sucesso."));
    }

    [HttpDelete("{idStatusTicket:int}")]
    public async Task<IActionResult> Excluir(int idStatusTicket)
    {
        var idEmpresa = ObterIdEmpresa();

        var status = await _context.StatusTicket
            .FirstOrDefaultAsync(s =>
                s.IdEmpresa == idEmpresa &&
                s.IdStatusTicket == idStatusTicket);

        if (status == null)
            return NotFound(ResponseDto<object>.Error(404, "Status do ticket não encontrado."));

        var estaEmUso = await _context.Ticket.AnyAsync(t =>
            t.IdEmpresa == idEmpresa &&
            t.IdStatusTicket == idStatusTicket);

        if (estaEmUso)
            return BadRequest(ResponseDto<object>.Error(400, "Este status está em uso e não pode ser excluído."));

        _context.StatusTicket.Remove(status);
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            IdEmpresa = idEmpresa,
            IdStatusTicket = idStatusTicket
        }, "Status excluído com sucesso."));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }

    private async Task<int> ObterProximoIdStatusTicket(int idEmpresa)
    {
        var ultimoId = await _context.StatusTicket
            .Where(s => s.IdEmpresa == idEmpresa)
            .MaxAsync(s => (int?)s.IdStatusTicket);

        return (ultimoId ?? 0) + 1;
    }
}