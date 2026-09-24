using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Repository.Interfaces;

public interface IJogadorRepository
{
    Task<Jogador> AdicionarAsync(Jogador jogador, CancellationToken cancelamento = default);
    Task<Jogador?> ObterPorIdAsync(Guid id, CancellationToken cancelamento = default);
    Task<List<Jogador>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancelamento = default);
    Task<List<Jogador>> ListarComVitoriaAsync(CancellationToken cancelamento = default);
    Task RemoverAsync(Guid id, CancellationToken cancelamento = default);
}
