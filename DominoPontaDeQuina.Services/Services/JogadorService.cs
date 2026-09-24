using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Interfaces;
using DominoPontaDeQuina.Services.Interfaces;

namespace DominoPontaDeQuina.Services.Services;

public class JogadorService(IJogadorRepository jogadorRepository) : IJogadorService
{
    public Task<Jogador?> ObterPorIdAsync(Guid id, CancellationToken cancelamento = default) =>
        jogadorRepository.ObterPorIdAsync(id, cancelamento);

    public Task<List<Jogador>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancelamento = default) =>
        jogadorRepository.ListarPorUsuarioAsync(usuarioId, cancelamento);

    public Task<List<Jogador>> ListarVencedoresAsync(CancellationToken cancelamento = default) =>
        jogadorRepository.ListarComVitoriaAsync(cancelamento);
}
