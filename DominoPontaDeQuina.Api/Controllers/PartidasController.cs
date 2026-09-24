using DominoPontaDeQuina.Api.Contracts;
using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DominoPontaDeQuina.Api.Controllers;

[ApiController]
[Route("api/partidas")]
[Produces("application/json")]
public class PartidasController(IPartidaService partidaService) : ControllerBase
{
    /// <summary>Cria uma partida aguardando início com os jogadores na ordem informada.</summary>
    [HttpPost]
    [ProducesResponseType<PartidaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartidaResponse>> Criar(
        CriarPartidaRequest requisicao,
        CancellationToken cancelamento)
    {
        var partida = await partidaService.CriarAsync(requisicao.JogadorIds, cancelamento);
        var criada = await partidaService.ObterPorIdAsync(partida.Id, cancelamento);

        return CreatedAtAction(nameof(ObterPorId), new { id = partida.Id }, PartidaResponse.De(criada!));
    }

    /// <summary>Obtém uma partida com as participações.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<PartidaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartidaResponse>> ObterPorId(Guid id, CancellationToken cancelamento)
    {
        var partida = await partidaService.ObterPorIdAsync(id, cancelamento);
        return partida is null ? NotFound() : PartidaResponse.De(partida);
    }

    /// <summary>Lista as partidas com o status informado.</summary>
    [HttpGet]
    public async Task<ActionResult<List<PartidaResponse>>> ListarPorStatus(
        [FromQuery] StatusPartida status,
        CancellationToken cancelamento)
    {
        var partidas = await partidaService.ListarPorStatusAsync(status, cancelamento);
        return partidas.Select(PartidaResponse.De).ToList();
    }

    /// <summary>Inicia uma partida que está aguardando.</summary>
    [HttpPost("{id:guid}/iniciar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Iniciar(Guid id, CancellationToken cancelamento)
    {
        await partidaService.IniciarAsync(id, cancelamento);
        return NoContent();
    }

    /// <summary>Registra as pontuações e finaliza uma partida em andamento.</summary>
    [HttpPost("{id:guid}/finalizar")]
    [ProducesResponseType<PartidaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PartidaResponse>> Finalizar(
        Guid id,
        FinalizarPartidaRequest requisicao,
        CancellationToken cancelamento)
    {
        if (requisicao.Pontuacoes.Select(p => p.JogadorId).Distinct().Count() != requisicao.Pontuacoes.Count)
        {
            ModelState.AddModelError(nameof(requisicao.Pontuacoes), "Cada jogador deve aparecer uma única vez.");
            return ValidationProblem(ModelState);
        }

        var pontuacoes = requisicao.Pontuacoes.ToDictionary(p => p.JogadorId, p => p.Pontuacao);
        var partida = await partidaService.FinalizarAsync(id, pontuacoes, cancelamento);

        return PartidaResponse.De(partida);
    }

    /// <summary>Cancela uma partida que ainda não foi finalizada.</summary>
    [HttpPost("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancelar(Guid id, CancellationToken cancelamento)
    {
        await partidaService.CancelarAsync(id, cancelamento);
        return NoContent();
    }
}
