using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.Tecnicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TecnicosController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public TecnicosController(HelpDeskDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var idEmpresa = ObterIdEmpresa();

        var tecnicos = await _context.Tecnico
            .Where(t => t.IdEmpresa == idEmpresa)
            .Include(t => t.Usuario)
            .OrderBy(t => t.Nome)
            .Select(t => new TecnicoResponseDto
            {
                IdEmpresa = t.IdEmpresa,
                IdTecnico = t.IdTecnico,
                Nome = t.Nome,
                NotificarNovosChamados = t.NotificarNovosChamados,
                IdUsuario = t.IdUsuario,
                NomeUsuario = t.Usuario != null ? t.Usuario.NomeUsuario : null
            })
            .ToListAsync();

        return Ok(ResponseDto<List<TecnicoResponseDto>>.Ok(tecnicos));
    }

    [HttpGet("{idTecnico:int}")]
    public async Task<IActionResult> BuscarPorId(int idTecnico)
    {
        var idEmpresa = ObterIdEmpresa();

        var tecnico = await _context.Tecnico
            .Where(t => t.IdEmpresa == idEmpresa && t.IdTecnico == idTecnico)
            .Include(t => t.Usuario)
            .Select(t => new TecnicoResponseDto
            {
                IdEmpresa = t.IdEmpresa,
                IdTecnico = t.IdTecnico,
                Nome = t.Nome,
                NotificarNovosChamados = t.NotificarNovosChamados,
                IdUsuario = t.IdUsuario,
                NomeUsuario = t.Usuario != null ? t.Usuario.NomeUsuario : null
            })
            .FirstOrDefaultAsync();

        if (tecnico == null)
            return NotFound(ResponseDto<object>.Error(404, "Técnico não encontrado."));

        return Ok(ResponseDto<TecnicoResponseDto>.Ok(tecnico));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TecnicoCreateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o nome do técnico."));

        if (request.IdUsuario.HasValue)
        {
            var usuarioExiste = await _context.Usuario.AnyAsync(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == request.IdUsuario.Value &&
                u.Ativo == "S");

            if (!usuarioExiste)
                return BadRequest(ResponseDto<object>.Error(400, "Usuário vinculado não encontrado ou inativo."));
        }

        var jaExiste = await _context.Tecnico.AnyAsync(t =>
            t.IdEmpresa == idEmpresa &&
            t.Nome == request.Nome.Trim());

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe um técnico com esse nome."));

        var proximoId = await ObterProximoIdTecnico(idEmpresa);

        var tecnico = new Entities.Tecnico
        {
            IdEmpresa = idEmpresa,
            IdTecnico = proximoId,
            Nome = request.Nome.Trim(),
            NotificarNovosChamados = request.NotificarNovosChamados == "S" ? "S" : "N",
            IdUsuario = request.IdUsuario
        };

        _context.Tecnico.Add(tecnico);
        await _context.SaveChangesAsync();

        var response = new TecnicoResponseDto
        {
            IdEmpresa = tecnico.IdEmpresa,
            IdTecnico = tecnico.IdTecnico,
            Nome = tecnico.Nome,
            NotificarNovosChamados = tecnico.NotificarNovosChamados,
            IdUsuario = tecnico.IdUsuario
        };

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { idTecnico = tecnico.IdTecnico },
            ResponseDto<TecnicoResponseDto>.Ok(response, "Técnico criado com sucesso.")
        );
    }

    [HttpPut("{idTecnico:int}")]
    public async Task<IActionResult> Atualizar(int idTecnico, [FromBody] TecnicoUpdateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o nome do técnico."));

        var tecnico = await _context.Tecnico
            .FirstOrDefaultAsync(t => t.IdEmpresa == idEmpresa && t.IdTecnico == idTecnico);

        if (tecnico == null)
            return NotFound(ResponseDto<object>.Error(404, "Técnico não encontrado."));

        if (request.IdUsuario.HasValue)
        {
            var usuarioExiste = await _context.Usuario.AnyAsync(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == request.IdUsuario.Value &&
                u.Ativo == "S");

            if (!usuarioExiste)
                return BadRequest(ResponseDto<object>.Error(400, "Usuário vinculado não encontrado ou inativo."));
        }

        var nome = request.Nome.Trim();

        var jaExiste = await _context.Tecnico.AnyAsync(t =>
            t.IdEmpresa == idEmpresa &&
            t.IdTecnico != idTecnico &&
            t.Nome == nome);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe outro técnico com esse nome."));

        tecnico.Nome = nome;
        tecnico.NotificarNovosChamados = request.NotificarNovosChamados == "S" ? "S" : "N";
        tecnico.IdUsuario = request.IdUsuario;

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            tecnico.IdEmpresa,
            tecnico.IdTecnico
        }, "Técnico atualizado com sucesso."));
    }

    [HttpDelete("{idTecnico:int}")]
    public async Task<IActionResult> Excluir(int idTecnico)
    {
        var idEmpresa = ObterIdEmpresa();

        var tecnico = await _context.Tecnico
            .FirstOrDefaultAsync(t => t.IdEmpresa == idEmpresa && t.IdTecnico == idTecnico);

        if (tecnico == null)
            return NotFound(ResponseDto<object>.Error(404, "Técnico não encontrado."));

        var estaEmUso = await _context.Ticket.AnyAsync(t =>
            t.IdEmpresa == idEmpresa &&
            t.IdTecnicoFinalizacao == idTecnico);

        if (estaEmUso)
            return BadRequest(ResponseDto<object>.Error(400, "Este técnico está em uso em tickets e não pode ser excluído."));

        _context.Tecnico.Remove(tecnico);
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            IdEmpresa = idEmpresa,
            IdTecnico = idTecnico
        }, "Técnico excluído com sucesso."));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }

    private async Task<int> ObterProximoIdTecnico(int idEmpresa)
    {
        var ultimoId = await _context.Tecnico
            .Where(t => t.IdEmpresa == idEmpresa)
            .MaxAsync(t => (int?)t.IdTecnico);

        return (ultimoId ?? 0) + 1;
    }
}