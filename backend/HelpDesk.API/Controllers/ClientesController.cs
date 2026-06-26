using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.Clientes;
using HelpDesk.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public ClientesController(HelpDeskDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var idEmpresa = ObterIdEmpresa();

        var clientes = await _context.Cliente
            .Where(c => c.IdEmpresa == idEmpresa)
            .OrderBy(c => c.DscCliente)
            .Select(c => new ClienteResponseDto
            {
                IdEmpresa = c.IdEmpresa,
                IdCliente = c.IdCliente,
                DscCliente = c.DscCliente,
                Email = c.Email,
                Ativo = c.Ativo,
                CodClienteERP = c.CodClienteERP
            })
            .ToListAsync();

        return Ok(ResponseDto<List<ClienteResponseDto>>.Ok(clientes));
    }

    [HttpGet("{idCliente:int}")]
    public async Task<IActionResult> BuscarPorId(int idCliente)
    {
        var idEmpresa = ObterIdEmpresa();

        var cliente = await _context.Cliente
            .Where(c => c.IdEmpresa == idEmpresa && c.IdCliente == idCliente)
            .Select(c => new ClienteResponseDto
            {
                IdEmpresa = c.IdEmpresa,
                IdCliente = c.IdCliente,
                DscCliente = c.DscCliente,
                Email = c.Email,
                Ativo = c.Ativo,
                CodClienteERP = c.CodClienteERP
            })
            .FirstOrDefaultAsync();

        if (cliente == null)
            return NotFound(ResponseDto<object>.Error(404, "Cliente não encontrado."));

        return Ok(ResponseDto<ClienteResponseDto>.Ok(cliente));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ClienteCreateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.DscCliente))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o nome do cliente."));

        var proximoId = await ObterProximoIdCliente(idEmpresa);

        var cliente = new Cliente
        {
            IdEmpresa = idEmpresa,
            IdCliente = proximoId,
            DscCliente = request.DscCliente.Trim(),
            Email = request.Email?.Trim(),
            CodClienteERP = request.CodClienteERP?.Trim(),
            Ativo = "S"
        };

        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();

        var response = new ClienteResponseDto
        {
            IdEmpresa = cliente.IdEmpresa,
            IdCliente = cliente.IdCliente,
            DscCliente = cliente.DscCliente,
            Email = cliente.Email,
            Ativo = cliente.Ativo,
            CodClienteERP = cliente.CodClienteERP
        };

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { idCliente = cliente.IdCliente },
            ResponseDto<ClienteResponseDto>.Ok(response, "Cliente criado com sucesso.")
        );
    }

    [HttpPut("{idCliente:int}")]
    public async Task<IActionResult> Atualizar(int idCliente, [FromBody] ClienteUpdateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.DscCliente))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o nome do cliente."));

        var cliente = await _context.Cliente
            .FirstOrDefaultAsync(c => c.IdEmpresa == idEmpresa && c.IdCliente == idCliente);

        if (cliente == null)
            return NotFound(ResponseDto<object>.Error(404, "Cliente não encontrado."));

        cliente.DscCliente = request.DscCliente.Trim();
        cliente.Email = request.Email?.Trim();
        cliente.CodClienteERP = request.CodClienteERP?.Trim();
        cliente.Ativo = request.Ativo == "N" ? "N" : "S";

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            cliente.IdEmpresa,
            cliente.IdCliente
        }, "Cliente atualizado com sucesso."));
    }

    [HttpPatch("{idCliente:int}/ativar")]
    public async Task<IActionResult> Ativar(int idCliente)
    {
        var idEmpresa = ObterIdEmpresa();

        var cliente = await _context.Cliente
            .FirstOrDefaultAsync(c => c.IdEmpresa == idEmpresa && c.IdCliente == idCliente);

        if (cliente == null)
            return NotFound(ResponseDto<object>.Error(404, "Cliente não encontrado."));

        cliente.Ativo = "S";
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new { cliente.IdEmpresa, cliente.IdCliente }, "Cliente ativado com sucesso."));
    }

    [HttpPatch("{idCliente:int}/desativar")]
    public async Task<IActionResult> Desativar(int idCliente)
    {
        var idEmpresa = ObterIdEmpresa();

        var cliente = await _context.Cliente
            .FirstOrDefaultAsync(c => c.IdEmpresa == idEmpresa && c.IdCliente == idCliente);

        if (cliente == null)
            return NotFound(ResponseDto<object>.Error(404, "Cliente não encontrado."));

        cliente.Ativo = "N";
        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new { cliente.IdEmpresa, cliente.IdCliente }, "Cliente desativado com sucesso."));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }

    private async Task<int> ObterProximoIdCliente(int idEmpresa)
    {
        var ultimoId = await _context.Cliente
            .Where(c => c.IdEmpresa == idEmpresa)
            .MaxAsync(c => (int?)c.IdCliente);

        return (ultimoId ?? 0) + 1;
    }
}