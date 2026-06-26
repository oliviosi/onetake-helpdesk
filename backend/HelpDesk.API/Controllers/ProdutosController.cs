using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.Produtos;
using HelpDesk.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public ProdutosController(HelpDeskDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var idEmpresa = ObterIdEmpresa();

        var produtos = await _context.Produto
            .Where(p => p.IdEmpresa == idEmpresa)
            .OrderBy(p => p.DscProduto)
            .Select(p => new ProdutoResponseDto
            {
                IdEmpresa = p.IdEmpresa,
                IdProduto = p.IdProduto,
                DscProduto = p.DscProduto,
                Ativo = p.Ativo
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ProdutoResponseDto>>.Ok(produtos));
    }

    [HttpGet("{idProduto:int}")]
    public async Task<IActionResult> BuscarPorId(int idProduto)
    {
        var idEmpresa = ObterIdEmpresa();

        var produto = await _context.Produto
            .Where(p => p.IdEmpresa == idEmpresa && p.IdProduto == idProduto)
            .Select(p => new ProdutoResponseDto
            {
                IdEmpresa = p.IdEmpresa,
                IdProduto = p.IdProduto,
                DscProduto = p.DscProduto,
                Ativo = p.Ativo
            })
            .FirstOrDefaultAsync();

        if (produto == null)
            return NotFound(ResponseDto<object>.Error(404, "Produto não encontrado."));

        return Ok(ResponseDto<ProdutoResponseDto>.Ok(produto));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ProdutoCreateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.DscProduto))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a descrição do produto."));

        var jaExiste = await _context.Produto.AnyAsync(p =>
            p.IdEmpresa == idEmpresa &&
            p.DscProduto == request.DscProduto.Trim());

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe um produto com essa descrição."));

        var proximoId = await ObterProximoIdProduto(idEmpresa);

        var produto = new Produto
        {
            IdEmpresa = idEmpresa,
            IdProduto = proximoId,
            DscProduto = request.DscProduto.Trim(),
            Ativo = "S"
        };

        _context.Produto.Add(produto);
        await _context.SaveChangesAsync();

        var response = new ProdutoResponseDto
        {
            IdEmpresa = produto.IdEmpresa,
            IdProduto = produto.IdProduto,
            DscProduto = produto.DscProduto,
            Ativo = produto.Ativo
        };

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { idProduto = produto.IdProduto },
            ResponseDto<ProdutoResponseDto>.Ok(response, "Produto criado com sucesso.")
        );
    }

    [HttpPut("{idProduto:int}")]
    public async Task<IActionResult> Atualizar(int idProduto, [FromBody] ProdutoUpdateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.DscProduto))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a descrição do produto."));

        var produto = await _context.Produto
            .FirstOrDefaultAsync(p => p.IdEmpresa == idEmpresa && p.IdProduto == idProduto);

        if (produto == null)
            return NotFound(ResponseDto<object>.Error(404, "Produto não encontrado."));

        var descricao = request.DscProduto.Trim();

        var jaExiste = await _context.Produto.AnyAsync(p =>
            p.IdEmpresa == idEmpresa &&
            p.IdProduto != idProduto &&
            p.DscProduto == descricao);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Já existe outro produto com essa descrição."));

        produto.DscProduto = descricao;
        produto.Ativo = request.Ativo == "N" ? "N" : "S";

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            produto.IdEmpresa,
            produto.IdProduto
        }, "Produto atualizado com sucesso."));
    }

    [HttpPatch("{idProduto:int}/ativar")]
    public async Task<IActionResult> Ativar(int idProduto)
    {
        var idEmpresa = ObterIdEmpresa();

        var produto = await _context.Produto
            .FirstOrDefaultAsync(p => p.IdEmpresa == idEmpresa && p.IdProduto == idProduto);

        if (produto == null)
            return NotFound(ResponseDto<object>.Error(404, "Produto não encontrado."));

        produto.Ativo = "S";
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            produto.IdEmpresa,
            produto.IdProduto
        }, "Produto ativado com sucesso."));
    }

    [HttpPatch("{idProduto:int}/desativar")]
    public async Task<IActionResult> Desativar(int idProduto)
    {
        var idEmpresa = ObterIdEmpresa();

        var produto = await _context.Produto
            .FirstOrDefaultAsync(p => p.IdEmpresa == idEmpresa && p.IdProduto == idProduto);

        if (produto == null)
            return NotFound(ResponseDto<object>.Error(404, "Produto não encontrado."));

        produto.Ativo = "N";
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            produto.IdEmpresa,
            produto.IdProduto
        }, "Produto desativado com sucesso."));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }

    private async Task<int> ObterProximoIdProduto(int idEmpresa)
    {
        var ultimoId = await _context.Produto
            .Where(p => p.IdEmpresa == idEmpresa)
            .MaxAsync(p => (int?)p.IdProduto);

        return (ultimoId ?? 0) + 1;
    }
}