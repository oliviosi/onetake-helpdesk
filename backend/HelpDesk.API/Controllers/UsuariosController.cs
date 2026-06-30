using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.Usuarios;
using HelpDesk.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly HelpDeskDbContext _context;
    private readonly PasswordHasherService _passwordHasherService;

    public UsuariosController(
        HelpDeskDbContext context,
        PasswordHasherService passwordHasherService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var idEmpresa = ObterIdEmpresa();

        var usuarios = await _context.Usuario
            .Where(u => u.IdEmpresa == idEmpresa)
            .OrderBy(u => u.NomeUsuario)
            .Select(u => new UsuarioResponseDto
            {
                IdEmpresa = u.IdEmpresa,
                IdUsuario = u.IdUsuario,
                NomeUsuario = u.NomeUsuario,
                NomeCompleto = u.NomeCompleto,
                Email = u.Email,
                EhAdm = u.EhAdm,
                Ativo = u.Ativo
            })
            .ToListAsync();

        return Ok(ResponseDto<List<UsuarioResponseDto>>.Ok(usuarios));
    }

    [HttpGet("{idUsuario:int}")]
    public async Task<IActionResult> BuscarPorId(int idUsuario)
    {
        var idEmpresa = ObterIdEmpresa();

        var usuario = await _context.Usuario
            .Where(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == idUsuario)
            .Select(u => new UsuarioResponseDto
            {
                IdEmpresa = u.IdEmpresa,
                IdUsuario = u.IdUsuario,
                NomeUsuario = u.NomeUsuario,
                NomeCompleto = u.NomeCompleto,
                Email = u.Email,
                EhAdm = u.EhAdm,
                Ativo = u.Ativo
            })
            .FirstOrDefaultAsync();

        if (usuario == null)
            return NotFound(ResponseDto<object>.Error(404, "Usuário não encontrado."));

        return Ok(ResponseDto<UsuarioResponseDto>.Ok(usuario));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] UsuarioCreateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.NomeUsuario))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o nome do usuário."));

        if (string.IsNullOrWhiteSpace(request.Senha))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a senha."));

        var nomeUsuario = request.NomeUsuario.Trim();

        var jaExiste = await _context.Usuario.AnyAsync(u =>
            u.IdEmpresa == idEmpresa &&
            u.NomeUsuario == nomeUsuario);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe um usuário com esse nome."));

        var proximoId = await ObterProximoIdUsuario(idEmpresa);

        var usuario = new Entities.Usuario
        {
            IdEmpresa = idEmpresa,
            IdUsuario = proximoId,
            NomeUsuario = nomeUsuario,
            Senha = _passwordHasherService.HashPassword(request.Senha),
            NomeCompleto = string.IsNullOrWhiteSpace(request.NomeCompleto) ? null : request.NomeCompleto.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            EhAdm = request.EhAdm == "S" ? "S" : "N",
            Ativo = "S"
        };

        _context.Usuario.Add(usuario);
        await _context.SaveChangesAsync();

        var response = new UsuarioResponseDto
        {
            IdEmpresa = usuario.IdEmpresa,
            IdUsuario = usuario.IdUsuario,
            NomeUsuario = usuario.NomeUsuario,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email,
            EhAdm = usuario.EhAdm,
            Ativo = usuario.Ativo
        };

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { idUsuario = usuario.IdUsuario },
            ResponseDto<UsuarioResponseDto>.Ok(response, "Usuário criado com sucesso.")
        );
    }

    [HttpPut("{idUsuario:int}")]
    public async Task<IActionResult> Atualizar(
        int idUsuario,
        [FromBody] UsuarioUpdateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.NomeUsuario))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o nome do usuário."));

        var usuario = await _context.Usuario
            .FirstOrDefaultAsync(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == idUsuario);

        if (usuario == null)
            return NotFound(ResponseDto<object>.Error(404, "Usuário não encontrado."));

        var nomeUsuario = request.NomeUsuario.Trim();

        var jaExiste = await _context.Usuario.AnyAsync(u =>
            u.IdEmpresa == idEmpresa &&
            u.IdUsuario != idUsuario &&
            u.NomeUsuario == nomeUsuario);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe outro usuário com esse nome."));

        usuario.NomeUsuario = nomeUsuario;
        usuario.NomeCompleto = string.IsNullOrWhiteSpace(request.NomeCompleto) ? null : request.NomeCompleto.Trim();
        usuario.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        usuario.EhAdm = request.EhAdm == "S" ? "S" : "N";
        usuario.Ativo = request.Ativo == "N" ? "N" : "S";

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            usuario.IdEmpresa,
            usuario.IdUsuario
        }, "Usuário atualizado com sucesso."));
    }

    [HttpPatch("{idUsuario:int}/alterar-senha")]
    public async Task<IActionResult> AlterarSenha(
        int idUsuario,
        [FromBody] UsuarioAlterarSenhaDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.NovaSenha))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a nova senha."));

        var usuario = await _context.Usuario
            .FirstOrDefaultAsync(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == idUsuario);

        if (usuario == null)
            return NotFound(ResponseDto<object>.Error(404, "Usuário não encontrado."));

        usuario.Senha = _passwordHasherService.HashPassword(request.NovaSenha);

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            usuario.IdEmpresa,
            usuario.IdUsuario
        }, "Senha alterada com sucesso."));
    }

    [HttpPatch("{idUsuario:int}/ativar")]
    public async Task<IActionResult> Ativar(int idUsuario)
    {
        var idEmpresa = ObterIdEmpresa();

        var usuario = await _context.Usuario
            .FirstOrDefaultAsync(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == idUsuario);

        if (usuario == null)
            return NotFound(ResponseDto<object>.Error(404, "Usuário não encontrado."));

        usuario.Ativo = "S";

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            usuario.IdEmpresa,
            usuario.IdUsuario
        }, "Usuário ativado com sucesso."));
    }

    [HttpPatch("{idUsuario:int}/desativar")]
    public async Task<IActionResult> Desativar(int idUsuario)
    {
        var idEmpresa = ObterIdEmpresa();
        var idUsuarioLogado = ObterIdUsuario();

        if (idUsuario == idUsuarioLogado)
            return BadRequest(ResponseDto<object>.Error(400, "Você não pode desativar o próprio usuário logado."));

        var usuario = await _context.Usuario
            .FirstOrDefaultAsync(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == idUsuario);

        if (usuario == null)
            return NotFound(ResponseDto<object>.Error(404, "Usuário não encontrado."));

        usuario.Ativo = "N";

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            usuario.IdEmpresa,
            usuario.IdUsuario
        }, "Usuário desativado com sucesso."));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }

    private int ObterIdUsuario()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdUsuario")?.Value;

        if (!int.TryParse(claim, out var idUsuario))
            throw new UnauthorizedAccessException("Usuário não identificado no token.");

        return idUsuario;
    }

    private async Task<int> ObterProximoIdUsuario(int idEmpresa)
    {
        var ultimoId = await _context.Usuario
            .Where(u => u.IdEmpresa == idEmpresa)
            .MaxAsync(u => (int?)u.IdUsuario);

        return (ultimoId ?? 0) + 1;
    }
}