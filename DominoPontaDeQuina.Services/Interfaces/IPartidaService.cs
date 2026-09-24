using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Services.Interfaces;

public interface IPartidaService
{
    /// <summary>Cria uma partida aguardando início com os jogadores na ordem informada.</summary>
    Task<Partida> CriarAsync(IReadOnlyList<Guid> jogadorIds, CancellationToken cancelamento = default);

    /// <summary>Move uma partida de Aguardando para EmAndamento.</summary>
    Task IniciarAsync(Guid partidaId, CancellationToken cancelamento = default);

    /// <summary>Registra as pontuações, marca o(s) vencedor(es) e finaliza a partida.</summary>
    Task<Partida> FinalizarAsync(
        Guid partidaId,
        IReadOnlyDictionary<Guid, int> pontuacaoPorJogador,
        CancellationToken cancelamento = default);

    /// <summary>Cancela uma partida que ainda não foi finalizada.</summary>
    Task CancelarAsync(Guid partidaId, CancellationToken cancelamento = default);

    Task<Partida?> ObterPorIdAsync(Guid partidaId, CancellationToken cancelamento = default);

    Task<List<Partida>> ListarPorStatusAsync(StatusPartida status, CancellationToken cancelamento = default);

    Task<List<ParticipacaoPartida>> ListarHistoricoDoJogadorAsync(Guid jogadorId, CancellationToken cancelamento = default);
}
