using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HelpDeskDbContext _context;
    private readonly PasswordHasherService _passwordHasherService;
    private readonly TokenService _tokenService;

    public AuthController(
        HelpDeskDbContext context,
        PasswordHasherService passwordHasherService,
        TokenService tokenService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CodigoAcesso))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o código de acesso."));

        if (string.IsNullOrWhiteSpace(request.Usuario))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o usuário."));

        if (string.IsNullOrWhiteSpace(request.Senha))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a senha."));

        var codigoAcesso = request.CodigoAcesso.Trim();
        var nomeUsuario = request.Usuario.Trim();

        var empresa = await _context.Empresa
            .FirstOrDefaultAsync(e =>
                e.CodigoAcesso == codigoAcesso &&
                e.Ativo == "S");

        if (empresa == null)
            return Unauthorized(ResponseDto<object>.Error(401, "Código de acesso inválido ou empresa inativa."));

        var usuario = await _context.Usuario
            .FirstOrDefaultAsync(u =>
                u.IdEmpresa == empresa.IdEmpresa &&
                u.NomeUsuario == nomeUsuario &&
                u.Ativo == "S");

        if (usuario == null)
            return Unauthorized(ResponseDto<object>.Error(401, "Usuário ou senha inválidos."));

        var senhaValida = _passwordHasherService.VerifyPassword(request.Senha, usuario.Senha);

        if (!senhaValida)
            return Unauthorized(ResponseDto<object>.Error(401, "Usuário ou senha inválidos."));

        var token = _tokenService.GerarToken(empresa, usuario);

        var response = new LoginResponseDto
        {
            Token = token,
            Usuario = new UsuarioLogadoDto
            {
                IdEmpresa = usuario.IdEmpresa,
                IdUsuario = usuario.IdUsuario,
                NomeUsuario = usuario.NomeUsuario,
                NomeCompleto = usuario.NomeCompleto,
                Email = usuario.Email,
                EhAdm = usuario.EhAdm
            }
        };

        return Ok(ResponseDto<LoginResponseDto>.Ok(response, "Login realizado com sucesso."));
    }
}