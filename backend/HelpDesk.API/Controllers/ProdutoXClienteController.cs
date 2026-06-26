using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.ProdutoXCliente;
using HelpDesk.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutoXClienteController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public ProdutoXClienteController(HelpDeskDbContext context)
    {
        _context = context;
    }

    [HttpGet("cliente/{idCliente:int}")]
    public async Task<IActionResult> ListarPorCliente(int idCliente)
    {
        var idEmpresa = ObterIdEmpresa();

        var clienteExiste = await _context.Cliente.AnyAsync(c =>
            c.IdEmpresa == idEmpresa &&
            c.IdCliente == idCliente);

        if (!clienteExiste)
            return NotFound(ResponseDto<object>.Error(404, "Cliente não encontrado."));

        var produtos = await _context.ProdutoXCliente
            .Where(pc =>
                pc.IdEmpresa == idEmpresa &&
                pc.IdCliente == idCliente)
            .Include(pc => pc.Cliente)
            .Include(pc => pc.Produto)
            .OrderBy(pc => pc.Produto!.DscProduto)
            .Select(pc => new ProdutoXClienteResponseDto
            {
                IdEmpresa = pc.IdEmpresa,
                IdCliente = pc.IdCliente,
                DscCliente = pc.Cliente!.DscCliente,
                IdProduto = pc.IdProduto,
                DscProduto = pc.Produto!.DscProduto
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ProdutoXClienteResponseDto>>.Ok(produtos));
    }

    [HttpPost("vincular")]
    public async Task<IActionResult> Vincular([FromBody] ProdutoXClienteCreateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        var cliente = await _context.Cliente.FirstOrDefaultAsync(c =>
            c.IdEmpresa == idEmpresa &&
            c.IdCliente == request.IdCliente &&
            c.Ativo == "S");

        if (cliente == null)
            return NotFound(ResponseDto<object>.Error(404, "Cliente não encontrado ou inativo."));

        var produto = await _context.Produto.FirstOrDefaultAsync(p =>
            p.IdEmpresa == idEmpresa &&
            p.IdProduto == request.IdProduto &&
            p.Ativo == "S");

        if (produto == null)
            return NotFound(ResponseDto<object>.Error(404, "Produto não encontrado ou inativo."));

        var jaExiste = await _context.ProdutoXCliente.AnyAsync(pc =>
            pc.IdEmpresa == idEmpresa &&
            pc.IdCliente == request.IdCliente &&
            pc.IdProduto == request.IdProduto);

        if (jaExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Este produto já está vinculado ao cliente."));

        var vinculo = new Entities.ProdutoXCliente
        {
            IdEmpresa = idEmpresa,
            IdCliente = request.IdCliente,
            IdProduto = request.IdProduto
        };

        _context.ProdutoXCliente.Add(vinculo);
        await _context.SaveChangesAsync();

        var response = new ProdutoXClienteResponseDto
        {
            IdEmpresa = idEmpresa,
            IdCliente = cliente.IdCliente,
            DscCliente = cliente.DscCliente,
            IdProduto = produto.IdProduto,
            DscProduto = produto.DscProduto
        };

        return Ok(ResponseDto<ProdutoXClienteResponseDto>.Ok(response, "Produto vinculado ao cliente com sucesso."));
    }

    [HttpDelete("cliente/{idCliente:int}/produto/{idProduto:int}")]
    public async Task<IActionResult> RemoverVinculo(int idCliente, int idProduto)
    {
        var idEmpresa = ObterIdEmpresa();

        var vinculo = await _context.ProdutoXCliente.FirstOrDefaultAsync(pc =>
            pc.IdEmpresa == idEmpresa &&
            pc.IdCliente == idCliente &&
            pc.IdProduto == idProduto);

        if (vinculo == null)
            return NotFound(ResponseDto<object>.Error(404, "Vínculo não encontrado."));

        _context.ProdutoXCliente.Remove(vinculo);
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            IdEmpresa = idEmpresa,
            IdCliente = idCliente,
            IdProduto = idProduto
        }, "Vínculo removido com sucesso."));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }
}