using HelpDesk.API.Data;
using HelpDesk.API.DTOs;
using HelpDesk.API.DTOs.Tickets;
using HelpDesk.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly HelpDeskDbContext _context;

    public TicketsController(HelpDeskDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var idEmpresa = ObterIdEmpresa();

        var tickets = await _context.Ticket
            .Where(t => t.IdEmpresa == idEmpresa)
            .Include(t => t.Cliente)
            .Include(t => t.Usuario)
            .Include(t => t.TipoOcorrencia)
            .Include(t => t.StatusTicket)
            .OrderByDescending(t => t.DtHrAbertura)
            .Select(t => new TicketResumoDto
            {
                IdEmpresa = t.IdEmpresa,
                IdTicket = t.IdTicket,
                DtHrAbertura = t.DtHrAbertura,
                IdCliente = t.IdCliente,
                DscCliente = t.Cliente != null ? t.Cliente.DscCliente : string.Empty,
                IdUsuario = t.IdUsuario,
                NomeUsuario = t.Usuario != null ? t.Usuario.NomeUsuario : string.Empty,
                Assunto = t.Assunto,
                IdTipoOcorrencia = t.IdTipoOcorrencia,
                DscTipoOcorrencia = t.TipoOcorrencia != null ? t.TipoOcorrencia.DscTipoOcorrencia : string.Empty,
                Prioridade = t.Prioridade,
                IdStatusTicket = t.IdStatusTicket,
                DscStatusTicket = t.StatusTicket != null ? t.StatusTicket.DscStatusTicket : string.Empty,
                DtHrFinalizacao = t.DtHrFinalizacao,
                TicketCancelado = t.TicketCancelado
            })
            .ToListAsync();

        return Ok(ResponseDto<List<TicketResumoDto>>.Ok(tickets));
    }

    [HttpGet("{idTicket:int}")]
    public async Task<IActionResult> BuscarPorId(int idTicket)
    {
        var idEmpresa = ObterIdEmpresa();

        var ticket = await _context.Ticket
            .Where(t => t.IdEmpresa == idEmpresa && t.IdTicket == idTicket)
            .Include(t => t.Cliente)
            .Include(t => t.Usuario)
            .Include(t => t.TipoOcorrencia)
            .Include(t => t.StatusTicket)
            .Include(t => t.TecnicoFinalizacao)
            .FirstOrDefaultAsync();

        if (ticket == null)
            return NotFound(ResponseDto<object>.Error(404, "Ticket não encontrado."));

        var interacoes = await _context.TicketInteracao
            .Where(i => i.IdEmpresa == idEmpresa && i.IdTicket == idTicket)
            .Include(i => i.Tecnico)
            .Include(i => i.Usuario)
            .OrderBy(i => i.DtHrInteracao)
            .Select(i => new TicketInteracaoDto
            {
                IdEmpresa = i.IdEmpresa,
                IdTicketInteracao = i.IdTicketInteracao,
                IdTicket = i.IdTicket,
                DtHrInteracao = i.DtHrInteracao,
                DscInteracao = i.DscInteracao,
                IdTecnico = i.IdTecnico,
                NomeTecnico = i.Tecnico != null ? i.Tecnico.Nome : null,
                IdUsuario = i.IdUsuario,
                NomeUsuario = i.Usuario != null ? i.Usuario.NomeUsuario : null,
                Privativo = i.Privativo,
                AguardarInteracaoUsuario = i.AguardarInteracaoUsuario
            })
            .ToListAsync();

        var response = new TicketDetalheDto
        {
            IdEmpresa = ticket.IdEmpresa,
            IdTicket = ticket.IdTicket,
            DtHrAbertura = ticket.DtHrAbertura,
            IdCliente = ticket.IdCliente,
            DscCliente = ticket.Cliente != null ? ticket.Cliente.DscCliente : string.Empty,
            IdUsuario = ticket.IdUsuario,
            NomeUsuario = ticket.Usuario != null ? ticket.Usuario.NomeUsuario : string.Empty,
            Assunto = ticket.Assunto,
            IdTipoOcorrencia = ticket.IdTipoOcorrencia,
            DscTipoOcorrencia = ticket.TipoOcorrencia != null ? ticket.TipoOcorrencia.DscTipoOcorrencia : string.Empty,
            DscTicket = ticket.DscTicket,
            Prioridade = ticket.Prioridade,
            IdStatusTicket = ticket.IdStatusTicket,
            DscStatusTicket = ticket.StatusTicket != null ? ticket.StatusTicket.DscStatusTicket : string.Empty,
            DtHrFinalizacao = ticket.DtHrFinalizacao,
            IdTecnicoFinalizacao = ticket.IdTecnicoFinalizacao,
            NomeTecnicoFinalizacao = ticket.TecnicoFinalizacao != null ? ticket.TecnicoFinalizacao.Nome : null,
            TicketCancelado = ticket.TicketCancelado,
            Interacoes = interacoes
        };

        return Ok(ResponseDto<TicketDetalheDto>.Ok(response));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TicketCreateDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (request.IdCliente <= 0)
            return BadRequest(ResponseDto<object>.Error(400, "Informe o cliente."));

        if (request.IdUsuario <= 0)
            return BadRequest(ResponseDto<object>.Error(400, "Informe o usuário solicitante."));

        if (string.IsNullOrWhiteSpace(request.Assunto))
            return BadRequest(ResponseDto<object>.Error(400, "Informe o assunto do ticket."));

        if (string.IsNullOrWhiteSpace(request.DscTicket))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a descrição do ticket."));

        var clienteExiste = await _context.Cliente.AnyAsync(c =>
            c.IdEmpresa == idEmpresa &&
            c.IdCliente == request.IdCliente &&
            c.Ativo == "S");

        if (!clienteExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Cliente não encontrado ou inativo."));

        var usuarioExiste = await _context.Usuario.AnyAsync(u =>
            u.IdEmpresa == idEmpresa &&
            u.IdUsuario == request.IdUsuario &&
            u.Ativo == "S");

        if (!usuarioExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Usuário solicitante não encontrado ou inativo."));

        var tipoExiste = await _context.TipoOcorrencia.AnyAsync(t =>
            t.IdEmpresa == idEmpresa &&
            t.IdTipoOcorrencia == request.IdTipoOcorrencia &&
            t.Ativo == "S");

        if (!tipoExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Tipo de ocorrência não encontrado ou inativo."));

        var statusInicial = await ObterStatusInicial(idEmpresa);

        if (statusInicial == null)
            return BadRequest(ResponseDto<object>.Error(400, "Cadastre pelo menos um status inicial para tickets."));

        var proximoIdTicket = await ObterProximoIdTicket(idEmpresa);
        var proximoIdInteracao = await ObterProximoIdTicketInteracao(idEmpresa);

        var ticket = new Ticket
        {
            IdEmpresa = idEmpresa,
            IdTicket = proximoIdTicket,
            DtHrAbertura = DateTime.Now,
            IdCliente = request.IdCliente,
            IdUsuario = request.IdUsuario,
            Assunto = request.Assunto.Trim(),
            IdTipoOcorrencia = request.IdTipoOcorrencia,
            DscTicket = request.DscTicket.Trim(),
            Prioridade = NormalizarPrioridade(request.Prioridade),
            IdStatusTicket = statusInicial.IdStatusTicket,
            TicketCancelado = "N"
        };

        var interacaoInicial = new TicketInteracao
        {
            IdEmpresa = idEmpresa,
            IdTicketInteracao = proximoIdInteracao,
            IdTicket = proximoIdTicket,
            DtHrInteracao = DateTime.Now,
            DscInteracao = request.DscTicket.Trim(),
            IdUsuario = request.IdUsuario,
            Privativo = "N",
            AguardarInteracaoUsuario = "N"
        };

        _context.Ticket.Add(ticket);
        _context.TicketInteracao.Add(interacaoInicial);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { idTicket = ticket.IdTicket },
            ResponseDto<object>.Ok(new
            {
                ticket.IdEmpresa,
                ticket.IdTicket
            }, "Ticket criado com sucesso.")
        );
    }

    [HttpPost("{idTicket:int}/responder")]
    public async Task<IActionResult> Responder(int idTicket, [FromBody] TicketResponderDto request)
    {
        var idEmpresa = ObterIdEmpresa();

        if (string.IsNullOrWhiteSpace(request.Mensagem))
            return BadRequest(ResponseDto<object>.Error(400, "Informe a mensagem."));

        var ticket = await _context.Ticket
            .FirstOrDefaultAsync(t =>
                t.IdEmpresa == idEmpresa &&
                t.IdTicket == idTicket);

        if (ticket == null)
            return NotFound(ResponseDto<object>.Error(404, "Ticket não encontrado."));

        if (ticket.TicketCancelado == "S")
            return BadRequest(ResponseDto<object>.Error(400, "Ticket cancelado não pode receber resposta."));

        if (ticket.DtHrFinalizacao.HasValue)
            return BadRequest(ResponseDto<object>.Error(400, "Ticket finalizado não pode receber resposta."));

        if (!request.IdTecnico.HasValue && !request.IdUsuario.HasValue)
            return BadRequest(ResponseDto<object>.Error(400, "Informe o técnico ou usuário da interação."));

        if (request.IdTecnico.HasValue)
        {
            var tecnicoExiste = await _context.Tecnico.AnyAsync(t =>
                t.IdEmpresa == idEmpresa &&
                t.IdTecnico == request.IdTecnico.Value);

            if (!tecnicoExiste)
                return BadRequest(ResponseDto<object>.Error(400, "Técnico não encontrado."));
        }

        if (request.IdUsuario.HasValue)
        {
            var usuarioExiste = await _context.Usuario.AnyAsync(u =>
                u.IdEmpresa == idEmpresa &&
                u.IdUsuario == request.IdUsuario.Value &&
                u.Ativo == "S");

            if (!usuarioExiste)
                return BadRequest(ResponseDto<object>.Error(400, "Usuário não encontrado ou inativo."));
        }

        var proximoIdInteracao = await ObterProximoIdTicketInteracao(idEmpresa);

        var interacao = new TicketInteracao
        {
            IdEmpresa = idEmpresa,
            IdTicketInteracao = proximoIdInteracao,
            IdTicket = idTicket,
            DtHrInteracao = DateTime.Now,
            DscInteracao = request.Mensagem.Trim(),
            IdTecnico = request.IdTecnico,
            IdUsuario = request.IdUsuario,
            Privativo = request.Privativo == "S" ? "S" : "N",
            AguardarInteracaoUsuario = request.AguardarInteracaoUsuario == "S" ? "S" : "N"
        };

        _context.TicketInteracao.Add(interacao);

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            interacao.IdEmpresa,
            interacao.IdTicket,
            interacao.IdTicketInteracao
        }, "Resposta adicionada com sucesso."));
    }

    [HttpPatch("{idTicket:int}/fechar")]
    public async Task<IActionResult> Fechar(int idTicket, [FromQuery] int idTecnico)
    {
        var idEmpresa = ObterIdEmpresa();

        var ticket = await _context.Ticket
            .FirstOrDefaultAsync(t => t.IdEmpresa == idEmpresa && t.IdTicket == idTicket);

        if (ticket == null)
            return NotFound(ResponseDto<object>.Error(404, "Ticket não encontrado."));

        if (ticket.TicketCancelado == "S")
            return BadRequest(ResponseDto<object>.Error(400, "Ticket cancelado não pode ser fechado."));

        if (ticket.DtHrFinalizacao.HasValue)
            return BadRequest(ResponseDto<object>.Error(400, "Ticket já está fechado."));

        var tecnicoExiste = await _context.Tecnico.AnyAsync(t =>
            t.IdEmpresa == idEmpresa &&
            t.IdTecnico == idTecnico);

        if (!tecnicoExiste)
            return BadRequest(ResponseDto<object>.Error(400, "Técnico não encontrado."));

        var statusFechado = await ObterStatusFechado(idEmpresa);

        if (statusFechado == null)
            return BadRequest(ResponseDto<object>.Error(400, "Cadastre um status de fechamento com TipoTicket = FECHADO."));

        ticket.DtHrFinalizacao = DateTime.Now;
        ticket.IdTecnicoFinalizacao = idTecnico;
        ticket.IdStatusTicket = statusFechado.IdStatusTicket;

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            ticket.IdEmpresa,
            ticket.IdTicket,
            ticket.DtHrFinalizacao
        }, "Ticket fechado com sucesso."));
    }

    [HttpPatch("{idTicket:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int idTicket)
    {
        var idEmpresa = ObterIdEmpresa();

        var ticket = await _context.Ticket
            .FirstOrDefaultAsync(t => t.IdEmpresa == idEmpresa && t.IdTicket == idTicket);

        if (ticket == null)
            return NotFound(ResponseDto<object>.Error(404, "Ticket não encontrado."));

        if (ticket.DtHrFinalizacao.HasValue)
            return BadRequest(ResponseDto<object>.Error(400, "Ticket fechado não pode ser cancelado."));

        ticket.TicketCancelado = "S";

        var statusCancelado = await ObterStatusCancelado(idEmpresa);

        if (statusCancelado != null)
            ticket.IdStatusTicket = statusCancelado.IdStatusTicket;

        await _context.SaveChangesAsync();

        return Ok(ResponseDto<object>.Ok(new
        {
            ticket.IdEmpresa,
            ticket.IdTicket
        }, "Ticket cancelado com sucesso."));
    }

    private int ObterIdEmpresa()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

        if (!int.TryParse(claim, out var idEmpresa))
            throw new UnauthorizedAccessException("Empresa não identificada no token.");

        return idEmpresa;
    }

    private async Task<int> ObterProximoIdTicket(int idEmpresa)
    {
        var ultimoId = await _context.Ticket
            .Where(t => t.IdEmpresa == idEmpresa)
            .MaxAsync(t => (int?)t.IdTicket);

        return (ultimoId ?? 0) + 1;
    }

    private async Task<int> ObterProximoIdTicketInteracao(int idEmpresa)
    {
        var ultimoId = await _context.TicketInteracao
            .Where(t => t.IdEmpresa == idEmpresa)
            .MaxAsync(t => (int?)t.IdTicketInteracao);

        return (ultimoId ?? 0) + 1;
    }

    private async Task<StatusTicket?> ObterStatusInicial(int idEmpresa)
    {
        var statusAberto = await _context.StatusTicket
            .Where(s => s.IdEmpresa == idEmpresa)
            .OrderBy(s => s.IdStatusTicket)
            .FirstOrDefaultAsync(s =>
                s.TipoTicket == "ABERTO" ||
                s.DscStatusTicket.ToLower() == "aberto");

        if (statusAberto != null)
            return statusAberto;

        return await _context.StatusTicket
            .Where(s => s.IdEmpresa == idEmpresa)
            .OrderBy(s => s.IdStatusTicket)
            .FirstOrDefaultAsync();
    }

    private async Task<StatusTicket?> ObterStatusFechado(int idEmpresa)
    {
        return await _context.StatusTicket
            .Where(s => s.IdEmpresa == idEmpresa)
            .FirstOrDefaultAsync(s =>
                s.TipoTicket == "FECHADO" ||
                s.DscStatusTicket.ToLower() == "fechado");
    }

    private async Task<StatusTicket?> ObterStatusCancelado(int idEmpresa)
    {
        return await _context.StatusTicket
            .Where(s => s.IdEmpresa == idEmpresa)
            .FirstOrDefaultAsync(s =>
                s.TipoTicket == "CANCELADO" ||
                s.DscStatusTicket.ToLower() == "cancelado");
    }

    private static string NormalizarPrioridade(string prioridade)
    {
        if (string.IsNullOrWhiteSpace(prioridade))
            return "Normal";

        var valor = prioridade.Trim();

        return valor.ToLower() switch
        {
            "baixa" => "Baixa",
            "normal" => "Normal",
            "média" => "Média",
            "media" => "Média",
            "alta" => "Alta",
            "urgente" => "Urgente",
            _ => "Normal"
        };
    }
}