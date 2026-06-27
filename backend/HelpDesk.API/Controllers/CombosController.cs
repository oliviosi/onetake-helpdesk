using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.Combos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CombosController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public CombosController(HelpDeskDbContext context)
    {
        _context = context;
    }

    [HttpGet("clientes")]
    public async Task<IActionResult> Clientes()
    {
        var idEmpresa = ObterIdEmpresa();

        var dados = await _context.Cliente
            .Where(c => c.IdEmpresa == idEmpresa && c.Ativo == "S")
            .OrderBy(c => c.DscCliente)
            .Select(c => new ComboDto
            {
                Id = c.IdCliente,
                Descricao = c.DscCliente
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ComboDto>>.Ok(dados));
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> Usuarios([FromQuery] int? idCliente)
    {
        var idEmpresa = ObterIdEmpresa();

        var query = _context.Usuario
            .Where(u => u.IdEmpresa == idEmpresa && u.Ativo == "S");

        if (idCliente.HasValue)
            query = query.Where(u => u.IdCliente == idCliente.Value);

        var dados = await query
            .OrderBy(u => u.NomeCompleto ?? u.NomeUsuario)
            .Select(u => new ComboDto
            {
                Id = u.IdUsuario,
                Descricao = !string.IsNullOrWhiteSpace(u.NomeCompleto)
                    ? u.NomeCompleto
                    : u.NomeUsuario
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ComboDto>>.Ok(dados));
    }

    [HttpGet("produtos")]
    public async Task<IActionResult> Produtos()
    {
        var idEmpresa = ObterIdEmpresa();

        var dados = await _context.Produto
            .Where(p => p.IdEmpresa == idEmpresa && p.Ativo == "S")
            .OrderBy(p => p.DscProduto)
            .Select(p => new ComboDto
            {
                Id = p.IdProduto,
                Descricao = p.DscProduto
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ComboDto>>.Ok(dados));
    }

    [HttpGet("produtos-por-cliente/{idCliente:int}")]
    public async Task<IActionResult> ProdutosPorCliente(int idCliente)
    {
        var idEmpresa = ObterIdEmpresa();

        var clienteExiste = await _context.Cliente.AnyAsync(c =>
            c.IdEmpresa == idEmpresa &&
            c.IdCliente == idCliente &&
            c.Ativo == "S");

        if (!clienteExiste)
            return NotFound(ResponseDto<object>.Error(404, "Cliente não encontrado ou inativo."));

        var dados = await _context.ProdutoXCliente
            .Where(pc =>
                pc.IdEmpresa == idEmpresa &&
                pc.IdCliente == idCliente &&
                pc.Produto != null &&
                pc.Produto.Ativo == "S")
            .OrderBy(pc => pc.Produto!.DscProduto)
            .Select(pc => new ComboDto
            {
                Id = pc.IdProduto,
                Descricao = pc.Produto!.DscProduto
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ComboDto>>.Ok(dados));
    }

    [HttpGet("tipos-ocorrencia")]
    public async Task<IActionResult> TiposOcorrencia()
    {
        var idEmpresa = ObterIdEmpresa();

        var dados = await _context.TipoOcorrencia
            .Where(t => t.IdEmpresa == idEmpresa && t.Ativo == "S")
            .OrderBy(t => t.DscTipoOcorrencia)
            .Select(t => new ComboDto
            {
                Id = t.IdTipoOcorrencia,
                Descricao = t.DscTipoOcorrencia
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ComboDto>>.Ok(dados));
    }

    [HttpGet("status-ticket")]
    public async Task<IActionResult> StatusTicket()
    {
        var idEmpresa = ObterIdEmpresa();

        var dados = await _context.StatusTicket
            .Where(s => s.IdEmpresa == idEmpresa)
            .OrderBy(s => s.IdStatusTicket)
            .Select(s => new ComboDto
            {
                Id = s.IdStatusTicket,
                Descricao = s.DscStatusTicket
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ComboDto>>.Ok(dados));
    }

    [HttpGet("tecnicos")]
    public async Task<IActionResult> Tecnicos()
    {
        var idEmpresa = ObterIdEmpresa();

        var dados = await _context.Tecnico
            .Where(t => t.IdEmpresa == idEmpresa)
            .OrderBy(t => t.Nome)
            .Select(t => new ComboDto
            {
                Id = t.IdTecnico,
                Descricao = t.Nome
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ComboDto>>.Ok(dados));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }
}