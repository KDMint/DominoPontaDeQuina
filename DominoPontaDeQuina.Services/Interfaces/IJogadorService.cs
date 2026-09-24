using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Services.Interfaces;

public interface IJogadorService
{
    Task<Jogador?> ObterPorIdAsync(Guid id, CancellationToken cancelamento = default);
    Task<List<Jogador>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancelamento = default);
    Task<List<Jogador>> ListarVencedoresAsync(CancellationToken cancelamento = default);
}
