using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Repository.Interfaces;

public interface IParticipacaoPartidaRepository
{
    Task<ParticipacaoPartida> AdicionarAsync(ParticipacaoPartida participacao, CancellationToken cancelamento = default);
    Task<ParticipacaoPartida?> ObterPorIdAsync(Guid id, CancellationToken cancelamento = default);
    Task<List<ParticipacaoPartida>> ListarPorPartidaAsync(Guid partidaId, CancellationToken cancelamento = default);
    Task<List<ParticipacaoPartida>> ListarPorJogadorAsync(Guid jogadorId, CancellationToken cancelamento = default);
    Task<bool> AtualizarResultadoAsync(Guid id, int pontuacao, bool vencedor, CancellationToken cancelamento = default);
    Task RemoverAsync(Guid id, CancellationToken cancelamento = default);
}
