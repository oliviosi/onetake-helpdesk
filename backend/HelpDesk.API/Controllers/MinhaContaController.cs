using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.MinhaConta;
using HelpDesk.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MinhaContaController : ControllerBase
{
    private readonly HelpDeskDbContext _context;
    private readonly PasswordHasherService _passwordHasherService;

    public MinhaContaController(
        HelpDeskDbContext context,
        PasswordHasherService passwordHasherService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
    }

    [HttpPatch("alterar-senha")]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarMinhaSenhaDto request)
    {
        var idEmpresa = ObterIdEmpresa();
        var idUsuario = ObterIdUsuario();

        if (string.IsNullOrWhiteSpace(request.SenhaAtual))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a senha atual."));

        if (string.IsNullOrWhiteSpace(request.NovaSenha))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a nova senha."));

        if (request.NovaSenha.Trim().Length < 6)
            return BadRequest(ResponseDto<object>.Error(400, "A nova senha deve ter pelo menos 6 caracteres."));

        var usuario = await _context.Usuario
            .FirstOrDefaultAsync(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == idUsuario &&
                u.Ativo == "S");

        if (usuario == null)
            return NotFound(ResponseDto<object>.Error(404, "Usuário não encontrado ou inativo."));

        var senhaAtualValida = _passwordHasherService.VerifyPassword(
            request.SenhaAtual,
            usuario.Senha
        );

        if (!senhaAtualValida)
            return BadRequest(ResponseDto<object>.Error(400, "Senha atual inválida."));

        usuario.Senha = _passwordHasherService.HashPassword(request.NovaSenha.Trim());

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            usuario.IdEmpresa,
            usuario.IdUsuario
        }, "Senha alterada com sucesso."));
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
}