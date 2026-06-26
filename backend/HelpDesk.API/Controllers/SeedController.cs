using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.Entities;
using HelpDesk.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly HelpDeskDbContext _context;
    private readonly PasswordHasherService _passwordHasherService;
    private readonly IWebHostEnvironment _environment;

    public SeedController(
        HelpDeskDbContext context,
        PasswordHasherService passwordHasherService,
        IWebHostEnvironment environment)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
        _environment = environment;
    }

    [HttpPost("admin-inicial")]
    public async Task<IActionResult> CriarAdminInicial()
    {
        if (!_environment.IsDevelopment())
            return Forbid("Seed permitido apenas em ambiente de desenvolvimento.");

        var empresa = await _context.Empresa
            .FirstOrDefaultAsync(e => e.CodigoAcesso == "ONETAKE");

        if (empresa == null)
        {
            empresa = new Empresa
            {
                IdEmpresa = 1,
                DscEmpresa = "OneTake",
                CodigoAcesso = "ONETAKE",
                Email = "admin@onetake.com.br",
                Ativo = "S"
            };

            _context.Empresa.Add(empresa);
        }

        var usuario = await _context.Usuario
            .FirstOrDefaultAsync(u =>
                u.IdEmpresa == empresa.IdEmpresa &&
                u.NomeUsuario == "admin");

        if (usuario == null)
        {
            usuario = new Usuario
            {
                IdEmpresa = empresa.IdEmpresa,
                IdUsuario = 1,
                NomeUsuario = "admin",
                NomeCompleto = "Administrador",
                Email = "admin@onetake.com.br",
                Senha = _passwordHasherService.HashPassword("123456"),
                EhAdm = "S",
                Ativo = "S"
            };

            _context.Usuario.Add(usuario);
        }

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            CodigoAcesso = "ONETAKE",
            Usuario = "admin",
            Senha = "123456"
        }, "Seed inicial criado com sucesso."));
    }
}