using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Repository.Interfaces;

public interface IPartidaRepository
{
    Task<Partida> AdicionarAsync(Partida partida, CancellationToken cancelamento = default);
    Task<Partida?> ObterPorIdAsync(Guid id, CancellationToken cancelamento = default);
    Task<List<Partida>> ListarPorStatusAsync(StatusPartida status, CancellationToken cancelamento = default);
    Task<List<Partida>> ListarPorJogadorAsync(Guid jogadorId, CancellationToken cancelamento = default);
    Task<bool> AtualizarStatusAsync(Guid id, StatusPartida novoStatus, CancellationToken cancelamento = default);
    Task RemoverAsync(Guid id, CancellationToken cancelamento = default);
}
