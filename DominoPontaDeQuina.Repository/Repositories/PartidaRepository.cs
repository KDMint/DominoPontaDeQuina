using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class PartidaRepository(DominoDbContext contexto) : IPartidaRepository
{
    /// <summary>Adiciona uma nova partida ao banco.</summary>
    public async Task<Partida> AdicionarAsync(Partida partida, CancellationToken cancelamento = default)
    {
        ArgumentNullException.ThrowIfNull(partida);
        await contexto.Partidas.AddAsync(partida, cancelamento);
        await contexto.SaveChangesAsync(cancelamento);
        return partida;
    }

    /// <summary>Obtém uma partida pelo Id, incluindo todas as participações e jogadores.</summary>
    public Task<Partida?> ObterPorIdAsync(Guid id, CancellationToken cancelamento = default) =>
        contexto.Partidas
                .Include(p => p.Participacoes)
                    .ThenInclude(pp => pp.Jogador)
                .SingleOrDefaultAsync(p => p.Id == id, cancelamento);

    /// <summary>Lista todas as partidas com um determinado status.</summary>
    public Task<List<Partida>> ListarPorStatusAsync(StatusPartida status, CancellationToken cancelamento = default) =>
        contexto.Partidas
                .Where(p => p.Status == status)
                .Include(p => p.Participacoes)
                    .ThenInclude(pp => pp.Jogador)
                .OrderByDescending(p => p.IniciadoEm)
                .ToListAsync(cancelamento);

    /// <summary>Lista as partidas em que um determinado jogador participou.</summary>
    public Task<List<Partida>> ListarPorJogadorAsync(Guid jogadorId, CancellationToken cancelamento = default) =>
        contexto.Partidas
                .Where(p => p.Participacoes.Any(pp => pp.JogadorId == jogadorId))
                .Include(p => p.Participacoes)
                    .ThenInclude(pp => pp.Jogador)
                .OrderByDescending(p => p.IniciadoEm)
                .ToListAsync(cancelamento);

    /// <summary>Atualiza o status e, opcionalmente, a data de finalização de uma partida.</summary>
    public async Task<bool> AtualizarStatusAsync(Guid id, StatusPartida novoStatus, CancellationToken cancelamento = default)
    {
        var partida = await contexto.Partidas.FindAsync([id], cancelamento);
        if (partida is null) return false;

        partida.Status = novoStatus;
        if (novoStatus == StatusPartida.Finalizado)
            partida.FinalizadoEm = DateTime.UtcNow;

        await contexto.SaveChangesAsync(cancelamento);
        return true;
    }

    /// <summary>Remove uma partida pelo Id.</summary>
    public async Task RemoverAsync(Guid id, CancellationToken cancelamento = default)
    {
        var partida = await contexto.Partidas.FindAsync([id], cancelamento);
        if (partida is not null)
        {
            contexto.Partidas.Remove(partida);
            await contexto.SaveChangesAsync(cancelamento);
        }
    }
}
