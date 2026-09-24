using DominoPontaDeQuina.Api.Contracts;
using DominoPontaDeQuina.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DominoPontaDeQuina.Api.Controllers;

[ApiController]
[Route("api/jogadores")]
[Produces("application/json")]
public class JogadoresController(
    IJogadorService jogadorService,
    IPartidaService partidaService) : ControllerBase
{
    /// <summary>Obtém um jogador com o total de partidas e vitórias.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<JogadorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JogadorResponse>> ObterPorId(Guid id, CancellationToken cancelamento)
    {
        var jogador = await jogadorService.ObterPorIdAsync(id, cancelamento);
        return jogador is null ? NotFound() : JogadorResponse.De(jogador);
    }

    /// <summary>Lista os jogadores que venceram ao menos uma partida.</summary>
    [HttpGet("vencedores")]
    public async Task<ActionResult<List<JogadorResponse>>> ListarVencedores(CancellationToken cancelamento)
    {
        var jogadores = await jogadorService.ListarVencedoresAsync(cancelamento);
        return jogadores.Select(JogadorResponse.De).ToList();
    }

    /// <summary>Lista o histórico de partidas de um jogador, da mais recente para a mais antiga.</summary>
    [HttpGet("{id:guid}/historico")]
    public async Task<ActionResult<List<HistoricoPartidaResponse>>> ListarHistorico(
        Guid id,
        CancellationToken cancelamento)
    {
        var historico = await partidaService.ListarHistoricoDoJogadorAsync(id, cancelamento);
        return historico.Select(HistoricoPartidaResponse.De).ToList();
    }
}
